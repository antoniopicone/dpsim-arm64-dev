using CQRS.Commands;
using DomainModel.Services.User;
using System;
using System.Collections.Generic;
using System.Text;


namespace DomainModel.CQRS.Commands.User.DeleteUser
{
    public class DeleteUserCommandHandler: ICommandHandler<DeleteUserCommand>
    {
        private readonly IDeleteUser deleteUser;

        public DeleteUserCommandHandler(IDeleteUser deleteUser)
        {
            this.deleteUser = deleteUser ?? throw new ArgumentNullException(nameof(deleteUser));
        }

        public void Handle(DeleteUserCommand command)
        {
            var user = new DeleteUserCommand();
            user.Username = command.Username;
            this.deleteUser.Delete(user);
        }
    }
}

