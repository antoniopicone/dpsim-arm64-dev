using System;
using DomainModel.CQRS.Commands.User.DeleteUser;

namespace DomainModel.Services.User
{
    public interface IDeleteUser
    {
        void Delete(DeleteUserCommand userCommand);
    }
}

