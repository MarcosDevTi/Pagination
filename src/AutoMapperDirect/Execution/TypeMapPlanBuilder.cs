using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using AutoMapperDirect.Configuration;
using AutoMapperDirect.Internal;

namespace AutoMapperDirect.Execution
{
    public class TypeMapPlanBuilder
    {
        private static readonly Expression<Func<AutoMapperMappingException>> CtorExpression =
            () => new AutoMapperMappingException(null, null, default, null, null);

        private static readonly Expression<Action<ResolutionContext>> IncTypeDepthInfo =
            ctxt => ctxt.IncrementTypeDepth(default);

        private static readonly Expression<Action<ResolutionContext>> ValidateMap =
            ctxt => ctxt.ValidateMap(default);

        private static readonly Expression<Action<ResolutionContext>> DecTypeDepthInfo =
            ctxt => ctxt.DecrementTypeDepth(default);

        private static readonly Expression<Func<ResolutionContext, int>> GetTypeDepthInfo =
            ctxt => ctxt.GetTypeDepth(default);

        private readonly IConfigurationProvider _configurationProvider;
        private readonly ParameterExpression _destination;
        private readonly ParameterExpression _initialDestination;
        private readonly TypeMap _typeMap;

        public TypeMapPlanBuilder(IConfigurationProvider configurationProvider, TypeMap typeMap)
        {
            _configurationProvider = configurationProvider;
            _typeMap = typeMap;
            Source = Expression.Parameter(typeMap.SourceType, "src");
            _initialDestination = Expression.Parameter(typeMap.DestinationTypeToUse, "dest");
            Context = Expression.Parameter(typeof(ResolutionContext), "ctxt");
            _destination = Expression.Variable(_initialDestination.Type, "typeMapDestination");
        }

        public ParameterExpression Source { get; }

        public ParameterExpression Context { get; }

        public LambdaExpression CreateMapperLambda(HashSet<TypeMap> typeMapsPath)
        {
            var customExpression = TypeConverterMapper() ?? _typeMap.CustomMapFunction ?? _typeMap.CustomMapExpression;
            if(customExpression != null)
            {
                return Expression.Lambda(customExpression.ReplaceParameters(Source, _initialDestination, Context), Source, _initialDestination, Context);
            }

            CheckForCycles(typeMapsPath);

            if(typeMapsPath != null)
            {
                return null;
            }

            var destinationFunc = CreateDestinationFunc();

            var assignmentFunc = CreateAssignmentFunc(destinationFunc);

            var mapperFunc = CreateMapperFunc(assignmentFunc);

            var checkContext = ExpressionBuilder.CheckContext(_typeMap, Context);
            var lambaBody = checkContext != null ? new[] {checkContext, mapperFunc} : new[] {mapperFunc};

            return Expression.Lambda(Expression.Block(new[] {_destination}, lambaBody), Source, _initialDestination, Context);
        }

