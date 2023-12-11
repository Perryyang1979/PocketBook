using GrpcShared;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PocketBookServer.Data;
using PocketBookServer.Models;
using System.Globalization;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;
using RSAService = PocketBookClient.Helper.RSAService;

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

        public override async Task<MessageResponse> RegisterUser(PocketUserInfo request, ServerCallContext context)
        {
            // use sign up logic 

            if(string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
            {
                return await Task.FromResult(new MessageResponse { Success = false, Message = "User registered Failed with empty failed." });
            }

            //if find same name ,will return username are exist 

           if(IsUserExist(request.Username))
            {
                return await Task.FromResult(new MessageResponse { Success = false, Message = "UserName have been registered already!" });
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
            return await Task.FromResult(new MessageResponse { Success = true, Message = "User registered successfully." });
        }

        public override async Task<MessageResponse> LoginUser(UserRequest request, ServerCallContext context)
        {
            // implement User Login 

            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
            {
                return await Task.FromResult(new MessageResponse { Success = false, Message = "User registered Failed with empty failed." });
            }

            // use function need emunalble first
            if (!_dbContext.Users.AsEnumerable().Any(u => u.Name.Equals(request.Username) 
            && RSAService.DecryptData(u.Password).Equals(RSAService.DecryptData(request.Password))))
            {
                return await Task.FromResult(new MessageResponse { Success = false, Message = "UserName or password is wrong!" });
            }
            
            
            return await Task.FromResult(new MessageResponse { Success = true, Message = "User logged in successfully." });
        }

        public override async Task<MessageResponse> AddTransaction(PocketTransactionInfo request, ServerCallContext context)
        {
            // if not find user, return fail message
            if (!IsUserExist(request.Username))
            {
                return await Task.FromResult(new MessageResponse { Success = false, Message = $"User {request.Username} not found." });
            }
            //need to do 
            if(request.Amount < 0 )
            {
                return await Task.FromResult(new MessageResponse { Success = false, Message = $"Money should not less than 0." });
            }

            //need to do duble compare
            if (request.Amount == 0)
            {
                return await Task.FromResult(new MessageResponse { Success = false, Message = $"No need to record since money is 0." });
            }

            var transaction = new PocketTransaction
           {
                    Id = Guid.NewGuid().ToString(),
                    UserId = UserId(request.Username),
                    Amount = request.Amount,
                    IsIncome = request.IsIncome,
                    Category = request.Category,
                    Date = request.TransactionDate,
                    Comments = request.Comments,
                    Identification = request.Identification,    
            };

            await _dbContext.AddAsync(transaction);
            await _dbContext.SaveChangesAsync();

            return await Task.FromResult(new MessageResponse { Success = true, Message = "Transaction added successfully." });
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

            var transactions = await _dbContext.PocketTransactions.Where(pt => DateTime.Parse(pt.Date, CultureInfo.InvariantCulture) >=startDate && DateTime.Parse(pt.Date, CultureInfo.InvariantCulture) <= endDate).ToListAsync();

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
                    Identification = transaction.Identification
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

            var transactions =  _dbContext.PocketTransactions.ToList().Where(pt => DateTime.Compare(DateTime.Parse(pt.Date, CultureInfo.InvariantCulture),startDate)>=0 
            && DateTime.Compare(DateTime.Parse(pt.Date, CultureInfo.InvariantCulture),endDate)<=0).ToList();

            if (transactions.Any())
            {
                List<PocketTransactionInfo>? pocketTransactions = transactions
                .OrderBy(t => DateTime.Parse(t.Date, CultureInfo.InvariantCulture))
                .Select(transaction => new PocketTransactionInfo
                {
                    Username = UserName(transaction.UserId),
                    Amount = transaction.Amount,
                    IsIncome = transaction.IsIncome,
                    Category = transaction.Category,
                    Comments = transaction.Comments,
                    TransactionDate = transaction.Date,
                    Identification = transaction.Identification
                })
                 .ToList();

                foreach (PocketTransactionInfo  transaction in pocketTransactions)
                {
                    await responseStream.WriteAsync(transaction);
                }
            }
        }

        #region category
        public override async Task<MessageResponse> AddCategory(CategoryInfo request, ServerCallContext context)
        {
            // check required filed
            if (string.IsNullOrEmpty(request.Description))
            {
                return await Task.FromResult(new MessageResponse { Success = false, Message = $"Category need description." });
            }

            //check current description is exist 
            if (_dbContext.Categories.Any(u => u.Description.Equals(request.Description) && u.IsIncome== request.IsIncome))
            {
                return await Task.FromResult(new MessageResponse { Success = false, Message = $"Category {request.Description} have been added!" });
            }

            var category = new Category
            {
                Id = Guid.NewGuid().ToString(),
                IsIncome = request.IsIncome,
                Description = request.Description,
                DetailedInfo = request.Detail
            };

            await _dbContext.AddAsync(category);
            await _dbContext.SaveChangesAsync();

            return await Task.FromResult(new MessageResponse { Success = true, Message = "Category added successfully." });
        }

        public override async Task<MessageResponse> DeleteCategory(CategoryInfo request, ServerCallContext context)
        {
            // check required filed
            if (string.IsNullOrEmpty(request.Description))
            {
                return await Task.FromResult(new MessageResponse { Success = false, Message = $"Category need description." });
            }

            var category =  _dbContext.Categories.SingleOrDefault(u => u.Description.Equals(request.Description) && u.IsIncome == request.IsIncome);
            //check current description is exist 
            if (category == null)
            {
                return await Task.FromResult(new MessageResponse { Success = false, Message = $"no Category {request.Description} exist in the system!" });
            }

            _dbContext.Categories.Remove(category);
            await _dbContext.SaveChangesAsync();

            return await Task.FromResult(new MessageResponse { Success = true, Message = "Category delete successfully." });
        }

        public override async Task GetAllCategoryStream(GetCategoryRequest request, IServerStreamWriter<CategoryInfo> responseStream, ServerCallContext context)
        {

            var categories =  _dbContext.Categories;

            if (categories.Any())
            {
                List<CategoryInfo> allCategoris = categories
                .OrderBy(c => c.IsIncome)
                .Select(ct => new CategoryInfo
                {
                    Description = ct.Description, 
                    IsIncome = ct.IsIncome,
                    Detail = ct.DetailedInfo,   
                })
                 .ToList();

                foreach (var category in allCategoris)
                {

                   await responseStream.WriteAsync(category);
                    
                }
            }
        }

        #endregion

        #region Identification
        public override async Task<MessageResponse> AddIdentification(IdentificationInfo request, ServerCallContext context)
        {
            // check required filed
            if (string.IsNullOrEmpty(request.Description))
            {
                return await Task.FromResult(new MessageResponse { Success = false, Message = $"Identification need description." });
            }

            //check current description is exist 
            if (_dbContext.Identifications.Any(u => u.Name.Equals(request.Description)))
            {
                return await Task.FromResult(new MessageResponse { Success = false, Message = $"Identification {request.Description} have been added!" });
            }

            var identify = new Identification
            {
                Id = Guid.NewGuid().ToString(),
                Name = request.Description,
            };

            await _dbContext.AddAsync(identify);
            await _dbContext.SaveChangesAsync();

            return await Task.FromResult(new MessageResponse { Success = true, Message = $"Identification {identify.Name} added successfully." });
        }

        public override async Task<MessageResponse> DeleteIdentification(IdentificationInfo request, ServerCallContext context)
        {
            // check required filed
            if (string.IsNullOrEmpty(request.Description))
            {
                return await Task.FromResult(new MessageResponse { Success = false, Message = $"Identification need description." });
            }

            var Identy = _dbContext.Identifications.SingleOrDefault(u => u.Name.Equals(request.Description) );
            //check current description is exist 
            if (Identy == null)
            {
                return await Task.FromResult(new MessageResponse { Success = false, Message = $"no Identification {request.Description} exist in the system!" });
            }

            _dbContext.Identifications.Remove(Identy);
            await _dbContext.SaveChangesAsync();

            return await Task.FromResult(new MessageResponse { Success = true, Message = $"Identification {Identy.Name} deleted successfully." });
        }

        public override async Task GetAllIdentificationStream(GetIdentificationRequest request, IServerStreamWriter<IdentificationInfo> responseStream, ServerCallContext context)
        {

            var identifies = _dbContext.Identifications;

            if (identifies.Any())
            {
                List<IdentificationInfo> identificationInfos = identifies
                .Select(ct => new IdentificationInfo
                {
                    Description = ct.Name,
                })
                 .ToList();

                foreach (var iden in identificationInfos)
                {

                    await responseStream.WriteAsync(iden);

                }
            }
        }

        #endregion


        public override async Task GetDailySummaryStream(UserRequest request, IServerStreamWriter<DailySummary> responseStream, ServerCallContext contextServerCallContext)
        {
            // if not find user, or userName is null empty return fail message
            if (!IsUserExist(request.Username) || string.IsNullOrEmpty(request.Username))
            {
                 await Task.FromResult(new DailySummaryResponse());

            }

            var currentYear = DateTime.Now.Year;

            var transactions =  _dbContext.PocketTransactions.ToList().Where(pt=>pt.UserId ==UserId(request.Username) 
            && Convert.ToDateTime(pt.Date).Year == currentYear
           ).ToList();

            if(!transactions.Any())
            {
                 await Task.FromResult(new DailySummaryResponse());
            }
                
            var dailySummaries = transactions
                    .GroupBy(t => new
                    {
                        DateTime.Parse(t.Date, CultureInfo.InvariantCulture).Date.Year,
                        DateTime.Parse(t.Date, CultureInfo.InvariantCulture).Date.Month
                    })
                    .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month) // optional
                    .Select(group => new DailySummary
                    {
                        Date = $"{ group.Key.Year} - {group.Key.Month}",
                        TotalIncome = group.Where(t => t.IsIncome).Sum(t => t.Amount),
                        TotalExpense = group.Where(t => !t.IsIncome).Sum(t => t.Amount)
                    })
                    .ToList();

            foreach (DailySummary ds in dailySummaries)
            {
                await responseStream.WriteAsync(ds);
            }
        }

        public override async Task GetMonthlyCategorySummaryStream(UserRequest request, IServerStreamWriter<CategorySummary> responseStream, ServerCallContext contextServerCallContext)
        {
            // if not find user, or userName is null empty return fail message
            if (!IsUserExist(request.Username) || string.IsNullOrEmpty(request.Username))
            {
                await Task.FromResult(new MessageResponse());

            }

            var currentYear = DateTime.Now.Year;

            var transactions = _dbContext.PocketTransactions.ToList().Where(pt => pt.UserId == UserId(request.Username)
            && Convert.ToDateTime(pt.Date).Year == currentYear
           ).ToList();

            if (!transactions.Any())
            {
                await Task.FromResult(new MessageResponse() { Success = false,Message="No record Is find"});
            }

            var categorySummaries = transactions
                   .GroupBy(t => new
                   {
                       DateTime.Parse(t.Date, CultureInfo.InvariantCulture).Date.Year,
                       DateTime.Parse(t.Date, CultureInfo.InvariantCulture).Date.Month,
                       t.Identification,
                       t.Category
                   })
                   .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month) // optional
                   .Select(group => new CategorySummary
                   {
                       Date = $"{group.Key.Year} - {group.Key.Month}",
                       Category=group.Key.Category,
                       TotalIncome = group.Where(t => t.IsIncome).Sum(t => t.Amount),
                       TotalExpense = group.Where(t => !t.IsIncome).Sum(t => t.Amount),
                       Identification = group.Key.Identification,
                   })
                   .ToList();

            foreach (var ds in categorySummaries)
            {
                await responseStream.WriteAsync(ds);
            }

        }
    }
}
