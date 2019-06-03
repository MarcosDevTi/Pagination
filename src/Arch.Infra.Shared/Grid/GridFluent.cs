using Arch.Infra.Shared.EventSourcing;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Arch.Infra.Shared.Grid
{
    public abstract class GridFluent<T> : GridTypeBuilder<T> where T : class
    {
        public List<(string member, bool editable, string displayName)> MembersDisplayName;
        private void AddMember(MemberInfo member, bool editable, string displayName)
        {
            if (MembersDisplayName == null) MembersDisplayName = new List<(string member, bool editable, string displayName)>();
            MembersDisplayName.Add((member.Name, editable, displayName));
        }

        public GridFluent<T> AddGridMember<TProperty>(Expression<Func<T, TProperty>> property, bool editable, string displayName = null)
        {
            AddMember(property.GetMember(), editable, displayName);
            return this;
        }

        //public GridFluent<T> AddMemberInGrid<TProperty>(Expression<Func<T, TProperty>> property)
        //{
        //    AddMember(property.GetMember(), displayName);
        //    return this;
        //}
        public abstract void Configuration(GridFluent<T> builder);
    }
}
