using PocketBookClient.Services;

namespace PocketBookClient.Platforms
{
    public class PhotoProcess : IPhotoProcess
    {
        public async Task<string> ProcessReceipt(string filePath)
        {
            var engine = new Tesseract.TesseractEngine(@"./tessdata", "no", Tesseract.EngineMode.Default);
            var img = Tesseract.Pix.LoadFromFile(filePath);
            var page = engine.Process(img);
            return await Task.FromResult(page.GetText());
        }
    }
}
