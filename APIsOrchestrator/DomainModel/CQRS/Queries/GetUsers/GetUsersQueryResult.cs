using System;
using System.Collections.Generic;
using DomainModel.Classes.User;

namespace DomainModel.CQRS.Queries.GetUsers
{
    public class GetUsersQueryResult
    {
        public GetUsersQueryResult(IEnumerable<UserModel> users)
        {
            Users = users ?? throw new ArgumentNullException(nameof(users));
        }

        public IEnumerable<UserModel> Users { get; }
    }
}

