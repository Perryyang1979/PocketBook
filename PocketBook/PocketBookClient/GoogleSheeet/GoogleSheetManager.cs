using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Microsoft.Maui.Controls;
using System;

namespace PocketBookClient.GoogleSheeet
{
    public class GoogleSheetsManager : IGoogleSheetsManager
    {
        //   private readonly UserCredential _credential;

        private readonly GoogleCredential _credential;

        public GoogleSheetsManager(GoogleCredential credential)
        {
            _credential = credential;
        }

        public async Task<Spreadsheet> CreateNewAsync(string documentName)
        {
            if (string.IsNullOrEmpty(documentName))
                throw new ArgumentNullException(nameof(documentName));

            using (var sheetsService = new SheetsService(new BaseClientService.Initializer() { HttpClientInitializer = _credential }))
            {
                var documentCreationRequest = sheetsService.Spreadsheets.Create(new Spreadsheet()
                {
                    Sheets = new List<Sheet>() { new Sheet()
                      {
                          Properties = new SheetProperties()
                          {
                              Title = documentName,
                          }
                      } },

                    Properties = new SpreadsheetProperties()
                    {
                        Title = documentName,
                    }

                });

                return await documentCreationRequest.ExecuteAsync();
            }
        }

        public BatchGetValuesResponse GetMultipleValues(string googleSpreadsheetIdentifier, string[] ranges)
        {
            if (string.IsNullOrEmpty(googleSpreadsheetIdentifier))
                throw new ArgumentNullException(nameof(googleSpreadsheetIdentifier));

            if (ranges == null || ranges.Length == 0)
                throw new ArgumentNullException(nameof(ranges));

            using (var sheetsService = new SheetsService(new BaseClientService.Initializer() { HttpClientInitializer = _credential }))
            {
                var getValueRequest = sheetsService.Spreadsheets.Values.BatchGet
                    (googleSpreadsheetIdentifier);

                getValueRequest.Ranges = ranges;

                return getValueRequest.Execute();
            }
        }

        public ValueRange GetSingleValue(string googleSpreadsheetIdentifier, string valueRange)
        {
            if (string.IsNullOrEmpty(googleSpreadsheetIdentifier))
                throw new ArgumentNullException(nameof(googleSpreadsheetIdentifier));

            if (string.IsNullOrEmpty(valueRange))
                throw new ArgumentNullException(nameof(valueRange));

            using (var sheetsService = new SheetsService(new BaseClientService.Initializer() { HttpClientInitializer = _credential }))
            {
                var getValueRequest = sheetsService.Spreadsheets.Values.Get(googleSpreadsheetIdentifier, valueRange);

                return getValueRequest.Execute();
            }
        }

        public Spreadsheet GetSpreadsheet(string googleSpreadsheetIdentifier)
        {
            if (string.IsNullOrEmpty(googleSpreadsheetIdentifier))
                throw new ArgumentNullException(nameof(googleSpreadsheetIdentifier));

            using (var sheetsService = new SheetsService(new BaseClientService.Initializer() { HttpClientInitializer = _credential }))
            {
                return sheetsService.Spreadsheets.Get(googleSpreadsheetIdentifier).Execute();
            }
        }

       // var deleteRecordRange = $"{sheet}!A5:F5";
      //  googlesheetManager.RemoveSingleValue(spreadSheet.SpreadsheetId, deleteRecordRange);
        public void RemoveSingleValue(string googleSpreadsheetIdentifier, string valueRange)
        {
            if (string.IsNullOrEmpty(googleSpreadsheetIdentifier))
                throw new ArgumentNullException(nameof(googleSpreadsheetIdentifier));

            if (string.IsNullOrEmpty(valueRange))
                throw new ArgumentNullException(nameof(valueRange));

            using (var sheetsService = new SheetsService(new BaseClientService.Initializer() { HttpClientInitializer = _credential }))
            {
                var removeValueRequest = sheetsService.Spreadsheets.Values.Clear(new ClearValuesRequest(), googleSpreadsheetIdentifier, valueRange);

                removeValueRequest.Execute();
            }
        }

        public void RemoveMultipleValues(string googleSpreadsheetIdentifier, string[] ranges)
        {
            if (string.IsNullOrEmpty(googleSpreadsheetIdentifier))
                throw new ArgumentNullException(nameof(googleSpreadsheetIdentifier));

            if (ranges == null || ranges.Length == 0)
                throw new ArgumentNullException(nameof(ranges));

            using (var sheetsService = new SheetsService(new BaseClientService.Initializer() { HttpClientInitializer = _credential }))
            {
                var clearValueRequest = sheetsService.Spreadsheets.Values.BatchClear(new BatchClearValuesRequest() { Ranges = ranges },
                    googleSpreadsheetIdentifier);

                clearValueRequest.Execute();
            }
        }

