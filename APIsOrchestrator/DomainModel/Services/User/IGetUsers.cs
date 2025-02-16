using DomainModel.Classes.User;
using DomainModel.CQRS.Queries.GetUsers;
using System;
using System.Collections.Generic;
using System.Text;


namespace DomainModel.Services.User
{
    public interface IGetUsers
    {
        IEnumerable<UserModel> Get();

        IEnumerable<UserModel> getbyUsername(string username);
       
    }
}
