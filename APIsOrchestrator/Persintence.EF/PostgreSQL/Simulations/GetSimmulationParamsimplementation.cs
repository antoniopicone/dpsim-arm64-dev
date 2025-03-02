using System;
using System.Collections.Generic;
using System.Linq;
using DomainModel.Classes;
using DomainModel.Classes.Simulations;
using DomainModel.CQRS.Queries.GetSimmulationParams;
using DomainModel.Services;
using DomainModel.Services.Simulations;
using Persistence.EF.DbContexts;

namespace Persistence.EF.PostgreSQL.Simulations;

public class GetSimmulationParamsimplementation: IGetSimmulationParams
{
    private readonly PgDataContext dbContext;

    public GetSimmulationParamsimplementation(PgDataContext pgDataContext)
    {
        this.dbContext = pgDataContext;
    }

    public IEnumerable<Simulation> GetSimmulationParams(GetSimmulationParamsQuery query)
    {
        List<Simulation> simulations = [];
        
        Simulation simulation = new Simulation {
            id = query.id,
            endpoint_source = new UdpEndpointSim {
                host = "0.0.0.0",
                port = 12000
            },
            endpoint_dest = new UdpEndpointSim {
                host = "villas_lab_a",
                port = 12001
            },
            time_step = "1",
            time_period_excecution = "10",
            frequency_band = 50
        };
        
        simulations.Add(simulation);

        return simulations;
    }
}