        //range = $"{sheet}!A:F";
        public async Task<AppendValuesResponse> CreateSingleValueAsync(string googleSpreadsheetIdentifier, string range, List<Object> objectList)
        {
            if (string.IsNullOrEmpty(googleSpreadsheetIdentifier))
                throw new ArgumentNullException(nameof(googleSpreadsheetIdentifier));

            if (string.IsNullOrEmpty(range))
                throw new ArgumentNullException(nameof(range));

            var valueRange = new ValueRange();

            valueRange.Values = new List<IList<Object>> { objectList };

            using (var sheetsService = new SheetsService(new BaseClientService.Initializer() { HttpClientInitializer = _credential }))
            {
                var appendRequest = sheetsService.Spreadsheets.Values.Append(valueRange, googleSpreadsheetIdentifier, range);

                appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;

              return await appendRequest.ExecuteAsync();
            }
        }

        //range = $"{sheet}!D543";  single cell
        public async Task<UpdateValuesResponse> UpdateSingleValueAsync(string googleSpreadsheetIdentifier, string range, List<Object> objectList)
        {
            if (string.IsNullOrEmpty(googleSpreadsheetIdentifier))
                throw new ArgumentNullException(nameof(googleSpreadsheetIdentifier));

            if (string.IsNullOrEmpty(range))
                throw new ArgumentNullException(nameof(range));

            var valueRange = new ValueRange();

            valueRange.Values = new List<IList<Object>> { objectList };

            using (var sheetsService = new SheetsService(new BaseClientService.Initializer() { HttpClientInitializer = _credential }))
            {
                var updateRequest = sheetsService.Spreadsheets.Values.Update(valueRange, googleSpreadsheetIdentifier, range);

                updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;

                return await updateRequest.ExecuteAsync();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="googleSpreadsheetIdentifier"></param> google sheet ID
        /// <param name="keyRange"></param>  var range = $"{sheetName}!A:A"; // Assuming the primary key is in column A
        /// <returns></returns>
        public int? FindRowByKey(string googleSpreadsheetIdentifier, string keyRange, string primaryKey)
        {
            if (string.IsNullOrEmpty(googleSpreadsheetIdentifier))
                throw new ArgumentNullException(nameof(googleSpreadsheetIdentifier));

            if (string.IsNullOrEmpty(keyRange))
                throw new ArgumentNullException(nameof(keyRange));

            using (var sheetsService = new SheetsService(new BaseClientService.Initializer() { HttpClientInitializer = _credential }))
            {
                var getValueRequest = sheetsService.Spreadsheets.Values.Get(googleSpreadsheetIdentifier, keyRange);

                var response = getValueRequest.Execute();

                if (response != null && response.Values != null)
                {
                    for (int i = 0; i < response.Values.Count; i++)
                    {
                        if (response.Values[i][0].ToString() == primaryKey)
                        {
                            // Return the row index, adding 1 because Sheets is -1 based
                            return i +1;
                        }
                    }
                }

            }


            return null; // Primary key not found
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="googleSpreadsheetIdentifier"></param>
        /// <param name="sheetRange"></param>   var range = $"{sheetName}!A{row}:D{row}"; // Adjust the range according to how many columns you want to update
        /// <param name="values"></param>
        public void UpdateRow(string googleSpreadsheetIdentifier, string sheetRange,IList<object> values)
        {
            var valueRange = new ValueRange { Values = new List<IList<object>> { values } };

            using (var sheetsService = new SheetsService(new BaseClientService.Initializer() { HttpClientInitializer = _credential }))
            {
                var updateRequest = sheetsService.Spreadsheets.Values.Update(valueRange, googleSpreadsheetIdentifier, sheetRange);

                updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;

                updateRequest.ExecuteAsync();
            }
        }

        public void DeleteRow(string googleSpreadsheetIdentifier,int sheetId,int rowIndex )
        {
            var deleteRequest = new Request()
            {
                DeleteDimension = new DeleteDimensionRequest()
                {
                    Range = new DimensionRange()
                    {
                        SheetId = sheetId,  // You need to know the Sheet ID.
                        Dimension = "ROWS",
                        StartIndex = rowIndex,  // Index of the row to delete.
                        EndIndex = rowIndex + 1  // The end index is exclusive.
                    }
                }
             };

            var batchUpdateRequest = new BatchUpdateSpreadsheetRequest()
            {
                Requests = new List<Request> { deleteRequest }
            };

            try
            {
                using (var sheetsService = new SheetsService(new BaseClientService.Initializer() { HttpClientInitializer = _credential }))
                {

                    //var deleteBatchRequest = sheetsService.Spreadsheets.BatchUpdate(batchUpdateRequest, googleSpreadsheetIdentifier);

                    //BatchUpdateSpreadsheetResponse batchUpdateResponse = deleteBatchRequest.Execute();
                    SpreadsheetsResource.BatchUpdateRequest Deletion = new SpreadsheetsResource.BatchUpdateRequest(sheetsService, batchUpdateRequest, googleSpreadsheetIdentifier);
                    var batchUpdateResponse =Deletion.Execute();
                    Console.Write(batchUpdateResponse);
                }
            }catch(Exception ex)
            {
                Console.Write(ex.StackTrace);
                throw ex;
            }
        }
    }
}
