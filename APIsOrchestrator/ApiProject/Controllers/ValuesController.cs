using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Mqtt;
using Microsoft.Extensions.Configuration;

namespace RockApi.Controllers
{
    public class Simulation
    {
        public float vdc { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly ClientMosquitto clientMosquitto;

        private readonly IConfiguration configuration;

        public ValuesController(
            ClientMosquitto clientMosquitto,
            IConfiguration configuration)
        {
            this.clientMosquitto = clientMosquitto ?? throw new ArgumentNullException(nameof(clientMosquitto));
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        Simulation _simulation = new Simulation { vdc = 400 };

        // GET api/values/simulation
        [HttpGet("simulation")]
        public async Task<ActionResult<Simulation>> GetSim()
        {
            //await ClientMosquitto.PublishMessageAsync(configuration,System.Guid.NewGuid().ToString());
            return _simulation;
        }

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
