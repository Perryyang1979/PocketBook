using PocketBookClient.Helper;

namespace PocketBookClient.ViewModel;

public partial class BaseViewModel : ObservableObject
{
    public LocalizationResourceManager LocalizationResourceManager
=> LocalizationResourceManager.Instance;

    [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotBusy))]
        bool isBusy;

        [ObservableProperty]
        string title;

        public bool IsNotBusy => !IsBusy;
}
