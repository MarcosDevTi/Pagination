using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Arch.Cqrs.Client.Command.Customer;
using Arch.Cqrs.Client.Paging;
using Arch.Cqrs.Client.Query.Customer.Models;
using Arch.Cqrs.Client.Query.Customer.Queries;
using Arch.Domain.Core.DomainNotifications;
using Arch.Domain.Event;
using Arch.Infra.Shared.Cqrs;

namespace Arch.Api.Controllers
{
    [RoutePrefix("api/customers")]
    public class CustomerController : BaseController
    {
        private readonly IProcessor _processor;
        private readonly IEventRepository _eventRepository;

        public CustomerController(
            IDomainNotification notifications,
            IProcessor processor, IEventRepository eventRepository) : base(notifications)
        {
            _processor = processor;
            _eventRepository = eventRepository;
        }

        [HttpGet, Route("")]
        public IHttpActionResult Index([FromUri]Paging<CustomerIndex> paging, string search = null)
        {

            return Ok(_processor.Get(new GetCustomersIndex(paging, search)));

            return Ok(5);
        }

        [HttpPost, Route("")]
        public HttpResponseMessage Post([FromBody]CreateCustomer customer)
        {
            _processor.Send(customer);

            if (!ModelState.IsValid)

                return Request.CreateResponse(HttpStatusCode.BadRequest);
            return Request.CreateResponse(HttpStatusCode.Created);
        }
        //[HttpPut]
        //public HttpResponseMessage Edit(Guid? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var customer = _processor.Process(new GetCustomerDetails(id.Value));

        //    if (customer == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(customer);
        //}

        //public IActionResult Details(Guid? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var customer = _processor.Process(new GetCustomerDetails(id.Value));

        //    if (customer == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(customer);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult Edit(CustomerDetails customer)
        //{
        //    if (!ModelState.IsValid) return View(customer);

        //    _processor.Send(Mapper.Map<UpdateCustomer>(customer));

        //    if (IsValidOperation())
        //        ViewBag.Sucesso = "Customer Updated!";

        //    return View(customer);
        //}

        //[HttpGet]
        //public IActionResult Delete(Guid? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var customer = _processor.Process(new GetCustomerDetails(id.Value));

        //    return customer == null ? (IActionResult)NotFound() : View(customer);
        //}

        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public IActionResult DeleteConfirmed(Guid id)
        //{
        //    _processor.Send(new DeleteCustomer(id));

        //    if (!IsValidOperation())
        //        return View(_processor.Process(new GetCustomerDetails(id)));

        //    ViewBag.Sucesso = "Customer Removed!";
        //    return RedirectToAction("Index");
        //}
        [HttpGet, Route("History")]
        public IHttpActionResult History()
        {
            return Ok(_eventRepository.GetAllHistories());
        }
    }
}