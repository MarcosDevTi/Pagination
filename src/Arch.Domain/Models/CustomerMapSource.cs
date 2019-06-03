using Arch.Infra.Shared.EventSourcing;

namespace Arch.Domain.Models
{
    public class CustomerMapSource : SourceFluent<Customer>
    {
        public override void Configuration(SourceFluent<Customer> builder)
        {
            //builder.Ignore(_ => _.EmailAddress);
            builder
                //.Ignore(_ => _.BirthDate)
                .DisplayName(_ => _.FirstName, "Prenom")
                .DisplayName(_ => _.LastName, "Nom")
                .DisplayName(_ => _.EmailAddress, "Couriel");
            //    .DisplayName(_ => _.BirthDate, "Date Naissance")


        }
    }
}