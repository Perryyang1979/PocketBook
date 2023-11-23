using GrpcShared;
using Microsoft.AspNetCore.Identity;

namespace PocketBookServer.Services
{
    public class UserService : User.UserBase
    {
        UsersManager _usersManager;

        public UserService(UsersManager usersManager)
        {
            _usersManager = usersManager;
        }

        public override Task<UserReply> GetAllUser(GetAllUserRequest request, ServerCallContext context)
        {
            var reply = new UserReply();
            reply.User.AddRange(_usersManager.UserPerson);
            return Task.FromResult(reply);
        }

        public override async Task GetUsersStream(GetAllUserRequest request, IServerStreamWriter<UserPerson> responseStream, ServerCallContext context)
        {
            var users = _usersManager.UserPerson;

            foreach (UserPerson user in users)
            {
                await responseStream.WriteAsync(user);
            }
        }

        public override Task<UserPerson> GetUserById(GetUserRequest request, ServerCallContext context)
        {
            var result = (from x in _usersManager.UserPerson where x.UserId == request.UserId select x).FirstOrDefault();

            return Task.FromResult(result);
        }

        public override Task<UserPerson> GetUserByName(GetUserViaName request, ServerCallContext context)
        {
            var result = (from x in _usersManager.UserPerson where x.UserName == request.UserName select x).FirstOrDefault();

            return Task.FromResult(result);
        }
    }
}
