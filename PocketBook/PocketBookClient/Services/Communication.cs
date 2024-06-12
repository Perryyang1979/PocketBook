
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using PocketBookClient.Data;
using PocketBookClient.GoogleSheeet;
using PocketBookClient.Helper;
using System.Globalization;


namespace PocketBookClient.Services
{
    public  class Communication
    {
        private const string googleClientId = "91971851610-v664ka8th2qpbfbm8qshuq8eaj1721ht.apps.googleusercontent.com";
        private const string googleClientSecret = "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";
        private const string spreadSheetId = "1fVhez65UqRgRq9Dz0Wpnu7SsAOPwZ9AluB-T18K7T18";
        private const string UserSheet = "User";
        private const string CategorySheet = "Category";
        private const string UserGroupSheet = "UserGroup";
        private const string DailySheet = "DailyRecord";

        Dictionary<int, string> sheetIDAndNameDictionary = new Dictionary<int, string>()
        {
            { 52291343, UserSheet },
            { 721815880, CategorySheet },
            { 1834737748, DailySheet }
        };


        private static Communication _instance;
        private GoogleSheetsManager _sheetsManager;
        private IConnectivity _connectivity;
        private List<TransactionCategory> _transactionCategories;
        private Spreadsheet _spreadsheet;
        private List<UserTransaction> _userTransactions;

        public class MessageResponse
        {
            public bool Success { get; set; }
            public string Message { get; set;}
        }


        private Communication()
        {
            _connectivity = ServiceHelper.GetService<IConnectivity>();
        }

        private GoogleSheetsManager CurrentSheetsManager
        {
            get
            {
                var checkResult = CheckInternetConnection().Result;
                if (_sheetsManager == null)
                {
                    if (checkResult)
                    {
                        //  string[] scopes = new[] { Google.Apis.Sheets.v4.SheetsService.Scope.Spreadsheets };
                        var credential = GoogleAuthentication.LoginCredential();
                        _sheetsManager = new GoogleSheetsManager(credential);
                        return _sheetsManager;
                    }

                    return null;
                }

                if (checkResult)
                {
                    return _sheetsManager;
                }

                return null;
            }
        }

        public Spreadsheet CurrentSpreadSheet
        {
            get
            {
                if(CurrentSheetsManager== null)
                {
                    return null;
                }

                if(_spreadsheet == null)
                {
                    _spreadsheet = CurrentSheetsManager.GetSpreadsheet(spreadSheetId);
                }

                return _spreadsheet;   
            }
        }

        private int GetSheetIdFromSheetName(string sheetName)
        {
            foreach(var key in sheetIDAndNameDictionary.Keys) {
                if(sheetIDAndNameDictionary[key] == sheetName)
                {
                    return key;
                }
            }

            return 0;
        }

        private async Task<bool> CheckInternetConnection()
        {
            if(_connectivity.NetworkAccess!=NetworkAccess.Internet)
            {
                await Shell.Current.DisplayAlert("Internet Disconnected", "No Internet,Check you internet connection!", "OK");
                return false;
            }

            return true;
        }

        public static Communication Instance
        {
            get
            {
                return _instance ?? (_instance = new Communication());
            }
        }

        private List<TransactionCategory>  Categories
        {
            get
            {
                if (_transactionCategories == null)
                {
                    _transactionCategories = new List<TransactionCategory>();
                    GetAllCategory();
                }

                return _transactionCategories;
            }
        }

        private List<UserTransaction> Transactions
        {
            get
            {
                if (_userTransactions == null)
                {
                    _userTransactions = new List<UserTransaction>();
                    GetAllTransactions();
                }

                return _userTransactions;
            }
        }

