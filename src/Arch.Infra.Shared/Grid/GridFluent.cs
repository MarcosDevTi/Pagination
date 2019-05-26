using Arch.Infra.Shared.EventSourcing;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Arch.Infra.Shared.Grid
{
    public abstract class GridFluent<T> : GridTypeBuilder<T> where T : class
    {
        public Dictionary<MemberInfo, string> MembersDisplayName;
        private void AddMember(MemberInfo member, string displayName)
        {
            if (MembersDisplayName == null) MembersDisplayName = new Dictionary<MemberInfo, string>();
            MembersDisplayName.Add(member, displayName);
        }

        public GridFluent<T> DisplayName<TProperty>(Expression<Func<T, TProperty>> property, string displayName)
        {
            AddMember(property.GetMember(), displayName);
            return this;
        }
        public abstract void Configuration(GridFluent<T> builder);
    }
}
