using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Arch.Infra.Shared.Cqrs.Event
{
    //public abstract class ClasseBase: ISaveSource<ClasseBase>
    //{
    //    public ClasseBase()
    //    {
    //        Lista = new List<string>();
    //    }
    //    public List<string> Lista { get; set; }
    //    public void Add(string item) => Lista.Add(item);
    //    public abstract void Configure();
    //}
    //public class CreateTeste: ClasseBase
    //{
    //    private ISourceWrite<ClasseBase> _builder;
    //    public CreateTeste(ISourceWrite<ClasseBase> builder)
    //    {
    //        _builder = builder;
    //    }
    //    public string Name { get; set; }
    //    public string Email { get; set; }
    //    public string PropIgnore { get; set; }
    //    public string PropIgnore2 { get; set; }
    //    public override void Configure()
    //    {
            
    //        _builder.Ignore(x => ((CreateTeste)x).PropIgnore)
    //            .Ignore(x => ((CreateTeste)x).PropIgnore2);
    //    }
    //}

   

    //public interface ISaveSource<ClasseBase> where ClasseBase : class
    //{
    //    void Configure();
    //}

    //public interface ISourceWrite<ClasseBase> where ClasseBase : class
    //{
    //    SourceWrite<ClasseBase> Ignore<TProperty>(
    //        Expression<Func<ClasseBase, TProperty>> propertyExpression);
    //}

    //public class SourceWrite<ClasseBase> : ISourceWrite<ClasseBase>
    //    where ClasseBase : class
    //{
    //    private readonly ISourceList _sourceList;

    //    public SourceWrite(ISourceList sourceList)
    //    {
    //        _sourceList = sourceList;
    //    }

    //    public SourceWrite<ClasseBase> Ignore<TProperty>(Expression<Func<ClasseBase, TProperty>> propertyExpression)
    //    {
    //        _sourceList.Add((propertyExpression.Name, ""));
    //        return this;
    //    }
    //}

    //public interface ISourceList
    //{
    //    void Add((string, string) value);
    //    (string, string) Get();
    //}

    //public class SourceList : ISourceList
    //{
    //    public void Add((string, string) value)
    //    {
            
    //    }

    //    public (string, string) Get()
    //    {
    //        return ("", "");
    //    }
    //}


}
