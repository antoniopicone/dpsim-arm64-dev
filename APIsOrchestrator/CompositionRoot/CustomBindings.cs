using System;
using SimpleInjector;
using DomainModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Persistence.EF.DbContexts;
using Mqtt;

namespace CompositionRoot
{
    /// <summary>
    /// This class contains all the custom application bindings.
    /// </summary>
    internal static class CustomBindings
    {
        internal static void Bind(Container container)
        {
            // Put here the bindings of your own custom services

            container.Register<Persistence.EF.DbContexts.PgDataContext>(Lifestyle.Scoped);

            container.Register<Mqtt.ClientMosquitto>(Lifestyle.Scoped);

            container.Register<
                       DomainModel.Services.User.IAuthenticateUser,
                       Persistence.EF.PostgreSQL.Users.AuthenticateUser>(Lifestyle.Scoped);

                container.Register<
                  DomainModel.Services.User.IGetUsers,
                  Persistence.EF.PostgreSQL.Users.GetUsers>(Lifestyle.Scoped);

                container.Register<DomainModel.Services.User.IAddUser,
                    Persistence.EF.PostgreSQL.Users.AddUser>(Lifestyle.Scoped);

                container.Register<DomainModel.Services.User.IUpdateUser,
                    Persistence.EF.PostgreSQL.Users.UpdateUser>(Lifestyle.Scoped);

                container.Register<DomainModel.Services.User.IDeleteUser,
                    Persistence.EF.PostgreSQL.Users.DeleteUser>(Lifestyle.Scoped);
                 
        }
    }
}