        public async Task<MessageResponse> RegisterUserAsync(PocketBookClient.Data.User user)
        {
            var checkResult = await CheckInternetConnection();
            if(!checkResult) 
            { 
                return  new MessageResponse { Success = false }; 
            }

            var spreadSheet = CurrentSheetsManager.GetSpreadsheet(spreadSheetId);

            var newRecordRange = $"{UserSheet}!A:F";
            var newRecord = new List<object>() { DateTime.Today.ToShortDateString(), user.Name, RSAService.EncryptData(user.Password), user.PhoneNumber, user.Email, user.GroupId };

            try
            {

              var  response = await CurrentSheetsManager.CreateSingleValueAsync(spreadSheet.SpreadsheetId, newRecordRange, newRecord);

                return new MessageResponse { Success = true };

            }
            catch(Exception ex)
            {
                return new MessageResponse { Success = false };
            }
          }

        public async Task<MessageResponse>  ChangePasswordAsync(string userName, string oldPassword,string newPassword)
        {
            var checkResult = CheckInternetConnection();
            if (!checkResult.Result)
            {
                return new MessageResponse { Success = false };
            }

            var existedUsers = GetAllUsers();

            var loginUser = existedUsers.SingleOrDefault(u => u.Name.Equals(userName) && u.Password.Equals(oldPassword));

            if(loginUser == null)
            {
                return new MessageResponse { Success = false };
            }

            var keyRange = $"{UserSheet}!B:B";

            var indexToUpdate = _sheetsManager.FindRowByKey(CurrentSpreadSheet.SpreadsheetId, keyRange, loginUser.Name);


            var updateRange = $"{UserSheet}!C{indexToUpdate}:C{indexToUpdate}";

            var newRecord = new List<object>() { RSAService.EncryptData(newPassword) };

            try
            {

                UpdateValuesResponse response = await _sheetsManager.UpdateSingleValueAsync(CurrentSpreadSheet.SpreadsheetId, updateRange, newRecord);

                return new MessageResponse { Success = true };

            }
            catch (Exception ex)
            {
                return new MessageResponse { Success = false };
            }

        }


        public async Task<MessageResponse>  ResetPasswordAsync(string email)
        {
            var checkResult = CheckInternetConnection();
            if (!checkResult.Result)
            {
                return new MessageResponse { Success = false };
            }

            var existedUsers = GetAllUsers();

            var currentUser = existedUsers.SingleOrDefault(u => u.Email == email);
            if (currentUser == null)
            {
                return await Task.FromResult(new MessageResponse { Success = false, Message = "Email address is not registered." });
            }

            var resetPassword = Helper.Utils.GetRandomPassword(8);

            if (Helper.Utils.SendNewPasswordEmail(currentUser.Email, resetPassword) == false)
            {
                return await Task.FromResult(new MessageResponse { Success = false, Message = "Password Reset failure." });
            }

            var keyRange = $"{UserSheet}!B:B";

            var indexToUpdate = _sheetsManager.FindRowByKey(CurrentSpreadSheet.SpreadsheetId, keyRange, currentUser.Name);


            var updateRange = $"{UserSheet}!C{indexToUpdate}:C{indexToUpdate}";

            var newRecord = new List<object>() { RSAService.EncryptData(resetPassword) };

            try
            {

                UpdateValuesResponse response = await _sheetsManager.UpdateSingleValueAsync(CurrentSpreadSheet.SpreadsheetId, updateRange, newRecord);

                return await Task.FromResult(new MessageResponse { Success = true, Message = "Password Reset successfully." });

            }
            catch (Exception ex)
            {
                return new MessageResponse { Success = false };
            }
        }

        public List<User> GetAllUsers()
        {
            var checkResult =  CheckInternetConnection();
            if (!checkResult.Result)
            {
                return new List<User>();
            }

            var users = new List<User>();

            var range = $"{UserSheet}!A1:F100";
            string[] valueRange = new[] { range };

            var response = CurrentSheetsManager.GetMultipleValues(CurrentSpreadSheet.SpreadsheetId, valueRange);

            var userList = response.ValueRanges;

            if (userList != null && userList.Count > 0)
            {
                foreach (var row in userList.ElementAt(0).Values)
                {
                    if (row.Count >= 6)  //make sure there is enough data
                    {
                        users.Add(new User()
                        {
                            Name = row[1].ToString(),
                            Password = RSAService.DecryptData(row[2].ToString()),
                            PhoneNumber = row[3].ToString(),
                            Email = row[4].ToString(),
                            GroupId = row[5].ToString(),
                        });
                    }

                };

            }
            else
            {
                Console.WriteLine("no data found.");
            }


            return users;
        }

