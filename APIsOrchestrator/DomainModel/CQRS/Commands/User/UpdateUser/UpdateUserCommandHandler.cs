using System;
using DomainModel.Services;
using DomainModel.Services.User;
using System.Collections.Generic;
using System.Text;
using CQRS.Commands;
using DomainModel.CQRS.Commands.User.UpdateUser;

namespace DomainModel.CQRS.Commands.User.UpdateUser
{
    public class UpdateUserCommandHandler: ICommandHandler<UpdateUserCommand>
    {
        private readonly IUpdateUser updateUser;

        public UpdateUserCommandHandler(IUpdateUser updateUser)
        {
            this.updateUser = updateUser ?? throw new ArgumentNullException(nameof(updateUser));
        }

        public void Handle(UpdateUserCommand command)
        {
            var user = new UpdateUserCommand();

            user.Username = command.Username;
            user.Password = command.Password;
            user.Id = command.Id;
            user.FirstName = command.FirstName;
            user.LastName = command.LastName;
            user.Active = command.Active;
            user.old_Password = command.old_Password;
            this.updateUser.Update(user);
        }
    }
}

