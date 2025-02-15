using System;
using DomainModel.Services.User;
using DomainModel.Classes.User;
using Serilog;
using DomainModel.CQRS.Queries.AuthUsers;
using DomainModel.Classes;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using Persistence.EF.DbContexts;
using System.ComponentModel.DataAnnotations;

[assembly: InternalsVisibleTo("CompositionRoot")]
namespace Persistence.EF.PostgreSQL.Users
{
    internal class AuthenticateUser: IAuthenticateUser
    {
        private readonly PgDataContext dbContext;

        private readonly AppSettings _appSettings;

        public AuthenticateUser(IOptions<AppSettings> appSettings, PgDataContext DataContext)
        {
            this.dbContext = DataContext;
            _appSettings = appSettings.Value;
        }

        public IEnumerable<AuthUsersQuery> Authenticate(string username, string password)
        {
            AuthUsersQuery? authUsers = new AuthUsersQuery();

            List<AuthUsersQuery> users = new List<AuthUsersQuery>().ToList();
            

            try
            {
                UserModel user = dbContext.Users.AsNoTracking().Single(usr => usr.username == username.Trim().ToLower());

                // validate

                if (BCrypt.Net.BCrypt.Verify(password, user.password))
                {
                    authUsers.Id = user.id;
                    authUsers.FirstName = user.firstName;
                    authUsers.LastName = user.lastName;
                    authUsers.Username = user.username;
                    authUsers.Password = string.Empty;

                    // authentication successful so generate jwt token
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Name, user.id.ToString()) }),
                        Expires = DateTime.UtcNow.AddDays(1),
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                    };
                    var token = tokenHandler.CreateToken(tokenDescriptor);

                    // completiamo l'utenza autenticata con token e expiration date
                    authUsers.Token = tokenHandler.WriteToken(token);
                    authUsers.Expiration_date = DateTime.UtcNow.AddDays(30).ToString();
                    dbContext.Dispose();
                    users.Add(authUsers);
                } else {
                    Log.Debug("Errore Insert : {@classe} {@message}", this.ToString(), "password for User " + user.username + " is wrong !! ");
                }

            }
            catch(Exception Ex) {
                Log.Debug("Errore Insert : {@classe} {@message}", this.ToString(), Ex.Message);
                //throw;
            }
            return users;
        }
    }
}

