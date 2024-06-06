
using System.Globalization;
using System.Text.RegularExpressions;
#if ANDROID
using Tesseract;
using static Android.Service.Autofill.FieldClassification;
#elif IOS
using CoreImage;
using UIKit;
using Vision;
#endif

namespace PocketBookClient.Helper
{
    public class PhotoProcessHelper
    {
        private readonly string[] MoneyIdentifyList = new string[] { "TOTALBELØP", "VAREKJØP","TOTAL" };

        private const string Language = "no";

        public class PhotoTransaction
        {
            public DateTime Date { get; set; } 
            public string IdentifiedBrand { get; set; }
            public decimal Amount { get; set; }
        }

        public async Task<PhotoTransaction> ProcessReceipt(string filePath)
        {
#if ANDROID
            return   await ProcessReceiptWithTesseract(filePath);
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

              
                  var lines = observations
            .GroupBy(o => Math.Round(o.BoundingBox.Top, 2))  // 按照BoundingBox的顶部分组，并使用Math.Round进行分组
            .OrderBy(g => g.Key)  // 按照顶部位置排序
            .Select(g => string.Join(" ", g.SelectMany(o => o.TopCandidates(1).Select(c => c.String))))
            .ToList();
            var text = string.Join("\n", lines);
              // var text = string.Join("\n", observations.SelectMany(o => o.TopCandidates(1).Select(c => c.String)));
                // 解析文本
                var transaction = ParseReceipt(text);
                 tcs.SetResult(transaction);
            });

            textRecognitionRequest.RecognitionLevel = VNRequestTextRecognitionLevel.Accurate;
            textRecognitionRequest.RecognitionLanguages = new string[] { "no" }; // 设置为你需要的语言/"en", "zh-Hans", 

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
            var lines = text?.Split('\n') ?? Array.Empty<string>();
            var amountLine = lines.FirstOrDefault(line => MoneyIdentifyList.Any(token => line.Contains(token, StringComparison.OrdinalIgnoreCase)));
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
            var   match = Regex.Match(line, @"\d+([,.]\d{1,2})?");
            return match.Success ? decimal.Parse(match.Value, new CultureInfo("no-NO")) : 0;
        }
    }
}