        private void CheckForCycles(HashSet<TypeMap> typeMapsPath)
        {
            var inlineWasChecked = _typeMap.WasInlineChecked;
            _typeMap.WasInlineChecked = true;
            if(typeMapsPath == null)
            {
                typeMapsPath = new HashSet<TypeMap>();
            }
            typeMapsPath.Add(_typeMap);
            var members = 
                _typeMap.MemberMaps.Where(pm=>pm.CanResolveValue)
                .ToArray()
                .Select(pm=> new { MemberTypeMap = ResolveMemberTypeMap(pm), MemberMap = pm })
                .Where(p => p.MemberTypeMap != null && !p.MemberTypeMap.PreserveReferences && p.MemberTypeMap.MapExpression == null);
            foreach(var item in members)
            {
                var memberMap = item.MemberMap;
                var memberTypeMap = item.MemberTypeMap;
                if(!inlineWasChecked && typeMapsPath.Count % _configurationProvider.MaxExecutionPlanDepth == 0)
                {
                    memberMap.Inline = false;
                    Debug.WriteLine($"Resetting Inline: {memberMap.DestinationName} in {_typeMap.SourceType} - {_typeMap.DestinationType}");
                }
                if(typeMapsPath.Contains(memberTypeMap))
                {
                    if(memberTypeMap.SourceType.IsValueType())
                    {
                        if(memberTypeMap.MaxDepth == 0)
                        {
                            memberTypeMap.MaxDepth = 10;
                        }
                        typeMapsPath.Remove(_typeMap);
                        return;
                    }

                    SetPreserveReferences(memberTypeMap);
                    foreach(var derivedTypeMap in memberTypeMap.IncludedDerivedTypes.Select(ResolveTypeMap))
                    {
                        SetPreserveReferences(derivedTypeMap);
                    }
                }
                memberTypeMap.CreateMapperLambda(_configurationProvider, typeMapsPath);
            }
            typeMapsPath.Remove(_typeMap);
            return;

            void SetPreserveReferences(TypeMap memberTypeMap)
            {
                Debug.WriteLine($"Setting PreserveReferences: {_typeMap.SourceType} - {_typeMap.DestinationType} => {memberTypeMap.SourceType} - {memberTypeMap.DestinationType}");
                memberTypeMap.PreserveReferences = true;
            }

            TypeMap ResolveMemberTypeMap(IMemberMap memberMap)
            {
                if(memberMap.SourceType == null)
                {
                    return null;
                }
                var types = new TypePair(memberMap.SourceType, memberMap.DestinationType);
                return ResolveTypeMap(types);
            }

            TypeMap ResolveTypeMap(TypePair types)
            {
                var typeMap = _configurationProvider.ResolveTypeMap(types);
                if(typeMap == null && _configurationProvider.FindMapper(types) is IObjectMapperInfo mapper)
                {
                    typeMap = _configurationProvider.ResolveTypeMap(mapper.GetAssociatedTypes(types));
                }
                return typeMap;
            }
        }

        private LambdaExpression TypeConverterMapper()
        {
            if (_typeMap.TypeConverterType == null)
                return null;
            Type type;
            if (_typeMap.TypeConverterType.IsGenericTypeDefinition())
            {
                var genericTypeParam = _typeMap.SourceType.IsGenericType()
                    ? _typeMap.SourceType.GetTypeInfo().GenericTypeArguments[0]
                    : _typeMap.DestinationTypeToUse.GetTypeInfo().GenericTypeArguments[0];
                type = _typeMap.TypeConverterType.MakeGenericType(genericTypeParam);
            }
            else
            {
                type = _typeMap.TypeConverterType;
            }
            // (src, dest, ctxt) => ((ITypeConverter<TSource, TDest>)ctxt.Options.CreateInstance<TypeConverterType>()).ToType(src, ctxt);
            var converterInterfaceType =
                typeof(ITypeConverter<,>).MakeGenericType(_typeMap.SourceType, _typeMap.DestinationTypeToUse);
            return Expression.Lambda(
                Expression.Call(
                    ExpressionFactory.ToType(CreateInstance(type), converterInterfaceType),
                    converterInterfaceType.GetDeclaredMethod("Convert"),
                    Source, _initialDestination, Context
                ),
                Source, _initialDestination, Context);
        }

        private Expression CreateDestinationFunc()
        {
            var newDestFunc = ExpressionFactory.ToType(CreateNewDestinationFunc(), _typeMap.DestinationTypeToUse);

            var getDest = _typeMap.DestinationTypeToUse.IsValueType() ? newDestFunc : Expression.Coalesce(_initialDestination, newDestFunc);

            Expression destinationFunc = Expression.Assign(_destination, getDest);

            if (_typeMap.PreserveReferences)
            {
                var dest = Expression.Variable(typeof(object), "dest");
                var setValue = Context.Type.GetDeclaredMethod("CacheDestination");
                var set = Expression.Call(Context, setValue, Source, Expression.Constant(_destination.Type), _destination);
                var setCache = Expression.IfThen(Expression.NotEqual(Source, Expression.Constant(null)), set);

                destinationFunc = Expression.Block(new[] {dest}, Expression.Assign(dest, destinationFunc), setCache, dest);
            }
            return destinationFunc;
        }

