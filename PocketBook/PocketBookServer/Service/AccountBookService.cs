using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PocketBookServer.Data;
using PocketBookServer.Models;
using System.Globalization;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PocketBookServer.Services
{
    public class AccountBookService: AccountBook.AccountBookBase
    {
        private static readonly Dictionary<string, List<PocketTransaction>> UserTransactions = new Dictionary<string, List<PocketTransaction>>();
        private readonly AppDbContext _dbContext;

        public AccountBookService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        private bool IsUserExist(string userName)
        {
            return _dbContext.Users.Any(u => u.Name.Equals(userName));
        }

        private string UserId(string userName) => _dbContext?.Users.SingleOrDefault(u => u.Name.Equals(userName)).Id;
        private string UserName(string userId) => _dbContext?.Users.SingleOrDefault(u => u.Id.Equals(userId)).Name;

        public override async Task<UserResponse> RegisterUser(PocketUserInfo request, ServerCallContext context)
        {
            // use sign up logic 

            if(string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
            {
                return await Task.FromResult(new UserResponse { Success = false, Message = "User registered Failed with empty failed." });
            }

            //if find same name ,will return username are exist 

           if(IsUserExist(request.Username))
            {
                return await Task.FromResult(new UserResponse { Success = false, Message = "UserName have been registered already!" });
            }

            var pocketUser = new PocketUser()
            {
                Id = Guid.NewGuid().ToString(),
                Name = request.Username,
                Email = request.Email,
                Password = request.Password,
                PhoneNumber = request.PhoneNumber
            };


            await _dbContext.AddAsync(pocketUser);
            await _dbContext.SaveChangesAsync();
            return await Task.FromResult(new UserResponse { Success = true, Message = "User registered successfully." });
        }

        public override async Task<UserResponse> LoginUser(UserRequest request, ServerCallContext context)
        {
            // implement User Login 

            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
            {
                return await Task.FromResult(new UserResponse { Success = false, Message = "User registered Failed with empty failed." });
            }

            if (!_dbContext.Users.Any(u => u.Name.Equals(request.Username) 
            && u.Password.Equals(request.Password)))
            {
                return await Task.FromResult(new UserResponse { Success = false, Message = "UserName or password is wrong!" });
            }
            
            
            return await Task.FromResult(new UserResponse { Success = true, Message = "User logged in successfully." });
        }

        public override async Task<TransactionResponse> AddTransaction(PocketTransactionInfo request, ServerCallContext context)
        {
            // if not find user, return fail message
            if (!IsUserExist(request.Username))
            {
                return await Task.FromResult(new TransactionResponse { Success = false, Message = $"User {request.Username} not found." });
            }
            //need to do 
            if(request.Amount < 0 )
            {
                return await Task.FromResult(new TransactionResponse { Success = false, Message = $"Money should not less than 0." });
            }

            //need to do duble compare
            if (request.Amount == 0)
            {
                return await Task.FromResult(new TransactionResponse { Success = false, Message = $"No need to record since money is 0." });
            }

            var transaction = new PocketTransaction
           {
                    Id = Guid.NewGuid().ToString(),
                    UserId = UserId(request.Username),
                    Amount = request.Amount,
                    IsIncome = request.IsIncome,
                    Category = request.Category,
                    Date = request.TransactionDate,
                    Comments = request.Comments
                // Date = DateTime.Now.ToShortDateString()
            };

            await _dbContext.AddAsync(transaction);
            await _dbContext.SaveChangesAsync();

            return await Task.FromResult(new TransactionResponse { Success = true, Message = "Transaction added successfully." });
            }

        public override async Task<DailyTransactionResponse> GetDailyTransations(DailyDetailTransationRequest request, ServerCallContext context)
        {
            var startDate = DateTime.Parse(request.StartDate, CultureInfo.InvariantCulture);
            var endDate = DateTime.Parse(request.EndDate, CultureInfo.InvariantCulture);
            if(startDate > endDate)
            {
                var temp = startDate;
                startDate = endDate;
                endDate = temp;
            }

            var transactions = await _dbContext.PocketTransaction.Where(pt => DateTime.Parse(pt.Date, CultureInfo.InvariantCulture) >=startDate && DateTime.Parse(pt.Date, CultureInfo.InvariantCulture) <= endDate).ToListAsync();

            if (!transactions.Any())
            {
                return await Task.FromResult(new DailyTransactionResponse());
            }

            List<PocketTransactionInfo>? pocketTransactions = transactions
            .OrderBy(t => t.Date)
            .Select(transaction => new PocketTransactionInfo
            {
                    Username = UserName(transaction.UserId),
                    Amount = transaction.Amount,
                    IsIncome = transaction.IsIncome,
                    Category = transaction.Category,
                    Comments = transaction.Comments,
                    TransactionDate = transaction.Date,
             })
             .ToList();

            return await Task.FromResult(new DailyTransactionResponse { DailyTransactions = { pocketTransactions } });
        }

        //write steam when number of date is over 20 or more.
        public override async Task GetDailyTransationsStream(DailyDetailTransationRequest request, IServerStreamWriter<PocketTransactionInfo> responseStream, ServerCallContext context)
        {
            var startDate = DateTime.Parse(request.StartDate, CultureInfo.InvariantCulture);
            var endDate = DateTime.Parse(request.EndDate, CultureInfo.InvariantCulture);
            if (startDate > endDate)
            {
                var temp = startDate;
                startDate = endDate;
                endDate = temp;
            }

            var transactions = await _dbContext.PocketTransaction.Where(pt => string.Compare(pt.Date, startDate.ToShortDateString())>=0 && string.Compare(pt.Date ,endDate.ToString())<=0).ToListAsync();

            if (transactions.Any())
            {
                List<PocketTransactionInfo>? pocketTransactions = transactions
                .OrderBy(t => t.Date)
                .Select(transaction => new PocketTransactionInfo
                {
                    Username = UserName(transaction.UserId),
                    Amount = transaction.Amount,
                    IsIncome = transaction.IsIncome,
                    Category = transaction.Category,
                    Comments = transaction.Comments,
                    TransactionDate = transaction.Date,
                })
                 .ToList();

                foreach (var  transaction in pocketTransactions)
                {
                    await responseStream.WriteAsync(transaction);
                }
            }
        }


        public override async Task<DailySummaryResponse> GetDailySummary(UserRequest request, ServerCallContext context)
        {
            // if not find user, or userName is null empty return fail message
            if (!IsUserExist(request.Username) || string.IsNullOrEmpty(request.Username))
            {
                return await Task.FromResult(new DailySummaryResponse());

            }

            var transactions =await  _dbContext.PocketTransaction.Where(pt=>pt.UserId ==UserId(request.Username)).ToListAsync();

            if(!transactions.Any())
            {
                return await Task.FromResult(new DailySummaryResponse());
            }
                
            var dailySummaries = transactions
                    .GroupBy(t => t.Date)
                    .Select(group => new DailySummary
                    {
                        Date = group.Key,
                        TotalIncome = group.Where(t => t.IsIncome).Sum(t => t.Amount),
                        TotalExpense = group.Where(t => !t.IsIncome).Sum(t => t.Amount)
                    })
                    .ToList();

           return await Task.FromResult(new DailySummaryResponse { DailySummaries = { dailySummaries } });
        }

        public override async Task<CategorySummaryResponse> GetCategorySummary(UserRequest request, ServerCallContext context)
        {
            // if not find user, or userName is null empty return fail message
            if (!IsUserExist(request.Username) || string.IsNullOrEmpty(request.Username))
            {
                return await Task.FromResult(new CategorySummaryResponse());

            }

            var transactions =await  _dbContext.PocketTransaction.Where(pt => pt.UserId == UserId(request.Username)).ToListAsync();

            var categorySummaries =  transactions
                    .GroupBy(t => t.Category)
                    .Select(group => new CategorySummary
                    {
                        Category = group.Key,
                        TotalAmount = group.Sum(t => t.Amount)
                    })
                    .ToList();

             return await Task.FromResult(new CategorySummaryResponse { CategorySummaries = { categorySummaries } });

        }
    }
}
