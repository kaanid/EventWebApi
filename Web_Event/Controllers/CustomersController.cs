using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Web_Event.E;
using Web_Event.Models;
using Weeb.Event;

namespace Web_Event.Controllers
{
    [Route("api/[controller]")]
    public class CustomerController : Controller
    {
        private readonly ILogger<ValuesController> _log;
        private readonly IConfiguration _configuration;
        private readonly IEventBus _eventBus;
        private readonly string connectionString;

        private static IList<Customer> list = new List<Customer>();

        public CustomerController(ILogger<ValuesController> logger,IConfiguration configuration, IEventBus eventBus)
        {
            _log = logger;
            _configuration = configuration;
            _eventBus = eventBus;

            connectionString = _configuration.GetSection("Mysql").GetSection("connectionString").Value;
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public IActionResult Get(long id)
        {
            var model = list.FirstOrDefault(m => m.Id == id);
            if (model==null)
            {
                return NotFound();
            }
            return Ok(model);
        }

        // POST api/values
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]dynamic model)
        {
            var name = (string)model.Name;
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest();
            }

            var customer = new Customer {
                Id = DateTime.Now.Ticks,
                Name = name
            };

            list.Add(customer);

            await _eventBus.PublishAsync(new CustomerCreatedEvent(name));

            return Created(Url.Action("Get", new { id = customer.Id }),customer.Id);
        }
    }
}
