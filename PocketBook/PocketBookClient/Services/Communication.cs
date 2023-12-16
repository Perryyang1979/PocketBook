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

        public async Task<MessageResponse> RegisterUserAsync(PocketBookClient.Data.User user)
        {
            var userRegisterRequest = new PocketUserInfo()
            {
                Username = user.Name,
                Password = user.Password,
                PhoneNumber =user.PhoneNumber,
                Email =user.Email,
                GroupId = user.GroupId,
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

        public async Task<UserLoginResponse> UserLoginAsync(string userName,string password)
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


        public AsyncServerStreamingCall<PocketTransactionInfo> GetDailyTransationsInCurrentMonthAsync()
        {
            var currentDay = DateTime.Today.Day;

            var startDate = DateTime.Today.AddDays(1-currentDay);
            var endDate = DateTime.Today.AddDays(31-currentDay);
            var dailyDetailTransationRequest = new DailyDetailTransationRequest()
            {
                StartDate = startDate.ToShortDateString(),
                EndDate = endDate.ToShortDateString(),
            };

            try
            {

                return _accountBookClient.GetDailyTransationsStream(dailyDetailTransationRequest);
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        public async Task<MessageResponse> AddTransactionAsync(UserTransaction userTransaction)
        {
            var transactionRequest = new PocketTransactionInfo()
            {
                TransactionDate = userTransaction.Date,
                Amount = (double)userTransaction.Amount,
                Category = userTransaction.Category,
                Comments = userTransaction.Comments,
                IsIncome = userTransaction.IsIncome,
                Username = Settings.Current.CurrentUserSetting.UserName
            };

            MessageResponse response = null;

            try
            {

               response =  await _accountBookClient.AddTransactionAsync(transactionRequest);
            }
            catch (Exception ex)
            {
                return await Task.FromResult(new MessageResponse { Success = false, Message = ex.Message });
            }

            return response;
        }

        public async Task<MessageResponse> AddCategoryAsync(TransactionCategory category)
        {
            var categoryRequest = new CategoryInfo()
            {

                IsIncome = category.IsIncome,
                Description = category.CategoryDescription,
                Detail = category.CategoryDetailedDescription
            };

            MessageResponse response = null;

            try
            {

                response = await _accountBookClient.AddCategoryAsync(categoryRequest);
            }
            catch (Exception ex)
            {
                return await Task.FromResult(new MessageResponse { Success = false, Message = ex.Message });
            }

            return response;
        }


        public async Task<MessageResponse> DeleteCategoryAsync(TransactionCategory category)
        {
            var categoryRequest = new CategoryInfo()
            {

                IsIncome = category.IsIncome,
                Description = category.CategoryDescription,
                Detail = category.CategoryDetailedDescription
            };

            MessageResponse response = null;

            try
            {

                response = await _accountBookClient.DeleteCategoryAsync(categoryRequest);
            }
            catch (Exception ex)
            {
                return await Task.FromResult(new MessageResponse { Success = false, Message = ex.Message });
            }

            return response;
        }


        public AsyncServerStreamingCall<CategoryInfo> GetAllCategoryStream()
        {
            try
            {

                return _accountBookClient.GetAllCategoryStream(new GetCategoryRequest());
            }
            catch (Exception ex)
            {

            }

            return null;
        }

        public async Task<MessageResponse> AddIdentificationAsync(TransactionIdentification identification)
        {
            var request = new IdentificationInfo()
            {
                Description = identification.IdentifyDescription,
            };

            MessageResponse response = null;

            try
            {

                response = await _accountBookClient.AddIdentificationAsync(request);
            }
            catch (Exception ex)
            {
                return await Task.FromResult(new MessageResponse { Success = false, Message = ex.Message });
            }

            return response;
        }


        public async Task<MessageResponse> DeleteIdentificationAsync(TransactionIdentification identification)
        {
            var request = new IdentificationInfo()
            {
                Description = identification.IdentifyDescription,
            };

            MessageResponse response = null;

            try
            {

                response = await _accountBookClient.DeleteIdentificationAsync(request);
            }
            catch (Exception ex)
            {
                return await Task.FromResult(new MessageResponse { Success = false, Message = ex.Message });
            }

            return response;
        }


        public AsyncServerStreamingCall<IdentificationInfo> GetAllIdentificationStream()
        {
            try
            {

                return _accountBookClient.GetAllIdentificationStream(new GetIdentificationRequest());
            }
            catch (Exception ex)
            {

            }

            return null;
        }

        public AsyncServerStreamingCall<DailySummary> GetMonthlyReport()
        {
            try
            {

                return _accountBookClient.GetDailySummaryStream(new UserRequest()
                {
                    Username = Settings.Current.CurrentUserSetting.UserName
                }); 
            }
            catch (Exception ex)
            {

            }

            return null;
        }

        public AsyncServerStreamingCall<CategorySummary> GetMonthlyCategoryReport()
        {
            try
            {

                return _accountBookClient.GetMonthlyCategorySummaryStream(new UserRequest()
                {
                    Username = Settings.Current.CurrentUserSetting.UserName
                });
            }
            catch (Exception ex)
            {

            }

            return null;
        }
    }
}
