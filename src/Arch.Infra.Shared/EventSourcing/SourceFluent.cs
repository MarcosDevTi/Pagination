using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Arch.Infra.Shared.EventSourcing
{
    public abstract class SourceFluent<T>: SourceTypeBuilder<T> where T: class
    {
        public List<MemberInfo> Members;
        public Dictionary<MemberInfo, string> MembersDisplayName;
        public void AddMember(MemberInfo member)
        {
            if (Members == null) Members = new List<MemberInfo>();
                Members.Add(member);
        }

        public void AddMember(MemberInfo member, string displayName)
        {
            if (MembersDisplayName == null) MembersDisplayName = new Dictionary<MemberInfo, string>();
            MembersDisplayName.Add(member, displayName);
        }

        public SourceFluent<T> DisplayName<TProperty>(Expression<Func<T, TProperty>> property, string displayName)
        {
            AddMember(property.GetMember(), displayName);
            return this;
        }

        public SourceFluent<T> Ignore<TProperty>(params Expression<Func<T, TProperty>>[] expression)
        {
            foreach (var exp in expression)
                AddMember(exp.GetMember());
            return this;
        }

        public abstract void Configuration(SourceFluent<T> builder);
    }


}
