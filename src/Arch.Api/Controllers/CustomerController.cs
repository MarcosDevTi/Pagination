using Arch.Cqrs.Client.Command.Customer;
using Arch.Cqrs.Client.Command.Customer.Generics;
using Arch.Cqrs.Client.Paging;
using Arch.Cqrs.Client.Query.Customer.Models;
using Arch.Cqrs.Client.Query.Customer.Queries;
using Arch.Cqrs.Client.Query.Generics;
using Arch.Domain.Core.DomainNotifications;
using Arch.Domain.Event;
using Arch.Infra.Shared.Cqrs;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web.Http;
using System.Web.Http.Description;

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
        [HttpGet, Route("v1/public/list")]
        public HttpResponseMessage Index([FromUri]Paging<CustomerIndex> paging, string search = null)
        {

            HttpResponseMessage response;
            try
            {
                var result = _processor.Get(new GetCustomersIndex(paging, search));

                response = Request.CreateResponse(HttpStatusCode.OK, new { result.Items, result.HeadGrid, result.TotalNumberOfItems});
            }
            catch
            {
                response = Request.CreateResponse(HttpStatusCode.BadRequest, "Server Error");
            }

            return response;
        }

        [HttpGet, Route("v1/public/list/csv")]
        public HttpResponseMessage ListCsv([FromUri] GetObjectsCsv customersCsv)
        {

            HttpResponseMessage response;
            try
            {
                var result = _processor.Get(customersCsv);

                response = Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch(Exception e)
            {
                response = Request.CreateResponse(HttpStatusCode.BadRequest, "Server Error");
            }

            return response;
        }

        [HttpPost, Route("")]
        public HttpResponseMessage Post(CreateCustomer customer)
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
        [HttpPost, Route("update")]
        public HttpResponseMessage Edit(ListUpdate obj)
        {

            _processor.Send(obj);
            if (_notifications.HasNotifications())
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, _notifications.GetNotifications());
            }
            return Request.CreateResponse(HttpStatusCode.OK, obj);
        }


        public static object ReadToObject(string json, string typeP)
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