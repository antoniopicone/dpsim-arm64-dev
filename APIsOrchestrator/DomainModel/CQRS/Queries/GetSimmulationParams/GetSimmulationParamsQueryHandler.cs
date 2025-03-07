using System;
using CQRS.Queries;
using DomainModel.Services.Simulations;

namespace DomainModel.CQRS.Queries.GetSimmulationParams;

public class GetSimmulationParamsQueryHandler: IQueryHandler<GetSimmulationParamsQuery, GetSimmulationParamsQueryResult>
{
    private readonly IGetSimmulationParams getSimmulationParams;

    public GetSimmulationParamsQueryHandler(IGetSimmulationParams getSimmulationParameters)
    {
        this.getSimmulationParams = getSimmulationParameters ?? throw new ArgumentNullException(nameof(getSimmulationParameters));
    }

    public GetSimmulationParamsQueryResult Handle(GetSimmulationParamsQuery getSimmulationParamsQuery)
    {
        return new GetSimmulationParamsQueryResult(getSimmulationParams.GetSimmulationParams(getSimmulationParamsQuery));
    }
}
