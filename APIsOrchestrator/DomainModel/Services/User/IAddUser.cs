using System;
using DomainModel.CQRS.Commands.User.AddUser;

namespace DomainModel.Services.User
{
    public interface IAddUser
    {
        void Save(AddUserCommand user);
    }
}

