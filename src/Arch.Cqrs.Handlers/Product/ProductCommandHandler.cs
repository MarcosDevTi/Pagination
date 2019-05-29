using Arch.Cqrs.Client.Command.Product;
using Arch.Cqrs.Client.Event.Product;
using Arch.Domain.Core.DomainNotifications;
using Arch.Domain.Event;
using Arch.Infra.Data;
using Arch.Infra.Shared.Cqrs.Command;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Arch.Cqrs.Handlers.Product
{
    public class ProductCommandHandler : CommandHandler<Domain.Models.Product>,
        ICommandHandler<CreateProduct>,
        ICommandHandler<UpdateProduct>,
        ICommandHandler<DeleteProduct>
    {
        private readonly ArchDbContext _architectureContext;
        public ProductCommandHandler(
            ArchDbContext architectureContext,
            IDomainNotification notifications,
            IEventRepository eventRepository,
            EventSourcingContext eventSourcingContext) : base(architectureContext, notifications, eventRepository, eventSourcingContext)
        {
            _architectureContext = architectureContext;
        }

        public void Handle(CreateProduct command)
        {
            AddListBase();
            ValidateCommand(command);

            var customer = Mapper.Map<Domain.Models.Product>(command);


            if (Db().Any(x => x.Name == customer.Name))
            {
                AddNotification(new DomainNotification(
                    command.Action, "The Product Name has already been taken."));
                return;
            }

            Db().Add(customer);

            Commit(Mapper.Map<ProductCreated>(command));
        }

        public void Handle(UpdateProduct command)
        {
            ValidateCommand(command);

            var product = Mapper.Map<Domain.Models.Product>(command);

            var getProducts = Db().Where(x => x.Name == command.Name);

            //check already been taken name from other product and send notification
            //_architectureContext.Set<Domain.Models.Product>().Update(product);
            Commit(Mapper.Map<ProductUpdated>(command));
        }

        public void Handle(DeleteProduct command)
        {
            ValidateCommand(command);
            Db().Remove(GetById(command.Id));
            Commit(new ProductDeleted(command.Id));
        }

        private Domain.Models.Product GetById(Guid id) => Db().Find(id);

        public void AddListBase()
        {
            var listBase = new List<Domain.Models.Product>
            {
                new Domain.Models.Product("aaProduct Nam","Description Product", 12.14M),
                new Domain.Models.Product("aProduct Nam","Description Product", 12.14M),
                new Domain.Models.Product("ssProduct Nam","Description Product", 12.14M),
                new Domain.Models.Product("sProduct Nam","Description Product", 12.14M),
                new Domain.Models.Product("ddProduct Nam","Description Product", 12.14M),
                new Domain.Models.Product("dProduct Nam","Description Product", 12.14M),
                new Domain.Models.Product("ffProduct Nam","Description Product", 12.14M),
                new Domain.Models.Product("fProduct Nam","Description Product", 12.14M),
                new Domain.Models.Product("ggProduct Nam","Description Product", 12.14M),
                new Domain.Models.Product("gProduct Nam","Description Product", 12.14M),
                new Domain.Models.Product("hhProduct Nam","Description Product", 12.14M),
                new Domain.Models.Product("hProduct Nam","Description Product", 12.14M),
                new Domain.Models.Product("jjProduct Nam","Description Product", 12.14M),
                new Domain.Models.Product("jProduct Nam","Description Product", 12.14M),
                new Domain.Models.Product("kkProduct Nam","Description Product", 12.14M),
                new Domain.Models.Product("lProduct Nam","Description Product", 12.14M),
                new Domain.Models.Product("qqProduct Nam","Description Product", 12.14M),
                new Domain.Models.Product("qProduct Nam","Description Product", 12.14M),
                new Domain.Models.Product("wwProduct Nam","Description Product", 12.14M),
                new Domain.Models.Product("wProduct Nam","Description Product", 12.14M),
                new Domain.Models.Product("eeProduct Nam","Description Product", 12.14M),
                new Domain.Models.Product("eProduct Nam","Description Product", 12.14M),
                new Domain.Models.Product("rrProduct Nam","Description Product", 12.14M),
                new Domain.Models.Product("rProduct Nam","Description Product", 12.14M),
                new Domain.Models.Product("ttProduct Nam","Description Product", 12.14M),
                new Domain.Models.Product("tProduct Nam","Description Product", 12.14M),
                new Domain.Models.Product("yyProduct Nam","Description Product", 12.14M),
                new Domain.Models.Product("yProduct Nam","Description Product", 12.14M),
                new Domain.Models.Product("uuProduct Nam","Description Product", 12.14M),
                new Domain.Models.Product("uProduct Nam","Description Product", 12.14M),
                new Domain.Models.Product("iiProduct Nam","Description Product", 12.14M),
                new Domain.Models.Product("iProduct Nam","Description Product", 12.14M),
                new Domain.Models.Product("ooProduct Nam","Description Product", 12.14M),
                new Domain.Models.Product("oProduct Nam","Description Product", 12.14M),
                new Domain.Models.Product("ppProduct Nam","Description Product", 12.14M),
                new Domain.Models.Product("pProduct Nam","Description Product", 12.14M),
                new Domain.Models.Product("zzProduct Nam","Description Product", 12.14M),
                new Domain.Models.Product("zProduct Nam","Description Product", 12.14M),
                new Domain.Models.Product("xxProduct Nam","Description Product", 12.14M),
                new Domain.Models.Product("xProduct Nam","Description Product", 12.14M),
                new Domain.Models.Product("ccProduct Nam","Description Product", 12.14M),
                new Domain.Models.Product("ProductCommandHandler.csProduct Nam","Description Product", 12.14M),
                new Domain.Models.Product("vProduct Nam","Description Product", 12.14M),
                new Domain.Models.Product("bProduct Nam","Description Product", 12.14M),
                new Domain.Models.Product("nProduct Nam","Description Product", 12.14M),
                new Domain.Models.Product("mProduct Nam","Description Product", 12.14M),
                new Domain.Models.Product("mmProduct Nam","Description Product", 12.14M),
                new Domain.Models.Product("nnProduct Nam","Description Product", 12.14M),
                new Domain.Models.Product("bbProduct Nam","Description Product", 12.14M),

            };

            foreach (var product in listBase)
            {
                _architectureContext.Products.Add(product);
            }

            _architectureContext.SaveChanges();
        }


    }
}