        public User LoginUser(string userName,string password)
        {
            var checkResult = CheckInternetConnection();
            if (!checkResult.Result)
            {
                return null ;
            }

            var existedUsers = GetAllUsers();

            var loginUser = existedUsers.SingleOrDefault(u=>u.Name.Equals(userName) && u.Password.Equals(password));

            return loginUser;
        }

        public async Task<MessageResponse> AddCategoryAsync(PocketBookClient.Data.TransactionCategory category)
        {
            var checkResult = await CheckInternetConnection();
            if (!checkResult)
            {
                return new MessageResponse { Success = false };
            }

            //check category exist
            if(Categories.Count == 0)
            {
                GetAllCategory();
            }

            if (Categories.Any(c => c.IsIncome == category.IsIncome && c.CategoryDescription == category.CategoryDescription))
            {
                return new MessageResponse { Success = false, Message = "There exist this category Description" };
            }

            if (CurrentSpreadSheet == null)
            {
                return new MessageResponse { Success = false, Message = "There exist this category Description" };
            }

            var newRecordRange = $"{CategorySheet}!A:E";
            var newRecord = new List<object>() { category.GuidCategory,Settings.Current.CurrentUserSetting.UserGroupId, category.IsIncome, category.CategoryDescription, category.CategoryDetailedDescription };

            try
            {

                var response = await _sheetsManager.CreateSingleValueAsync(CurrentSpreadSheet.SpreadsheetId, newRecordRange, newRecord);

                _transactionCategories.Add(category);

                return new MessageResponse { Success = true };

            }
            catch (Exception ex)
            {
                return new MessageResponse { Success = false };
            }
        }

        public async Task<MessageResponse> UpdateCategoryAsync(PocketBookClient.Data.TransactionCategory category)
        {
            var checkResult = await CheckInternetConnection();
            if (!checkResult)
            {
                return new MessageResponse { Success = false };
            }

            //check category exist
            if (Categories.Count == 0)
            {
                GetAllCategory();
            }

            if (!Categories.Any(c => c.IsIncome == category.IsIncome && c.CategoryDescription == category.CategoryDescription))
            {
                return new MessageResponse { Success = false, Message = "this category not exist in the database" };
            }

            if (CurrentSpreadSheet == null)
            {
                return new MessageResponse { Success = false, Message = "There exist this category Description" };
            }


            var newRecordRange = $"{CategorySheet}!A:E";
            var newRecord = new List<object>() { Settings.Current.CurrentUserSetting.UserGroupId, category.IsIncome, category.CategoryDescription, category.CategoryDetailedDescription };

            try
            {

                AppendValuesResponse response = await _sheetsManager.CreateSingleValueAsync(CurrentSpreadSheet.SpreadsheetId, newRecordRange, newRecord);

                return new MessageResponse { Success = true };

            }
            catch (Exception ex)
            {
                return new MessageResponse { Success = false };
            }
        }


