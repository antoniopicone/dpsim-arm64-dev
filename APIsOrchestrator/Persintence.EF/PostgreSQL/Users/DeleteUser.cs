using System;
using DomainModel.Classes.User;
using DomainModel.Services.User;
using Microsoft.EntityFrameworkCore;
using DomainModel.CQRS.Commands.User.DeleteUser;
using Serilog;
using Persistence.EF.DbContexts;

namespace Persistence.EF.PostgreSQL.Users
{
    internal class DeleteUser: IDeleteUser
    {
        private readonly PgDataContext dbContext;

        public DeleteUser(PgDataContext DataContext)
        {
            this.dbContext = DataContext;
        }

        public void Delete(DeleteUserCommand userCommand)
        {
            try
            {
                var userToDelete = dbContext.Users.FirstOrDefault(usr => usr.username == userCommand.Username);
                if (userToDelete != null)
                {
                    dbContext.Users.Remove(userToDelete);
                    dbContext.SaveChanges();
                    dbContext.Dispose();
                }
            }
            catch (Exception Ex)
            {
                Log.Debug("Errore Insert : {@classe} {@message}", this.ToString(), Ex.Message);
                throw new NotImplementedException("Errore Durante la Delete", Ex);
            }
        }
    }
}

