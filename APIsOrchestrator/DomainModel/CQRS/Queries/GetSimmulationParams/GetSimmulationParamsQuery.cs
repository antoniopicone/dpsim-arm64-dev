using System;
using System.Data.Common;
using System.Diagnostics.Contracts;
using CQRS.Queries;

namespace DomainModel.CQRS.Queries.GetSimmulationParams;

public class GetSimmulationParamsQuery: IQuery<GetSimmulationParamsQueryResult>
{
    public Guid id { get; set; }
    
}