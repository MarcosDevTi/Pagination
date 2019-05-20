using System;
using Arch.Domain.Core;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace Arch.Cqrs.Handlers
{
    public static class DbContextExtensions
    {
        public static string Add(this DbContext context, Entity entity)
        {
            context.Set(entity.GetType()).Add(entity);
            return $"New {entity.GetType().Name}";
        }

        public static string Update(this DbContext context, Entity entity)
        {
            var entry = context.Entry(entity);
            entry.State = EntityState.Modified;
            return $"Updated {entity.GetType().Name}";
        }

        public static string Delete(this DbContext context, Entity entity)
        {
            context.Set(entity.GetType()).Remove(entity);
            return $"Deleted {entity.GetType().Name}";
        }
    }
}
