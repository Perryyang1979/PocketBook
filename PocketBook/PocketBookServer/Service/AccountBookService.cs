using Microsoft.EntityFrameworkCore;
using PocketBookServer.Data;
using PocketBookServer.Models;

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

        public override async Task<TransactionResponse> AddTransaction(TransactionRequest request, ServerCallContext context)
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
                    Date = request.TransactionDate
                   // Date = DateTime.Now.ToShortDateString()
            };

            await _dbContext.AddAsync(transaction);
            await _dbContext.SaveChangesAsync();

            return await Task.FromResult(new TransactionResponse { Success = true, Message = "Transaction added successfully." });
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