        private Expression CreateAssignmentFunc(Expression destinationFunc)
        {
            var isConstructorMapping = _typeMap.IsConstructorMapping;
            var actions = new List<Expression>();
            foreach (var propertyMap in _typeMap.PropertyMaps.Where(pm => pm.CanResolveValue))
            {
                var property = TryPropertyMap(propertyMap);
                if (isConstructorMapping && _typeMap.ConstructorParameterMatches(propertyMap.DestinationName))
                    property = _initialDestination.IfNullElse(Expression.Default(property.Type), property);
                actions.Add(property);
            }
            foreach (var pathMap in _typeMap.PathMaps.Where(pm => !pm.Ignored))
                actions.Add(TryPathMap(pathMap));
            foreach (var beforeMapAction in _typeMap.BeforeMapActions)
                actions.Insert(0, beforeMapAction.ReplaceParameters(Source, _destination, Context));
            actions.Insert(0, destinationFunc);
            if (_typeMap.MaxDepth > 0)
            {
                actions.Insert(0,
                    Expression.Call(Context, ((MethodCallExpression) IncTypeDepthInfo.Body).Method, Expression.Constant(_typeMap.Types)));
            }
            if (_typeMap.IsConventionMap && _typeMap.Profile.ValidateInlineMaps)
            {
                actions.Insert(0, Expression.Call(Context, ((MethodCallExpression)ValidateMap.Body).Method, Expression.Constant(_typeMap)));
            }
            actions.AddRange(
                _typeMap.AfterMapActions.Select(
                    afterMapAction => afterMapAction.ReplaceParameters(Source, _destination, Context)));

            if (_typeMap.MaxDepth > 0)
                actions.Add(Expression.Call(Context, ((MethodCallExpression) DecTypeDepthInfo.Body).Method,
                    Expression.Constant(_typeMap.Types)));

            actions.Add(_destination);

            return Expression.Block(actions);
        }

        private Expression TryPathMap(PathMap pathMap)
        {
            var destination = ((MemberExpression) pathMap.DestinationExpression.ConvertReplaceParameters(_destination)).Expression;
            var createInnerObjects = CreateInnerObjects(destination);
            var setFinalValue = CreatePropertyMapFunc(pathMap, destination, pathMap.MemberPath.Last);

            var pathMapExpression = Expression.Block(createInnerObjects, setFinalValue);

            return TryMemberMap(pathMap, pathMapExpression);
        }

        private Expression CreateInnerObjects(Expression destination) => Expression.Block(destination.GetMembers()
            .Select(NullCheck)
            .Reverse()
            .Concat(new[] {Expression.Empty()}));

        private Expression NullCheck(MemberExpression memberExpression)
        {
            var setter = ExpressionFactory.GetSetter(memberExpression);
            var ifNull = setter == null
                ? (Expression)
                Expression.Throw(Expression.Constant(new NullReferenceException(
                    $"{memberExpression} cannot be null because it's used by ForPath.")))
                : Expression.Assign(setter, DelegateFactory.GenerateConstructorExpression(memberExpression.Type));
            return memberExpression.IfNullElse(ifNull);
        }

        private Expression CreateMapperFunc(Expression assignmentFunc)
        {
            var mapperFunc = assignmentFunc;

            if(_typeMap.Condition != null)
                mapperFunc =
                    Expression.Condition(_typeMap.Condition.Body,
                        mapperFunc, Expression.Default(_typeMap.DestinationTypeToUse));

            if(_typeMap.MaxDepth > 0)
                mapperFunc = Expression.Condition(
                    Expression.LessThanOrEqual(
                        Expression.Call(Context, ((MethodCallExpression)GetTypeDepthInfo.Body).Method, Expression.Constant(_typeMap.Types)),
                        Expression.Constant(_typeMap.MaxDepth)
                    ),
                    mapperFunc,
                    Expression.Default(_typeMap.DestinationTypeToUse));

            if(_typeMap.Profile.AllowNullDestinationValues)
                mapperFunc = Source.IfNullElse(Expression.Default(_typeMap.DestinationTypeToUse), mapperFunc);

            return CheckReferencesCache(mapperFunc);
        }