        public async Task<MessageResponse> DeleteCategoryAsync(PocketBookClient.Data.TransactionCategory category)
        {
            var checkResult = await CheckInternetConnection();
            if (!checkResult)
            {
                return new MessageResponse { Success = false };
            }

            //check category exist
            if (Categories.Count == 0)
            {
                GetAllCategory();
            }

            if (!Categories.Any(c => c.IsIncome == category.IsIncome && c.CategoryDescription == category.CategoryDescription))
            {
                return new MessageResponse { Success = false, Message = "this category not exist in the database" };
            }

            if (CurrentSpreadSheet == null)
            {
                return new MessageResponse { Success = false, Message = "There exist this category Description" };
            }

            var keyRange = $"{CategorySheet}!A:A";

            var indexToDelete = _sheetsManager.FindRowByKey(CurrentSpreadSheet.SpreadsheetId, keyRange, category.GuidCategory);

            var sheetId = GetSheetIdFromSheetName(CategorySheet);

            try
            {

                _sheetsManager.DeleteRow(CurrentSpreadSheet.SpreadsheetId, sheetId, indexToDelete != null ? (int)indexToDelete - 1 : 0);

                TransactionCategory deleteCatelog = _transactionCategories.SingleOrDefault(t => t.GuidCategory == category.GuidCategory);

                _transactionCategories.Remove(deleteCatelog);

                return new MessageResponse { Success = true };

            }
            catch (Exception ex)
            {
                return new MessageResponse { Success = false };
            }


            //var deleteRecordRange = $"{CategorySheet}!A{indexToDelete}:E{indexToDelete}";


            //try
            //{

            //      _sheetsManager.RemoveSingleValue(CurrentSpreadSheet.SpreadsheetId, deleteRecordRange);

            //    return new MessageResponse { Success = true };

            //}
            //catch (Exception ex)
            //{
            //    return new MessageResponse { Success = false };
            //}
        }

        public List<TransactionCategory> GetCategories()
        {
            return Categories.ToList();
        }

        private void GetAllCategory()
        {
            var checkResult = CheckInternetConnection();
            if (!checkResult.Result)
            {
                return ;
            }

            List<TransactionCategory> categorys = new List<TransactionCategory>();
            if (CurrentSpreadSheet == null)
            {
                return;
            }

            var range = $"{CategorySheet}!A1:D10000";
            string[] valueRange = new[] { range };

            var response = CurrentSheetsManager.GetMultipleValues(CurrentSpreadSheet.SpreadsheetId, valueRange);

            var categoryList = response.ValueRanges;

            if (categoryList != null && categoryList.Count > 0 && categoryList.ElementAt(0).Values!=null)
            {
                foreach (var row in categoryList.ElementAt(0).Values)
                {
                    if (row.Count >= 4 && row[1].ToString() == Settings.Current.CurrentUserSetting.UserGroupId)  //make sure there is enough data
                    {
                        categorys.Add(new TransactionCategory()
                        {
                            GuidCategory = row[0].ToString(),
                            IsIncome = row[2].ToString() == "TRUE",
                            CategoryDescription = row[3].ToString(),
                            CategoryDetailedDescription = row.Count==5?row[4].ToString():string.Empty
                           ,
                        });
                    }

                };

            }
            else
            {
                Console.WriteLine("no data found.");
            }

            _transactionCategories.Clear();
            _transactionCategories = categorys;
        }


