using Grpc.Core;
using GrpcShared;
using PocketBookClient.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

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

        public  async Task<DailyTransactionResponse> GetDailyTransations(DateTime startDate,DateTime endDate)
        {
            var dailyDetailTransationRequest = new DailyDetailTransationRequest()
            {
                StartDate = startDate.ToShortDateString(),
                EndDate = endDate.ToShortDateString(),
            };

            try
            {

                return  _accountBookClient.GetDailyTransations(dailyDetailTransationRequest);
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        public  AsyncServerStreamingCall<PocketTransactionInfo> GetDailyTransationsStreamAsync(DateTime startDate, DateTime endDate)
        {
            var dailyDetailTransationRequest = new DailyDetailTransationRequest()
            {
                StartDate = startDate.ToShortDateString(),
                EndDate = endDate.ToShortDateString(),
            };

            try
            {

                return  _accountBookClient.GetDailyTransationsStream(dailyDetailTransationRequest);
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        public async Task<TransactionResponse> AddTransaction(UserTransaction userTransaction)
        {
            var transactionRequest = new PocketTransactionInfo()
            {
                TransactionDate = userTransaction.Date,
                Amount = (double)userTransaction.Amount,
                Category = userTransaction.Category,
                Comments = userTransaction.Comments,
                IsIncome = userTransaction.IsIncome,
                Username = Settings.Current.CurrentUserName
            };

            try
            {

                await _accountBookClient.AddTransactionAsync(transactionRequest);
            }
            catch (Exception ex)
            {

            }
            return null;
        }
    }
}