        private Expression CheckReferencesCache(Expression valueBuilder)
        {
            if(!_typeMap.PreserveReferences)
            {
                return valueBuilder;
            }
            var cache = Expression.Variable(_typeMap.DestinationTypeToUse, "cachedDestination");
            var getDestination = Context.Type.GetDeclaredMethod("GetDestination");
            var assignCache =
                Expression.Assign(cache,
                    ExpressionFactory.ToType(Expression.Call(Context, getDestination, Source, Expression.Constant(_destination.Type)), _destination.Type));
            var condition = Expression.Condition(
                Expression.AndAlso(Expression.NotEqual(Source, Expression.Constant(null)), Expression.NotEqual(assignCache, Expression.Constant(null))),
                cache,
                valueBuilder);
            return Expression.Block(new[] { cache }, condition);
        }

        private Expression CreateNewDestinationFunc()
        {
            if(_typeMap.CustomCtorExpression != null)
            {
                return _typeMap.CustomCtorExpression.ReplaceParameters(Source);
            }
            if(_typeMap.CustomCtorFunction != null)
            {
                return _typeMap.CustomCtorFunction.ReplaceParameters(Source, Context);
            }
            if(_typeMap.ConstructDestinationUsingServiceLocator)
            {
                return CreateInstance(_typeMap.DestinationTypeToUse);
            }
            if(_typeMap.ConstructorMap?.CanResolve == true)
            {
                return CreateNewDestinationExpression(_typeMap.ConstructorMap);
            }
            if(_typeMap.DestinationTypeToUse.IsInterface())
            {
                var ctor = Expression.Call(null,
                    typeof(DelegateFactory).GetDeclaredMethod(nameof(DelegateFactory.CreateCtor), new[] { typeof(Type) }),
                    Expression.Call(null,
                        typeof(ProxyGenerator).GetDeclaredMethod(nameof(ProxyGenerator.GetProxyType)),
                        Expression.Constant(_typeMap.DestinationTypeToUse)));
                // We're invoking a delegate here to make it have the right accessibility
                return Expression.Invoke(ctor);
            }
            return DelegateFactory.GenerateConstructorExpression(_typeMap.DestinationTypeToUse);
        }

        private Expression CreateNewDestinationExpression(ConstructorMap constructorMap)
        {
            var ctorArgs = constructorMap.CtorParams.Select(CreateConstructorParameterExpression);
            var variables = constructorMap.Ctor.GetParameters().Select(parameter => Expression.Variable(parameter.ParameterType, parameter.Name)).ToArray();
            var body = variables.Zip(ctorArgs,
                                                (variable, expression) => (Expression) Expression.Assign(variable, ExpressionFactory.ToType(expression, variable.Type)))
                                                .Concat(new[] { CheckReferencesCache(Expression.New(constructorMap.Ctor, variables)) })
                                                .ToArray();
            return Expression.Block(variables, body);
        }

        private Expression ResolveSource(ConstructorParameterMap ctorParamMap)
        {
            if(ctorParamMap.CustomMapExpression != null)
                return ctorParamMap.CustomMapExpression.ConvertReplaceParameters(Source)
                    .NullCheck(ctorParamMap.DestinationType);
            if(ctorParamMap.CustomMapFunction != null)
                return ctorParamMap.CustomMapFunction.ConvertReplaceParameters(Source, Context);
            if (ctorParamMap.HasDefaultValue)
                return Expression.Constant(ctorParamMap.Parameter.GetDefaultValue(), ctorParamMap.Parameter.ParameterType);
            return Chain(ctorParamMap.SourceMembers, ctorParamMap.DestinationType);
        }

