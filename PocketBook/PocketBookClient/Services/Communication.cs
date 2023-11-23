using GrpcShared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocketBookClient.Services
{
    public  class Communication
    {

        private static Communication _instance;
        private AccountBook.AccountBookClient _accountBookClient;

        private Communication()
        {

        }

        public static Communication Instance
        {
            get
            {
                return _instance ?? (_instance = new Communication());
            }
        }

        public AccountBook.AccountBookClient AccountBookClient
        {
            set
            {
                   _accountBookClient = value;
            }

            private get { return _accountBookClient; }
        }

        public async Task<UserResponse> RegisterUserAsync(PocketBookClient.Data.User user)
        {
            var userRegisterRequest = new PocketUserInfo()
            {
                Username = user.Name,
                Password = user.Password,
                PhoneNumber =user.PhoneNumber,
                Email =user.Email,
            };

            try
            {

                return await _accountBookClient.RegisterUserAsync(userRegisterRequest);
            }
            catch(Exception ex)
            {

            }
            return null;
          }

        public async Task<UserResponse> UserLoginAsync(string userName,string password)
        {
            var userLoginRequest = new UserRequest()
            {
                Username = userName,
                Password = password,
            };

            try
            {

                return await _accountBookClient.LoginUserAsync(userLoginRequest);
            }
            catch (Exception ex)
            {

            }
            return null;
        }
    }
}
