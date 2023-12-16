namespace PocketBookClient.View;

public partial class ScannerPage : ContentPage
{
	public ScannerPage()
	{
		InitializeComponent();
        barcodereader.Options = new ZXing.Net.Maui.BarcodeReaderOptions
        {
            Formats = ZXing.Net.Maui.BarcodeFormat.QrCode
        };
    }

    private void CameraBarcodeReaderView_BarcodesDetected(object sender, ZXing.Net.Maui.BarcodeDetectionEventArgs e)
    {
        barcodereader.IsDetecting = false;
        if (e.Results.Any())
        {
            var result = e.Results.FirstOrDefault();
            Dispatcher.Dispatch(async () =>
            {
                await Shell.Current.GoToAsync(nameof(RegistrationPage), true, new Dictionary<string, object>
                    {
                        {"UserGroupId", result.Value.ToString() }
                 });
            });
        }
    }
}