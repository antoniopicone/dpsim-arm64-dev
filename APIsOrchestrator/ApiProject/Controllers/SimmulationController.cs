using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DomainModel.Services.Simulations;
using DomainModel.CQRS.Queries.GetSimmulationParams;
using CQRS.Queries;
using System;


namespace ApiProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SimmulationController : ControllerBase
    {
        public readonly IQueryHandler<GetSimmulationParamsQuery, GetSimmulationParamsQueryResult> queryHandler;

        public SimmulationController(IQueryHandler<GetSimmulationParamsQuery, GetSimmulationParamsQueryResult> queryHandler)
        {
            this.queryHandler = queryHandler ?? throw new  ArgumentNullException(nameof(queryHandler));
        }

        // GET: api/Simmulation
        [HttpGet]
        public GetSimmulationParamsQueryResult Get([FromQuery] GetSimmulationParamsQuery getSimmulationParamsQuery)
        {
            return this.queryHandler.Handle(getSimmulationParamsQuery);
        }
    }
}