        private Expression CreateConstructorParameterExpression(ConstructorParameterMap ctorParamMap)
        {
            var resolvedExpression = ResolveSource(ctorParamMap);
            var resolvedValue = Expression.Variable(resolvedExpression.Type, "resolvedValue");
            var tryMap = Expression.Block(new[] {resolvedValue},
                Expression.Assign(resolvedValue, resolvedExpression),
                ExpressionBuilder.MapExpression(_configurationProvider, _typeMap.Profile, new TypePair(resolvedExpression.Type, ctorParamMap.DestinationType), resolvedValue, Context));
            return TryMemberMap(ctorParamMap, tryMap);
        }

        private Expression TryPropertyMap(PropertyMap propertyMap)
        {
            var pmExpression = CreatePropertyMapFunc(propertyMap, _destination, propertyMap.DestinationMember);

            if (pmExpression == null)
                return null;

            return TryMemberMap(propertyMap, pmExpression);
        }

        private static Expression TryMemberMap(IMemberMap memberMap, Expression memberMapExpression)
        {
            var exception = Expression.Parameter(typeof(Exception), "ex");

            var mappingExceptionCtor = ((NewExpression) CtorExpression.Body).Constructor;

            return Expression.TryCatch(memberMapExpression,
                        Expression.MakeCatchBlock(typeof(Exception), exception,
                            Expression.Block(
                                Expression.Throw(Expression.New(mappingExceptionCtor, Expression.Constant("Error mapping types."), exception,
                                    Expression.Constant(memberMap.TypeMap.Types), Expression.Constant(memberMap.TypeMap), Expression.Constant(memberMap))),
                                Expression.Default(memberMapExpression.Type))
                            , null));
        }

        private Expression CreatePropertyMapFunc(IMemberMap memberMap, Expression destination, MemberInfo destinationMember)
        {
            var destMember = Expression.MakeMemberAccess(destination, destinationMember);

            Expression getter;

            if (destinationMember is PropertyInfo pi && pi.GetGetMethod(true) == null)
                getter = Expression.Default(memberMap.DestinationType);
            else
                getter = destMember;

            Expression destValueExpr;
            if (memberMap.UseDestinationValue)
            {
                destValueExpr = getter;
            }
            else
            {
                if (_initialDestination.Type.IsValueType())
                    destValueExpr = Expression.Default(memberMap.DestinationType);
                else
                    destValueExpr = Expression.Condition(Expression.Equal(_initialDestination, Expression.Constant(null)),
                        Expression.Default(memberMap.DestinationType), getter);
            }

            var valueResolverExpr = BuildValueResolverFunc(memberMap, getter);
            var resolvedValue = Expression.Variable(valueResolverExpr.Type, "resolvedValue");
            var setResolvedValue = Expression.Assign(resolvedValue, valueResolverExpr);
            valueResolverExpr = resolvedValue;

            var typePair = new TypePair(valueResolverExpr.Type, memberMap.DestinationType);
            valueResolverExpr = memberMap.Inline
                ? ExpressionBuilder.MapExpression(_configurationProvider, _typeMap.Profile, typePair, valueResolverExpr, Context,
                    memberMap, destValueExpr)
                : ExpressionBuilder.ContextMap(typePair, valueResolverExpr, Context, destValueExpr, memberMap);

            valueResolverExpr = memberMap.ValueTransformers
                .Concat(_typeMap.ValueTransformers)
                .Concat(_typeMap.Profile.ValueTransformers)
                .Where(vt => vt.IsMatch(memberMap))
                .Aggregate(valueResolverExpr, (current, vtConfig) => ExpressionFactory.ToType(ExpressionFactory.ReplaceParameters(vtConfig.TransformerExpression, ExpressionFactory.ToType(current, vtConfig.ValueType)), memberMap.DestinationType));

            ParameterExpression propertyValue;
            Expression setPropertyValue;
            if (valueResolverExpr == resolvedValue)
            {
                propertyValue = resolvedValue;
                setPropertyValue = setResolvedValue;
            }
            else
            {
                propertyValue = Expression.Variable(valueResolverExpr.Type, "propertyValue");
                setPropertyValue = Expression.Assign(propertyValue, valueResolverExpr);
            }

            Expression mapperExpr;
            if (destinationMember is FieldInfo)
            {
                mapperExpr = memberMap.SourceType != memberMap.DestinationType
                    ? Expression.Assign(destMember, ExpressionFactory.ToType(propertyValue, memberMap.DestinationType))
                    : Expression.Assign(getter, propertyValue);
            }
            else
            {
                var setter = ((PropertyInfo)destinationMember).GetSetMethod(true);
                if (setter == null)
                    mapperExpr = propertyValue;
                else
                    mapperExpr = Expression.Assign(destMember, ExpressionFactory.ToType(propertyValue, memberMap.DestinationType));
            }
            var source = GetCustomSource(memberMap);
            if (memberMap.Condition != null)
                mapperExpr = Expression.IfThen(
                    memberMap.Condition.ConvertReplaceParameters(
                        source,
                        _destination,
                        ExpressionFactory.ToType(propertyValue, memberMap.Condition.Parameters[2].Type),
                        ExpressionFactory.ToType(getter, memberMap.Condition.Parameters[2].Type),
                        Context
                    ),
                    mapperExpr
                );

            mapperExpr = Expression.Block(new[] {setResolvedValue, setPropertyValue, mapperExpr}.Distinct());

            if (memberMap.PreCondition != null)
                mapperExpr = Expression.IfThen(
                    memberMap.PreCondition.ConvertReplaceParameters(
                        source,
                        _destination,
                        Context
                    ),
                    mapperExpr
                );

            return Expression.Block(new[] {resolvedValue, propertyValue}.Distinct(), mapperExpr);
        }

