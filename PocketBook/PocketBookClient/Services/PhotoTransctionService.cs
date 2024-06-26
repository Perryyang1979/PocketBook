using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PocketBookClient.Services
{
    public class PhotoProcessService
    {
        private readonly string[] MoneyIdentifyList = new string[] { "TOTALBELØP", "VAREKJØP", "BANK", "TOTAL elop", "VAREKJoP", "TOTALBELoP" };

        private readonly Dictionary<string, List<string>> BrandDictornary = new Dictionary<string, List<string>>()
        {
            {Helper.Utils.SuperMarket,new List<string> {"MENY","REMA 1000","KIWI","SPAR","JOKER" } },
            {Helper.Utils.ShoppingMall,new List<string> {"ZARA","HM","XXL","KID"} }
         };

        private readonly IPhotoProcess photoProcess;
        public PhotoProcessService(IPhotoProcess photoProcess) 
        {
            if (photoProcess == null) throw new ArgumentNullException("photo process can not null");

            this.photoProcess = photoProcess;
        }

        public async Task<PhotoTransaction> HandleProcess(string filePath)
        {
            var text = await photoProcess.ProcessReceipt(filePath);

            return await Task.FromResult(ParseReceipt(text));
        }

        private PhotoTransaction ParseReceipt(string text)
        {
            var lines = text?.Split('\n') ?? Array.Empty<string>();
            decimal amount = ExtractAmountFromText(lines);

            string brand = ExtractBrandCategory(lines);
            return new PhotoTransaction()
            {
                Amount = amount,
                IdentifiedBrand = brand
            };
        }

        private string ExtractBrandCategory(string[] lines)
        {
            var brand = string.Empty;

            foreach (var branddic in BrandDictornary)
            {
                if (lines.Any(line => branddic.Value.Any(token => line.Contains(token, StringComparison.OrdinalIgnoreCase))))
                {
                    brand = branddic.Key;
                    break;
                }
            }

            return brand;
        }

        private decimal ExtractAmountFromText( string[] lines)
        {
            var amountLine = lines.FirstOrDefault(line => MoneyIdentifyList.Any(token => line.Contains(token, StringComparison.OrdinalIgnoreCase)));
            if (!string.IsNullOrEmpty(amountLine))
            {
               return ExtractAmount(amountLine);
            }

            return 0;
        }

        private decimal ExtractAmount(string line)
        {
            try
            {
                var match = Regex.Match(line, @"\d+([,.]\d{1,2})?");
                if (match.Success)
                {
                    try
                    {
                        return decimal.Parse(FormatDecimal(match.Value), new CultureInfo("no-NO"));
                    }
                    catch (Exception ex)
                    {
                        return 0;
                    }
                }
                else
                    return 0;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        private string FormatDecimal(string strValue)
        {
            if (strValue.Contains("."))
            {
                return strValue.Replace(".", ",");
            }

            return strValue;
        }
    }

    public class PhotoTransaction
    {
        public DateTime Date { get; set; }
        public string IdentifiedBrand { get; set; }
        public decimal Amount { get; set; }
    }
}
