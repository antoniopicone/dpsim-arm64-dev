using DomainModel.CQRS.Queries.AuthUsers;
using System;
using System.Collections.Generic;
using System.Text;

namespace DomainModel.Services.User
{
    public interface IAuthenticateUser
    {
      IEnumerable<AuthUsersQuery> Authenticate(string username, string password);

      //IEnumerable<AuthUsersQuery> Authenticate(AuthUsersQuery);

    }
}
