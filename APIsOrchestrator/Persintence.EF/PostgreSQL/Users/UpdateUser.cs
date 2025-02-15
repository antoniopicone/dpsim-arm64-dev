using System;
using Persistence.EF.DbContexts;
using DomainModel.Classes.User;
using DomainModel.Services.User;
using Microsoft.EntityFrameworkCore;
using DomainModel.CQRS.Commands.User.UpdateUser;
using Serilog;

namespace Persistence.EF.PostgreSQL.Users
{
    internal class UpdateUser: IUpdateUser
    {
        private readonly PgDataContext dbContext;

        public UpdateUser(PgDataContext DataContext)
        {
            this.dbContext = DataContext;
        }

        public void Update(UpdateUserCommand userToBeSaved)
        {
                try
                {
                    UserModel? userInDB = dbContext.Users.Single(usr => usr.username == userToBeSaved.Username);

                    if (BCrypt.Net.BCrypt.Verify(userToBeSaved.old_Password, userInDB.password))
                    {
                        string passwordHash = BCrypt.Net.BCrypt.HashPassword(userToBeSaved.Password);

                      //    dbContext.Entry(userInDB).State = EntityState.Modified;
                            userInDB.password = passwordHash;
                            userInDB.firstName = userToBeSaved.FirstName;
                            userInDB.lastName = userToBeSaved.LastName;  
                            userInDB.timestamp = System.DateTime.Now.ToString();
                            userInDB.active = userToBeSaved.Active;
                            dbContext.SaveChanges();           
                    }
                }
                catch (Exception Ex)
                {
                    Log.Debug("Errore Insert : {@classe} {@message}", this.ToString(), Ex.Message);
                    //throw new NotImplementedException("Errore Durante Aggiornamento", Ex);
                }

            dbContext.Dispose();
        }

    }
}

