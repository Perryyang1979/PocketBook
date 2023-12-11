
using PocketBookClient.Data;

namespace PocketBookClient.View;

public partial class MultiCheckBoxContentView : ContentView
{
    public static readonly BindableProperty TitleProperty =
           BindableProperty.Create(nameof(Title), typeof(string), typeof(MultiCheckBoxContentView), default(string));
    public static readonly BindableProperty ItemsOptionSourceProperty =
            BindableProperty.Create(nameof(ItemsOptionSource), typeof(ObservableCollection<SelectOption>), typeof(MultiCheckBoxContentView), default(ObservableCollection<SelectOption>));

    public static readonly BindableProperty SelectedOptionItemsProperty =
            BindableProperty.Create(nameof(SelectedOptionItems), typeof(ObservableCollection<SelectOption>), typeof(MultiCheckBoxContentView), default(ObservableCollection<SelectOption>));
    public MultiCheckBoxContentView()
	{
		InitializeComponent();
	}

    public ObservableCollection<SelectOption> ItemsOptionSource
    {
        get { return (ObservableCollection<SelectOption>)GetValue(ItemsOptionSourceProperty); }
        set { SetValue(ItemsOptionSourceProperty, value); }
    }

    public ObservableCollection<SelectOption> SelectedOptionItems
    {
        get { return (ObservableCollection<SelectOption>)GetValue(SelectedOptionItemsProperty); }
        set { SetValue(SelectedOptionItemsProperty, value); }
    }

    public string Title
    {
        get { return (string)GetValue(TitleProperty); }
        set { SetValue(TitleProperty, value); }
    }

    private static void OnTitleChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (MultiCheckBoxContentView)bindable;
            string title = (string)newValue;
         // control.labelTitle.Text = title;
        //    // TODO: Update UI or perform any other necessary logic
    }

    private void CheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if(sender is CheckBox cb) { 

            var bindingItem = (SelectedOptionItems.SingleOrDefault(s => cb.BindingContext.Equals(s)));
            
            if(bindingItem ==null){

                var selectOption = ItemsOptionSource.FirstOrDefault(s => cb.BindingContext.Equals(s));
                selectOption.IsSelected = true;
                SelectedOptionItems.Add(selectOption);
            }
            else
            {
                SelectedOptionItems.Remove(bindingItem);
            }

            OnPropertyChanged(nameof(SelectedOptionItems));
        }
    }

    //private static void OnItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
    //{
    //    // Handle the property changed logic here
    //    // For example, update the UI based on the new ObservableCollection
    //    var control = (MultiCheckBoxContentView)bindable;
    //    ObservableCollection<SelectOption> newItemsSource = (ObservableCollection<SelectOption>)newValue;
    //    // TODO: Update UI or perform any other necessary logic
    //}

    //private static void OnSelectedItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
    //{
    //    // Handle the property changed logic here
    //    // For example, update the UI based on the new ObservableCollection
    //    var control = (MultiCheckBoxContentView)bindable;
    //    ObservableCollection<SelectOption> newSelectedItems = (ObservableCollection<SelectOption>)newValue;

    //    // TODO: Update UI or perform any other necessary logic
    //}
}