using System.Runtime.Serialization;

namespace Arch.Cqrs.Client.Paging
{
    [DataContract]
    public enum SortDirection
    {
        [EnumMember]
        Ascending,

        [EnumMember]
        Descending
    }
}
