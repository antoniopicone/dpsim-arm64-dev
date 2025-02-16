using System;
using System.Text.Json.Serialization;
using CQRS.Queries;
using DomainModel.CQRS.Queries.GetUserByUsername;

namespace DomainModel.CQRS.Queries.GetUsers
{
    public class GetUserByUsernameQuery : IQuery<GetUserByUsernameQueryResult>
    {
        public string Username { get; set; }
    }
}
