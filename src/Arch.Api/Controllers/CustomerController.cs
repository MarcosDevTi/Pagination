using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Arch.Cqrs.Client.Command.Customer;
using Arch.Cqrs.Client.Event.Customer;
using Arch.Cqrs.Client.Paging;
using Arch.Cqrs.Client.Query.Customer.Models;
using Arch.Cqrs.Client.Query.Customer.Queries;
using Arch.Domain.Core;
using Arch.Domain.Core.DomainNotifications;
using Arch.Domain.Event;
using Arch.Infra.Shared.Cqrs;
using Arch.Infra.Shared.Cqrs.Event;
using AutoMapper;

namespace Arch.Api.Controllers
{
    [RoutePrefix("api/customers")]
    public class CustomerController : BaseController
    {
        private readonly IDomainNotification _notifications;
        private readonly IProcessor _processor;
        private readonly IEventRepository _eventRepository;

        public CustomerController(
            IDomainNotification notifications,
            IProcessor processor, IEventRepository eventRepository) : base(notifications)
        {
            _notifications = notifications;
            _processor = processor;
            _eventRepository = eventRepository;
        }

        [HttpGet, Route("v1/public/history"), ResponseType(typeof(CustomerDetails))]
        public HttpResponseMessage GetHistory([FromUri]GetCustomerHistory getHistory)
        {
            var result = _processor.Get(getHistory);
            
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }
        //http://localhost:50005/api/customers?Skip=5&Top=15&SortColumn=Name&SortDirection=Descending&search=Teste
        [HttpGet, Route("v1/public/customers"), ResponseType(typeof(PagedResult<CustomerIndex>))]
        public HttpResponseMessage Index([FromUri]Paging<CustomerIndex> paging, string search = null)
        {
            HttpResponseMessage response;
            try
            {
                var result = _processor.Get(new GetCustomersIndex(paging, search));
                response = Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch
            {
                response = Request.CreateResponse(HttpStatusCode.BadRequest, "Server Error");
            }

            return response;
        }

        [HttpPost, Route("")]
        public HttpResponseMessage Post([FromBody]CreateCustomer customer)
        {
            _processor.Send(customer);

            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
            }
            if (_notifications.HasNotifications())
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, _notifications.GetNotifications());
            }
            return Request.CreateResponse(HttpStatusCode.Created);
        }
        [HttpPut, Route("")]
        public HttpResponseMessage Edit(UpdateCustomer customer)
        {
            if (!ModelState.IsValid) return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState);


            if (customer == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            _processor.Send(customer);
            if (_notifications.HasNotifications())
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, _notifications.GetNotifications());
            }
            return Request.CreateResponse(HttpStatusCode.OK, customer);
        }


        public static object ReadToObject(string json,string typeP)
        {
            Assembly asm = typeof(CreateCustomer).Assembly;
            Type type = asm.GetType(typeP);

            var ms = new MemoryStream(Encoding.UTF8.GetBytes(json));
            DataContractJsonSerializer ser = new DataContractJsonSerializer(type);
            var res = ser.ReadObject(ms) as object;
            ms.Close();

            Convert.ChangeType(res, type);
            return res;
        }

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