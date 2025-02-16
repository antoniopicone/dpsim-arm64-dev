using System;
//using System.Threading.Tasks;
using CQRS.Commands;
using CQRS.Queries;
using DomainModel.Classes.User;
using DomainModel.CQRS.Commands.User.AddUser;
using DomainModel.CQRS.Commands.User.DeleteUser;
using DomainModel.CQRS.Commands.User.UpdateUser;
using DomainModel.CQRS.Queries.AuthUsers;
using DomainModel.CQRS.Queries.GetUserByUsername;
using DomainModel.CQRS.Queries.GetUsers;
using DomainModel.Services.User;
using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
//using Serilog;
//using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace RockApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        // Commmand
        private readonly IAddUser userAdd;
        private readonly IDeleteUser deleteUser;
        private readonly IUpdateUser updateUser;

        // Command Handler 
        private readonly ICommandHandler<AddUserCommand> handlerAdd;
        private readonly ICommandHandler<DeleteUserCommand> handlerDelete;
        private readonly ICommandHandler<UpdateUserCommand> handlerUpdate;

        // Query Handler
        private readonly IQueryHandler<AuthUsersQuery, AuthUsersQueryResult> authenticateUser;
        private readonly IQueryHandler<GetUsersQuery,GetUsersQueryResult> getUsersHandler;
        private readonly IQueryHandler<GetUserByUsernameQuery, GetUserByUsernameQueryResult> queryUserByUsernameHandler;

        public UsersController(
            ICommandHandler<AddUserCommand> handlerAdd,
            ICommandHandler<DeleteUserCommand> handlerDelete,
            ICommandHandler<UpdateUserCommand> handlerUpdate,
            IAddUser userAdd,
            IDeleteUser deleteUser,
            IUpdateUser updateUser,
            IQueryHandler<GetUsersQuery,GetUsersQueryResult> getUsersHandler,
            IQueryHandler<AuthUsersQuery, AuthUsersQueryResult> authenticateUser,
            IQueryHandler<GetUserByUsernameQuery, GetUserByUsernameQueryResult> queryUserByUsername
            )
            
        {
            this.authenticateUser = authenticateUser ?? throw new  ArgumentNullException(nameof(authenticateUser));
            this.getUsersHandler = getUsersHandler ?? throw new ArgumentNullException(nameof(getUsersHandler));
            this.queryUserByUsernameHandler = queryUserByUsername ?? throw new ArgumentNullException(nameof(queryUserByUsername));

            this.handlerAdd = handlerAdd ?? throw new ArgumentNullException(nameof(handlerAdd));
            this.handlerDelete = handlerDelete ?? throw new ArgumentNullException(nameof(handlerDelete));
            this.handlerUpdate = handlerUpdate ?? throw new ArgumentNullException(nameof(handlerUpdate));

            this.userAdd = userAdd ?? throw new ArgumentNullException(nameof(userAdd));
            this.deleteUser = deleteUser ?? throw new ArgumentNullException(nameof(deleteUser));
            this.updateUser = updateUser ?? throw new ArgumentNullException(nameof(updateUser));
        }  

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Auth([FromBody] AuthUsersQuery authUsersQuery)
        //public AuthUsersQueryResult Auth([FromBody] AuthUsersQuery authUsersQuery)
        {
            var response = authenticateUser.Handle(authUsersQuery);
            return Ok(new Response<AuthUsersQueryResult>(response, 0, 1));
        }

        // POST: api/user
        // [AllowAnonymous]
        [HttpPost]
        // public void Post([FromBody] AddUserCommand command)
        public ActionResult<UserModel> Post([FromBody] AddUserCommand command)
        {
            handlerAdd.Handle(command);
            return CreatedAtAction(nameof(GetByUsername), new { username = command.Username }, command);
        }

        // PUT: api/user
        [HttpPut]
        public void Put([FromBody] UpdateUserCommand updateUserCommand)
        {
             handlerUpdate.Handle(updateUserCommand);
        }

        // PUT: api/user
        [HttpDelete]
        public void Delete([FromBody] DeleteUserCommand deleteUserCommand)
        {
            handlerDelete.Handle(deleteUserCommand);
        }

        // GET: api/users
        [HttpGet]
        public GetUsersQueryResult Get([FromQuery] GetUsersQuery getUsersQuery)
        {
            return this.getUsersHandler.Handle(getUsersQuery);
        }

        // GET: api/user/username
        [HttpGet("{username}")]
        //[Route("api/users/username")]
        public GetUserByUsernameQueryResult GetByUsername([FromRoute] GetUserByUsernameQuery getUserByUsernameQuery)
        //public GetUserByUsernameQueryResult GetByUsername([FromQuery] GetUserByUsernameQuery getUserByUsernameQuery)
        {
            return this.queryUserByUsernameHandler.Handle(getUserByUsernameQuery);
        }
    }
}