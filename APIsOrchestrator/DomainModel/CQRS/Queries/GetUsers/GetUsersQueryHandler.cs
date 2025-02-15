using System;
using CQRS.Queries;
using DomainModel.Services.User;
using System.Collections.Generic;
using DomainModel.Classes.User;
using System.Collections;
using System.Linq;

namespace DomainModel.CQRS.Queries.GetUsers
{
    public class GetUsersQueryHandler: IQueryHandler<GetUsersQuery,GetUsersQueryResult>
    {
        private readonly IGetUsers getUsers;

        public GetUsersQueryHandler(IGetUsers getUser)
        {
            this.getUsers = getUser ?? throw new ArgumentNullException(nameof(getUser));
        }

        public GetUsersQueryResult Handle(GetUsersQuery query)
        {
            IEnumerable<UserModel> users = this.getUsers.Get();

            return new GetUsersQueryResult(users);
        }
    }
}