        private Expression BuildValueResolverFunc(IMemberMap memberMap, Expression destValueExpr)
        {
            Expression valueResolverFunc;
            var destinationPropertyType = memberMap.DestinationType;

            if (memberMap.ValueConverterConfig != null)
            {
                valueResolverFunc = ExpressionFactory.ToType(BuildConvertCall(memberMap), destinationPropertyType);
            }
            else if (memberMap.ValueResolverConfig != null)
            {
                valueResolverFunc = ExpressionFactory.ToType(BuildResolveCall(destValueExpr, memberMap), destinationPropertyType);
            }
            else if (memberMap.CustomMapFunction != null)
            {
                valueResolverFunc =
                    memberMap.CustomMapFunction.ConvertReplaceParameters(GetCustomSource(memberMap), _destination, destValueExpr, Context);
            }
            else if (memberMap.CustomMapExpression != null)
            {
                var nullCheckedExpression = memberMap.CustomMapExpression.ReplaceParameters(Source)
                    .NullCheck(destinationPropertyType);
                var destinationNullable = destinationPropertyType.IsNullableType();
                var returnType = destinationNullable && destinationPropertyType.GetTypeOfNullable() ==
                                 nullCheckedExpression.Type
                    ? destinationPropertyType
                    : nullCheckedExpression.Type;
                valueResolverFunc =
                    Expression.TryCatch(
                        ExpressionFactory.ToType(nullCheckedExpression, returnType),
                        Expression.Catch(typeof(NullReferenceException), Expression.Default(returnType)),
                        Expression.Catch(typeof(ArgumentNullException), Expression.Default(returnType))
                    );
            }
            else if(memberMap.SourceMembers.Any() && memberMap.SourceType != null)
            {
                var last = memberMap.SourceMembers.Last();
                if(last is PropertyInfo pi && pi.GetGetMethod(true) == null)
                {
                    valueResolverFunc = Expression.Default(last.GetMemberType());
                }
                else
                {
                    valueResolverFunc = Chain(memberMap.SourceMembers, destinationPropertyType);
                }
            }
            else
            {
                valueResolverFunc = Expression.Throw(Expression.Constant(new Exception("I done blowed up")));
            }

            if (memberMap.NullSubstitute != null)
            {
                var nullSubstitute = Expression.Constant(memberMap.NullSubstitute);
                valueResolverFunc = Expression.Coalesce(valueResolverFunc, ExpressionFactory.ToType(nullSubstitute, valueResolverFunc.Type));
            }
            else if (!memberMap.TypeMap.Profile.AllowNullDestinationValues)
            {
                var toCreate = memberMap.SourceType ?? destinationPropertyType;
                if (!toCreate.IsAbstract && toCreate.IsClass && !toCreate.IsArray)
                    valueResolverFunc = Expression.Coalesce(
                        valueResolverFunc,
                        ExpressionFactory.ToType(DelegateFactory.GenerateNonNullConstructorExpression(toCreate), memberMap.SourceType)
                    );
            }

            return valueResolverFunc;
        }