        public async Task<MessageResponse> AddTransactionAsync(PocketBookClient.Data.UserTransaction transaction)
        {
            var checkResult = await CheckInternetConnection();
            if (!checkResult)
            {
                return new MessageResponse { Success = false };
            }

            //check category exist
            if (Transactions.Count == 0)
            {
                GetAllTransactions();
            }

            if (Transactions.Any(c => c.IsIncome == transaction.IsIncome && c.Date == transaction.Date && c.Amount == transaction.Amount
            && c.UserName == transaction.UserName

            && c.Category == transaction.Category))
            {
                return new MessageResponse { Success = false, Message = "this category  exist in the database" };
            }

            if (CurrentSpreadSheet == null)
            {
                return new MessageResponse { Success = false, Message = "There exist this category Description" };
            }


            var newRecordRange = $"{DailySheet}!A:H";
            var newRecord = new List<object>() { transaction.Id,
                Settings.Current.CurrentUserSetting.UserGroupId, transaction.UserName,
                transaction.Date, transaction.IsIncome, transaction.Amount,
                 transaction.Category, transaction.Comments };
            try
            {

                var response = await _sheetsManager.CreateSingleValueAsync(CurrentSpreadSheet.SpreadsheetId, newRecordRange, newRecord);

                _userTransactions.Add(transaction);

                return new MessageResponse { Success = true };

            }
            catch (Exception ex)
            {
                return new MessageResponse { Success = false };
            }
        }
        //  //ID, UserGroup,UserName,Date,InCome,Money,CategaryDescription,Comments
        public async Task<MessageResponse> UpdateTransactionAsync(PocketBookClient.Data.UserTransaction transaction)
        {
            var checkResult = await CheckInternetConnection();
            if (!checkResult)
            {
                return new MessageResponse { Success = false };
            }

            //check category exist
            if (Transactions.Count == 0)
            {
                GetAllTransactions();
            }

            if (!Transactions.Any(c =>c.Id == transaction.Id))
            {
                return new MessageResponse { Success = false, Message = "this transaction not exist in the database" };
            }

            if (CurrentSpreadSheet == null)
            {
                return new MessageResponse { Success = false, Message = "There exist this transaction Description" };
            }

            var keyRange = $"{DailySheet}!A:A";

            var indexToUpdate = _sheetsManager.FindRowByKey(CurrentSpreadSheet.SpreadsheetId, keyRange, transaction.Id);


            var updateRange = $"{DailySheet}!A{indexToUpdate}:H{indexToUpdate}";

            var newRecord = new List<object>() { transaction.Id, 
                Settings.Current.CurrentUserSetting.UserGroupId, transaction.UserName,
                transaction.Date, transaction.IsIncome, transaction.Amount,
                 transaction.Category, transaction.Comments };

            try
            {

                UpdateValuesResponse response = await _sheetsManager.UpdateSingleValueAsync(CurrentSpreadSheet.SpreadsheetId, updateRange, newRecord);

                var updateTransaction = _userTransactions.SingleOrDefault(t=>t.Id ==  transaction.Id);
                if(updateTransaction!=null)
                {
                    updateTransaction.UserName = transaction.UserName;
                    updateTransaction.Comments = transaction.Comments;
                    updateTransaction.Amount = transaction.Amount;
                    updateTransaction.Category = transaction.Category;
                    updateTransaction.Date = transaction.Date;
                    updateTransaction.IsIncome = transaction.IsIncome;
                }

                return new MessageResponse { Success = true };

            }
            catch (Exception ex)
            {
                return new MessageResponse { Success = false };
            }
        }


        public async Task<MessageResponse> DeleteUserTransactionAsync(PocketBookClient.Data.UserTransaction transaction)
        {
            var checkResult = await CheckInternetConnection();
            if (!checkResult)
            {
                return new MessageResponse { Success = false };
            }

            //check category exist
            if (Transactions.Count == 0)
            {
                GetAllTransactions();
            }

            if (!Transactions.Any(c => c.Id == transaction.Id))
            {
                return new MessageResponse { Success = false, Message = "this transaction not exist in the database" };
            }

            if (CurrentSpreadSheet == null)
            {
                return new MessageResponse { Success = false, Message = "There exist this category Description" };
            }

            var keyRange = $"{DailySheet}!A:A";

            var indexToDelete = _sheetsManager.FindRowByKey(CurrentSpreadSheet.SpreadsheetId, keyRange, transaction.Id);


            //var deleteRecordRange = $"{DailySheet}!A{indexToDelete}:H{indexToDelete}";


            var sheetId =  GetSheetIdFromSheetName(DailySheet);

            try
            {

                //_sheetsManager.RemoveSingleValue(CurrentSpreadSheet.SpreadsheetId, deleteRecordRange);

                //var deleteTransaction = _userTransactions.SingleOrDefault(t => t.Id == transaction.Id);

                _sheetsManager.DeleteRow(CurrentSpreadSheet.SpreadsheetId, sheetId, indexToDelete!=null? (int)indexToDelete-1: 0);

                UserTransaction deleteTransaction = _userTransactions.SingleOrDefault(t => t.Id == transaction.Id);

                _userTransactions.Remove(deleteTransaction);

                return new MessageResponse { Success = true };

            }
            catch (Exception ex)
            {
                return new MessageResponse { Success = false };
            }
        }
        /// <summary>
        /// get all the transaction with same user group ID
        /// </summary>
        /// <returns></returns>
        public List<UserTransaction> GetUserTransaction()
        {
            return Transactions.OrderBy(t=>t.Date).ToList();
        }

