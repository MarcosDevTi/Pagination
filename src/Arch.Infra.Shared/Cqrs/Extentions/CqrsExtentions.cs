using System;
using System.Linq;
using System.Reflection;
using Arch.Infra.Shared.Cqrs.Command;
using Arch.Infra.Shared.Cqrs.Query;
using SimpleInjector;

namespace Arch.Infra.Shared.Cqrs.Extentions
{
    public static class CqrsExtentions
    {
        public static void AddCqrs<T>(this Container container, Func<AssemblyName, bool> filter = null)
        {
            var handlers = new[] { typeof(IQueryHandler<,>), typeof(ICommandHandler<>) };
            var target = typeof(T).Assembly;
            bool FilterTrue(AssemblyName x) => true;

            var assemblies = target.GetReferencedAssemblies()
                .Where(filter ?? FilterTrue)
                .Select(Assembly.Load)
                .ToList();
            assemblies.Add(target);

            var types = from t in assemblies.SelectMany(a => a.GetExportedTypes())
                from i in t.GetInterfaces()
                where i.IsConstructedGenericType &&
                      handlers.Contains(i.GetGenericTypeDefinition())
                select new { i, t };

            foreach (var tp in types)
                container.Register(tp.i, tp.t, Lifestyle.Transient);
        }
    }
}
