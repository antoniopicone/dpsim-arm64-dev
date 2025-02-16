using System;
using DomainModel.Classes.User;
using System.Collections.Generic;

namespace DomainModel.CQRS.Queries.GetUserByUsername
{
    public class GetUserByUsernameQueryResult
    {
        public GetUserByUsernameQueryResult(IEnumerable<UserModel> users)
        {
            Users = users ?? throw new ArgumentNullException(nameof(users));
        }

        public IEnumerable<UserModel> Users { get; }
    }
}

