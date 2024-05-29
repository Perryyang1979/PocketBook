using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PocketBookClient.Data;
using PocketBookClient.Services;

namespace PocketBookClient.ViewModel
{

    [QueryProperty(nameof(CurrentCategoryType), "CurrentCategoryType")]
    public partial class CategoryListViewModel:BaseViewModel
    {
        public ReadOnlyCollection<string> MoneyChangeTypes { get; }
        public ObservableCollection<TransactionCategory> CategoryList { get; set; } = new ObservableCollection<TransactionCategory>();

        private readonly string IsIncome = "True";

        [ObservableProperty]
        string currentCategoryType = string.Empty;
        [ObservableProperty]
        private string categoryDescriptionAdd = string.Empty;
        [ObservableProperty]
        private string categoryDetailAdd = string.Empty;
        [ObservableProperty]
        private bool addCategoryPanelVisible = false;
        [ObservableProperty]
        private bool addButtonEnable = false;
        [ObservableProperty]
        private bool confirmButtonEnable = false;
        [ObservableProperty]
        private bool deleteButtonEnable = false;
        [ObservableProperty]
        private TransactionCategory currentSelectedItem = null;

        public CategoryListViewModel()
        {
            InitializePanel();
        }

        partial void OnCurrentCategoryTypeChanged(string value)
        {
            if(value != string.Empty) {
                AddButtonEnable = true;
            }

            InitializePanel();

             var isIncome = value.Equals(IsIncome);
             GetCurrentCategories(isIncome);   
        }

        partial void OnCategoryDescriptionAddChanged(string value)
        {
           if(string.IsNullOrEmpty(value))
            {
                ConfirmButtonEnable = false;
            }

            ConfirmButtonEnable = true;
        }

        partial void OnCurrentSelectedItemChanged(TransactionCategory value)
        {
           if(value != null)
            {
                DeleteButtonEnable = true;
            }
        }

        private void InitializePanel()
        {
            AddCategoryPanelVisible = false;
            CategoryDescriptionAdd = string.Empty;
            CategoryDetailAdd = string.Empty;
            ConfirmButtonEnable = false;
            DeleteButtonEnable = false;
            CurrentSelectedItem = null;
        }

        private async void GetCurrentCategories(bool isIncome)
        {
            CategoryList.Clear();

            foreach (var category in Communication.Instance.GetCategories().Where(c=>c.IsIncome == isIncome))
            {
              
                CategoryList.Add(category);
            }

            if (CategoryList.Any())
            {
                CurrentSelectedItem=CategoryList.FirstOrDefault();
            }
        }

        [RelayCommand]
        async Task AddCategoryAsync()
        {
            AddCategoryPanelVisible = true;
        }

        [RelayCommand]
        async Task DeleteCategoryAsync()
        {
           if(CurrentSelectedItem == null)
            {
                return;
            }

            var confirm = await Shell.Current.DisplayAlert("delete category ", "Are you sure to delete?", accept: "OK", "Cancel");

            if ( confirm)
            {
                var result = await Communication.Instance.DeleteCategoryAsync(CurrentSelectedItem);

                if (result == null)
                {
                    await Shell.Current.DisplayAlert("delete category failed", "unexpected error.", "OK");
                }
                if (!result.Success)
                {
                    await Shell.Current.DisplayAlert("delete category failed", result.Message, "OK");
                }

                GetCurrentCategories(CurrentSelectedItem.IsIncome);
            }
        }

        [RelayCommand]
        async Task CancelAsync()
        {
            InitializePanel();
        }

        [RelayCommand]
        async Task ConfirmAsync()
        {
            if(CategoryDescriptionAdd == string.Empty)
            {
                await  Shell.Current.DisplayAlert("Add category failed", "Description is mandory field.", "OK");
            }

            if(CurrentCategoryType == string.Empty) {
                await Shell.Current.DisplayAlert("Add category failed", "Select type to add .", "OK");
            }

            var categoryInfo = new TransactionCategory()
            {
                GuidCategory = Guid.NewGuid().ToString(),
                IsIncome = CurrentCategoryType.Equals(IsIncome),
                CategoryDescription = CategoryDescriptionAdd,
                CategoryDetailedDescription = CategoryDetailAdd
            };

            var result =await Communication.Instance.AddCategoryAsync(categoryInfo);

            if (result == null)
            {
                await Shell.Current.DisplayAlert("Add category failed", "unexpected error.", "OK");
            }
            if (!result.Success)
            {
                await Shell.Current.DisplayAlert("Add category failed", result.Message, "OK");
            }

            CategoryDescriptionAdd = string.Empty;
            CategoryDetailAdd = string.Empty;

            GetCurrentCategories(categoryInfo.IsIncome);
        }
    }
}
