using System;
using DomainModel.CQRS.Commands.User.UpdateUser;

namespace DomainModel.Services.User
{
    public interface IUpdateUser
    {
        void Update(UpdateUserCommand userCommand);
    }
}

