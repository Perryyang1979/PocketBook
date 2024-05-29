using Google.Apis.Sheets.v4.Data;

namespace PocketBookClient.GoogleSheeet
{
    internal interface IGoogleSheetsManager
    {
        Task<Spreadsheet> CreateNewAsync(string documentName);

        Spreadsheet GetSpreadsheet(string googleSpreadsheetIdentifier);

        ValueRange GetSingleValue(string googleSpreadsheetIdentifier, string valueRange);

        void RemoveSingleValue(string googleSpreadsheetIdentifier, string valueRange);

        BatchGetValuesResponse GetMultipleValues(string googleSpreadsheetIdentifier, string[] ranges);

        void RemoveMultipleValues(string googleSpreadsheetIdentifier, string[] ranges);

        Task<AppendValuesResponse> CreateSingleValueAsync(string googleSpreadsheetIdentifier, string range, List<Object> objectList);
        Task<UpdateValuesResponse> UpdateSingleValueAsync(string googleSpreadsheetIdentifier, string range, List<Object> objectList);
    }
}