        public List<UserTransaction> GetDailyTransations(DateTime startDate, DateTime endDate)
        {
            return Transactions.Where(t => Convert.ToDateTime(t.Date) >= startDate && Convert.ToDateTime(t.Date) <= endDate).OrderByDescending(t => Convert.ToDateTime( t.Date)).ToList();
        }

        public List<UserTransaction>  GetDailyTransationsInCurrentMonth()
        {
            var currentDate = DateTime.Now.Date;

            var startDate = currentDate.AddDays(1 - currentDate.Day);
            var endDate = currentDate.AddDays(30 - currentDate.Day);

            return GetDailyTransations(startDate, endDate);
        }

        public List<MonthlySummary> GetMonthlyReport()
        {
            var transactions = Transactions;

            if(!transactions.Any())
            {
                return new List<MonthlySummary>();
            }

            return transactions
                    .GroupBy(t => new
                    {
                        DateTime.Parse(t.Date, CultureInfo.InvariantCulture).Date.Year,
                        DateTime.Parse(t.Date, CultureInfo.InvariantCulture).Date.Month,
                        t.UserName,
                    })
                    .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month) // optional
                    .Select(group => new MonthlySummary
                    {
                        Date = $"{group.Key.Year} - {group.Key.Month}",
                        UserName = group.Key.UserName,
                        TotalIncome = group.Where(t => t.IsIncome).Sum(t => t.Amount),
                        TotalExpense = group.Where(t => !t.IsIncome).Sum(t => t.Amount),
                    })
                    .ToList();
        }

        public List<MonthlyCategorySummary> GetMonthlyCategoryReport()
        {
            var transactions = Transactions;
            if (!transactions.Any())
            {
                return new List<MonthlyCategorySummary>();
            }

            var categorySummaries = transactions
                  .GroupBy(t => new
                  {
                      DateTime.Parse(t.Date, CultureInfo.InvariantCulture).Date.Year,
                      DateTime.Parse(t.Date, CultureInfo.InvariantCulture).Date.Month,
                      t.Category
                  })
                  .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month) // optional
                  .Select(group => new MonthlyCategorySummary
                  {
                      Date = $"{group.Key.Year} - {group.Key.Month}",
                      Category = group.Key.Category,
                      TotalMoney = group.Sum(t => t.Amount) ,
                  })
                  .ToList();
            return categorySummaries;
        }

        private void GetAllTransactions()
        {
            var checkResult = CheckInternetConnection();
            if (!checkResult.Result)
            {
                return;
            }

            List<UserTransaction> transactions = new List<UserTransaction>();
            if (CurrentSpreadSheet == null)
            {
                return;
            }

            var range = $"{DailySheet}!A1:H10000";
            string[] valueRange = new[] { range };

            var response = CurrentSheetsManager.GetMultipleValues(CurrentSpreadSheet.SpreadsheetId, valueRange);

            var transactionList = response.ValueRanges;

            if (transactionList != null && transactionList.Count > 0 && transactionList.ElementAt(0).Values != null)
            {
                foreach (var row in transactionList.ElementAt(0).Values)
                {
                    if (row.Count >= 7 && row[1].ToString() == Settings.Current.CurrentUserSetting.UserGroupId)  //make sure there is enough data
                        //ID, UserGroup,UserName,Date,InCome,Money,CategaryDescription,Comments
                        //
                    {
                        transactions.Add(new UserTransaction()
                        {
                            Id = row[0].ToString(),
                            UserName = row[2].ToString(),
                            Date = row[3].ToString(),
                            IsIncome = row[4].ToString() == "TRUE",
                            Amount = Double.Parse(row[5].ToString()),
                            Category = row[6].ToString(),
                            Comments = row.Count == 8 ?row[7].ToString():string.Empty
                        });
                    }

                };

            }
            else
            {
                Console.WriteLine($"no data found for currrent userId {Settings.Current.CurrentUserSetting.UserGroupId}");
            }

            _userTransactions.Clear();
            _userTransactions = transactions;
        }
    }
}
