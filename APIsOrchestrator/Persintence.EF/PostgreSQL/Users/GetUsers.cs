using System;
using System.Data;
using DomainModel.CQRS.Commands.User.AddUser;
using DomainModel.CQRS.Queries.GetUsers;
using DomainModel.Services.User;
using Microsoft.EntityFrameworkCore;
using Persistence.EF.DbContexts;
using DomainModel.Classes.User;
using Serilog;
using System.Linq;

namespace Persistence.EF.PostgreSQL.Users
{
    internal class GetUsers : IGetUsers
    {
        private readonly PgDataContext dbContext;

        public GetUsers(PgDataContext DataContext)
        {
            this.dbContext = DataContext;
        }

        public IEnumerable<UserModel> Get()
        {
            List<UserModel> users = new List<UserModel>().ToList();

            using (var context = dbContext)
            {
               users = context.Users.AsNoTracking().ToList();
            }

            return users;
          
        }

        public IEnumerable<UserModel> getbyUsername(string usenameToFind)
        {

            List<UserModel> users = new List<UserModel>().ToList();

            try
            {
                using (var context = dbContext)
                {
                    UserModel user = context.Users.AsNoTracking().Single(usr => usr.username == usenameToFind);
                    
                    users.Add(user);
                }
            }
            catch { }

            return users;
        }
    }
}
