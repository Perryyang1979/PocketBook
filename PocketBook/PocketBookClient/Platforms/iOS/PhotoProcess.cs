using CoreImage;
using PocketBookClient.Services;
using UIKit;
using Vision;

namespace PocketBookClient.Platforms
{
    public class PhotoProcess : IPhotoProcess
    {
        public async Task<string> ProcessReceipt(string filePath)
        {
            TaskCompletionSource<string> tcs = new TaskCompletionSource<string>();

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
                tcs.SetResult(text);
            })
            {
                RecognitionLevel = VNRequestTextRecognitionLevel.Accurate,
                RecognitionLanguages = new string[] { "no-NO" }, // 设置为你需要的语言/"en", "zh-Hans", 
                UsesLanguageCorrection = true
            };

            try
            {
                var errorFoundation = new Foundation.NSError();
                vnImageRequestHandler.Perform(new VNRequest[] { textRecognitionRequest }, out errorFoundation);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                tcs.SetResult(null);
            }

            return await tcs.Task;
        }
    }
}
