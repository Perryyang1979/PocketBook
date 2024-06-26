using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PocketBookClient.Data;
using PocketBookClient.Services;

namespace PocketBookClient.ViewModel
{

    public partial class IdentifyListViewModel:BaseViewModel
    {
        public ObservableCollection<TransactionIdentification> IdentifyList { get; set; } = new ObservableCollection<TransactionIdentification>();


        [ObservableProperty]
        private string identifyDescriptionAdd = string.Empty;
        [ObservableProperty]
        private bool addPanelVisible = false;
        [ObservableProperty]
        private bool addButtonEnable = false;
        [ObservableProperty]
        private bool confirmButtonEnable = false;
        [ObservableProperty]
        private bool deleteButtonEnable = false;
        [ObservableProperty]
        private TransactionIdentification currentSelectedItem = null;

        public IdentifyListViewModel()
        {
            InitializePanel();
            GetCurrentIdenitfy();
        }


        partial void OnIdentifyDescriptionAddChanged(string value)
        {
           if(string.IsNullOrEmpty(value))
            {
                ConfirmButtonEnable = false;
            }

            ConfirmButtonEnable = true;
        }

        partial void OnCurrentSelectedItemChanged(TransactionIdentification value)
        {
           if(value != null)
            {
                DeleteButtonEnable = true;
            }
        }

        private void InitializePanel()
        {
            AddButtonEnable = true;
            AddPanelVisible = false;
            IdentifyDescriptionAdd = string.Empty;
            ConfirmButtonEnable = false;
            DeleteButtonEnable = false;
            CurrentSelectedItem = null;
        }

        private async void GetCurrentIdenitfy()
        {
            IdentifyList.Clear();
 
            //using (var call = Communication.Instance.GetAllIdentificationStream())
            //{
            //    var token = new CancellationToken();
            //    while (await call.ResponseStream.MoveNext(token))
            //    {
            //        IdentificationInfo responseInfo = call.ResponseStream.Current;

            //            IdentifyList.Add(new TransactionIdentification()
            //            {
            //                IdentifyDescription = responseInfo.Description,
            //            });
                    
            //    }
            //}
            if(IdentifyList.Any())
            {
                CurrentSelectedItem=IdentifyList.FirstOrDefault();
            }
        }

        [RelayCommand]
        async Task AddIdentifyAsync()
        {
            AddPanelVisible = true;
        }

        [RelayCommand]
        async Task DeleteIdentifyAsync()
        {
           if(CurrentSelectedItem == null)
            {
                return;
            }

            var confirm = await Shell.Current.DisplayAlert("delete Identify ", "Are you sure to delete?", accept: "OK", "Cancel");

            if ( confirm)
            {
                //var result = await Communication.Instance.DeleteIdentificationAsync(CurrentSelectedItem);

                //if (result == null)
                //{
                //    await Shell.Current.DisplayAlert("delete Identify failed", "unexpected error.", "OK");
                //}
                //if (!result.Success)
                //{
                //    await Shell.Current.DisplayAlert("delete Identify failed", result.Message, "OK");
                //}

                GetCurrentIdenitfy();
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
            if(IdentifyDescriptionAdd == string.Empty)
            {
                await  Shell.Current.DisplayAlert("Add Idenitfy failed", "Description is mandory field.", "OK");
            }

            var identifyInfo = new TransactionIdentification()
            {
                IdentifyDescription = IdentifyDescriptionAdd,
            };

            //var result =await Communication.Instance.AddIdentificationAsync(identifyInfo);

            //if(result == null) {
            //    await Shell.Current.DisplayAlert("Add Idenitfy failed", "unexpected error.", "OK");
            //}
            //if (!result.Success)
            //{
            //    await Shell.Current.DisplayAlert("Add Idenitfy failed", result.Message, "OK");
            //}

            IdentifyDescriptionAdd = string.Empty;

            GetCurrentIdenitfy() ;
        }
    }
}