        private Expression GetCustomSource(IMemberMap memberMap) =>
            memberMap.CustomSource?.ConvertReplaceParameters(Source) ?? Source;

        private Expression Chain(IEnumerable<MemberInfo> members, Type destinationType) =>
                members.MemberAccesses(Source).NullCheck(destinationType);

        private Expression CreateInstance(Type type)
            => Expression.Call(Expression.Property(Context, nameof(ResolutionContext.Options)),
                nameof(IMappingOperationOptions.CreateInstance), new[] {type});

        private Expression BuildResolveCall(Expression destValueExpr, IMemberMap memberMap)
        {
            var valueResolverConfig = memberMap.ValueResolverConfig;
            var resolverInstance = valueResolverConfig.Instance != null
                ? Expression.Constant(valueResolverConfig.Instance)
                : CreateInstance(valueResolverConfig.ConcreteType);
            var source = GetCustomSource(memberMap);
            var sourceMember = valueResolverConfig.SourceMember?.ReplaceParameters(source) ??
                               (valueResolverConfig.SourceMemberName != null
                                   ? Expression.PropertyOrField(source, valueResolverConfig.SourceMemberName)
                                   : null);

            var iResolverType = valueResolverConfig.InterfaceType;
            var parameters = 
                new[] { source, _destination, sourceMember, destValueExpr }.Where(p => p != null)
                .Zip(iResolverType.GetGenericArguments(), ExpressionFactory.ToType)
                .Concat(new[] {Context});
            return Expression.Call(ExpressionFactory.ToType(resolverInstance, iResolverType), iResolverType.GetDeclaredMethod("Resolve"), parameters);
        }

        private Expression BuildConvertCall(IMemberMap memberMap)
        {
            var valueConverterConfig = memberMap.ValueConverterConfig;
            var iResolverType = valueConverterConfig.InterfaceType;
            var iResolverTypeArgs = iResolverType.GetGenericArguments();

            var resolverInstance = valueConverterConfig.Instance != null
                ? Expression.Constant(valueConverterConfig.Instance)
                : CreateInstance(valueConverterConfig.ConcreteType);
            var source = GetCustomSource(memberMap);
            var sourceMember = valueConverterConfig.SourceMember?.ReplaceParameters(source) ??
                               (valueConverterConfig.SourceMemberName != null
                                   ? Expression.PropertyOrField(source, valueConverterConfig.SourceMemberName)
                                   : memberMap.SourceMembers.Any()
                                       ? Chain(memberMap.SourceMembers, iResolverTypeArgs[1])
                                       : Expression.Block(
                                           Expression.Throw(Expression.Constant(BuildExceptionMessage())),
                                           Expression.Default(iResolverTypeArgs[0])
                                       )
                               );

            return Expression.Call(ExpressionFactory.ToType(resolverInstance, iResolverType), iResolverType.GetDeclaredMethod("Convert"),
                ExpressionFactory.ToType(sourceMember, iResolverTypeArgs[0]), Context);

            AutoMapperConfigurationException BuildExceptionMessage() 
                => new AutoMapperConfigurationException($"Cannot find a source member to pass to the value converter of type {valueConverterConfig.ConcreteType.FullName}. Configure a source member to map from.");
        }
    }
}