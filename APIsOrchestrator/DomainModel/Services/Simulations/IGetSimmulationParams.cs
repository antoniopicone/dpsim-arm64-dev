using System;
using System.Collections.Generic;
using DomainModel.Classes.Simulations;
using DomainModel.CQRS.Queries.GetSimmulationParams;

namespace DomainModel.Services.Simulations;

public interface IGetSimmulationParams
{
    IEnumerable<Simulation> GetSimmulationParams(GetSimmulationParamsQuery query);
}
