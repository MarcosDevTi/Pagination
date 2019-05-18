using Arch.Infra.Shared.EventSourcing;

namespace Arch.Domain.Models
{
    public class CustomerMapSource: SourceFluent<Customer>
    {
        public override void Configuration(SourceFluent<Customer> builder)
        {
            builder.Ignore(_ => _.Email).Ignore(_=>_.BirthDate);
            builder.DisplayName(_ => _.BirthDate, "Date Naissance");
        }
    }
}