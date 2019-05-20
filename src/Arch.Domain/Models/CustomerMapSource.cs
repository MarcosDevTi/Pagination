using Arch.Infra.Shared.EventSourcing;

namespace Arch.Domain.Models
{
    public class CustomerMapSource : SourceFluent<Customer>
    {
        public override void Configuration(SourceFluent<Customer> builder)
        {
            //builder.Ignore(_ => _.Email);
            builder
                .Ignore(_ => _.BirthDate)
                .DisplayName(_ => _.FirstName, "Prenom")
                .DisplayName(_ => _.LastName, "Nom")
                .DisplayName(_ => _.Email, "Couriel");
            //    .DisplayName(_ => _.BirthDate, "Date Naissance")


        }
    }
}