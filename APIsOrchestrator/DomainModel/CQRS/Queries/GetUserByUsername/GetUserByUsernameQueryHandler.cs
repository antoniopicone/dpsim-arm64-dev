using System;
using CQRS.Queries;
using DomainModel.Services.User;
using System.Collections.Generic;
using DomainModel.Classes.User;
using System.Collections;
using System.Linq;
using DomainModel.CQRS.Queries.GetUsers;

namespace DomainModel.CQRS.Queries.GetUserByUsername
{
    public class GetUserByUsernameQueryHandler: IQueryHandler<GetUserByUsernameQuery, GetUserByUsernameQueryResult>
    {
        private readonly IGetUsers getUsers;

        public GetUserByUsernameQueryHandler(IGetUsers getUser)
        {
            this.getUsers = getUser ?? throw new ArgumentNullException(nameof(getUser));
        }

        public GetUserByUsernameQueryResult Handle(GetUserByUsernameQuery query)
        {
            IEnumerable<UserModel> users = this.getUsers.getbyUsername(query.Username);

            return new GetUserByUsernameQueryResult(users);
        }
    }
}

