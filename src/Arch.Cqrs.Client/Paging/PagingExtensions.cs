using Arch.Infra.Shared.Grid;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Arch.Cqrs.Client.Paging
{
    public static class PagingExtensions
    {

        private static readonly List<Type> Collections = new List<Type> { typeof(IEnumerable<>), typeof(IEnumerable) };

        private static Paging<TOut> Conversor<TIn, TOut>(Paging<TIn> entrada)
        {
            return new Paging<TOut>
            {
                SortColumn = entrada.SortColumn,
                Top = entrada.Top,
                Skip = entrada.Skip,
                SortDirection = entrada.SortDirection
            };
        }

        public static PagedResult<T2> GetPagedResult<T, T2>(this IQueryable<T> dbSet, Paging<T2> paging)
        {
            var count = dbSet.Count();

            var pagingT2 = Conversor<T2, T>(paging);

            var result = new PagedResult<T2>(dbSet.SortAndPage2<T, T2>(pagingT2), count, Conversor<T, T2>(pagingT2));

            result.HeadGrid = GetHeadGridAndParameterSort<T, T2>();
            return result;
        }

        public static IQueryable<T2> SortAndPage2<T, T2>(this IQueryable<T> dbSet, Paging<T> paging)
        {
            if (paging == null)
            {
                return dbSet.ProjectTo<T2>();
            }

            if (string.IsNullOrEmpty(paging.SortColumn))
            {
                paging.SortColumn = typeof(T)
                    .GetProperties()
                    .First(p => p.PropertyType == typeof(string)
                                || !p.PropertyType.GetInterfaces()
                                    .Any(i => Collections.Any(c => i == c)))
                    .Name;
            }

            var parameter = Expression.Parameter(typeof(T), "p");
            var command = paging.SortDirection == SortDirection.Descending ? "OrderByDescending" : "OrderBy";

            var parts = paging.SortColumn.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            var propp = GetSourceAndDest<T, T2>().FirstOrDefault(_ => _.dest.Name == parts[0]).src;
            var property = typeof(T).GetProperty(propp.Name);



            var member = Expression.MakeMemberAccess(parameter, property);
            for (var i = 1; i < parts.Length; i++)
            {
                property = property.PropertyType.GetProperty(parts[i]);
                member = Expression.MakeMemberAccess(member, property);
            }

            var orderByExpression = Expression.Lambda(member, parameter);

            var resultExpression = Expression.Call(
                typeof(Queryable),
                command,
                new[] { typeof(T), property.PropertyType },
                dbSet.Expression,
                Expression.Quote(orderByExpression));

            dbSet = dbSet.Provider.CreateQuery<T>(resultExpression);

            return dbSet.Skip(paging.Skip).Take(paging.Top).ProjectTo<T2>();
        }

        private static IEnumerable<HeadGridProp> GetHeadGridAndParameterSort<T, T2>()
        {
            var listDisplay = GridOutils.GetHeadGenericGrid(typeof(T2));
            
            var head = GetSourceAndDest<T, T2>().Select(_ =>
            {
                var view = _.dest.Name;
                var getDisplay = listDisplay.FirstOrDefault(d => d.memberName == view);

                string display;
                if(listDisplay.Any(d => d.memberName == view) && getDisplay.displayName != null)
                {
                    display = getDisplay.displayName;
                }
                else
                {
                    display = view;
                }

                return new HeadGridProp
                {
                    ViewPropCamelCase = ConvertToCamelCase(view),
                    ViewProp = view,
                    ModelProp = ((dynamic)_.src).Name,
                    DisplayProp = display,
                    AssemblyViewModel = typeof(T2).FullName,
                    AssemblyModel = typeof(T).FullName,
                    Editable = getDisplay.editable,
                    Displayable = listDisplay.Any(__ => __.memberName == view),
                    Type = _.src.GetMethod.ReturnType.Name
                };
            }).ToList();
            var result = OrderHeadGrid(head, listDisplay.Select(_ => _.memberName));
            return result;
        }

        private static IEnumerable<HeadGridProp> OrderHeadGrid(List<HeadGridProp> listHead, IEnumerable<string> propsOrdered)
        {
            var listToOrder = new List<HeadGridProp>();
            foreach(var item in propsOrdered)
            {
                var itemAdd = listHead.First(_ => _.ViewProp == item);
                listToOrder.Add(itemAdd);
            }
            listHead.ForEach(_ =>
            {
                if (!listToOrder.Contains(_)) listToOrder.Add(_);
            });
            return listToOrder;
        }

        private static string ConvertToCamelCase(string name)
        {
            return char.ToLowerInvariant(name[0]) + name.Substring(1);
        }

        private static IEnumerable<(PropertyInfo dest, PropertyInfo src)> GetSourceAndDest<T, T2>()
        {
            var typePair = new TypePair(typeof(T), typeof(T2));
            var target = Mapper.Configuration.GetMapperFunc<T, T2>(typePair).Target;
            var props = (target as Closure)?.Constants.Where(_ => _.GetType() == typeof(PropertyMap));

            return props?.Select(_ =>
                (
                    (PropertyInfo)((dynamic)_).DestinationProperty,
                    (PropertyInfo)((dynamic)_).SourceMember
                )
            ).ToList();
        }
    }


}
