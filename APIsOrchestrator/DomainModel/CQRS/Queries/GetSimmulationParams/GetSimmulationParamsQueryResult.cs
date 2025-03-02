using System;
using System.Collections.Generic;
using DomainModel.Classes.Simulations;

namespace DomainModel.CQRS.Queries.GetSimmulationParams;

public class GetSimmulationParamsQueryResult
{
    public GetSimmulationParamsQueryResult(IEnumerable<Simulation> simulations) {
        Simulations = simulations ?? throw new ArgumentNullException(nameof(simulations));
    }
    public IEnumerable<Simulation> Simulations { get;}
}
