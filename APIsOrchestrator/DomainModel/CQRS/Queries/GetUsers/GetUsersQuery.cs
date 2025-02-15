using System;
using System.Text.Json.Serialization;
using CQRS.Queries;

namespace DomainModel.CQRS.Queries.GetUsers
{
    public class GetUsersQuery: IQuery<GetUsersQueryResult>
    {
      //  public int Id { get; set; }
      //  public string FirstName { get; set; }
      //  public string LastName { get; set; }
        public string Username { get; set; }
      //  public string Password { get; set; }
      //  public bool Active { get; set; }
    }
}

