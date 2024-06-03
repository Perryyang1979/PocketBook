
using System.Text.RegularExpressions;
#if ANDROID
using Tesseract;
#elif IOS
using CoreImage;
using UIKit;
using Vision;
#endif

namespace PocketBookClient.Helper
{
    public class PhotoProcessHelper
    {
        private readonly string MoneyIdentify = "Totalbeløp";

        public class PhotoTransaction
        {
            public DateTime Date { get; set; } 
            public string IdentifiedBrand { get; set; }
            public decimal Amount { get; set; }
        }

        public async Task<PhotoTransaction> ProcessReceipt(string filePath)
        {
#if ANDROID
            return   ProcessReceiptWithTesseract(filePath);
#elif IOS
            return  await ProcessReceiptWithVision(filePath);
#else
            throw new PlatformNotSupportedException("Unsupported platform");
#endif
        }
        private async Task<PhotoTransaction> ProcessReceiptWithTesseract(string filePath)
        {
#if ANDROID
             var engine = new Tesseract.TesseractEngine(@"./tessdata", "no", Tesseract.EngineMode.Default);
             var img = Tesseract.Pix.LoadFromFile(filePath);
             var page = engine.Process(img);
             var text = page.GetText();
            // 解析文本
             return await Task.FromResult(ParseReceipt(text));
#else
            return null;
#endif
        }

        private async Task<PhotoTransaction> ProcessReceiptWithVision(string filePath)
        {
#if IOS
            TaskCompletionSource<PhotoTransaction> tcs = new TaskCompletionSource<PhotoTransaction>();

            var image = UIImage.FromFile(filePath);
            var ciImage = new CIImage(image);
            var vnImageRequestHandler = new VNImageRequestHandler(ciImage, new VNImageOptions());

            var textRecognitionRequest = new VNRecognizeTextRequest((request, error) =>
            {
                if (error != null)
                {
                    Console.WriteLine("Error: " + error.LocalizedDescription);
                    tcs.SetResult(null);
                    return;
                }

                var observations = request.GetResults<VNRecognizedTextObservation>();
                var text = string.Join("\n", observations.SelectMany(o => o.TopCandidates(1).Select(c => c.String)));
                // 解析文本
                var transaction = ParseReceipt(text);
                tcs.SetResult(transaction);
            });

            textRecognitionRequest.RecognitionLevel = VNRequestTextRecognitionLevel.Accurate;

            try
            {
                var errorFoundation = new Foundation.NSError();
                vnImageRequestHandler.Perform(new VNRequest[] { textRecognitionRequest },out errorFoundation);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                tcs.SetResult(null);
            }

            return  await tcs.Task;
#else
            return null;
#endif
        }

        private PhotoTransaction ParseReceipt(string text)
        {
            decimal amount = 0;
            // 假设小票包含金额信息
            var amountLine = text.Split('\n').FirstOrDefault(line => line.Contains(MoneyIdentify));
            if (!string.IsNullOrEmpty(amountLine))
            {
                amount = ExtractAmount(amountLine);
                //  SaveTransaction(amount);
            }
            return new PhotoTransaction()
            {
                Amount = amount
            };
        }

        private decimal ExtractAmount(string line)
        {
            // 提取金额
            var match = Regex.Match(line, @"\d+(\.\d{1,2})?");
            return match.Success ? decimal.Parse(match.Value) : 0;
        }
    }
}
