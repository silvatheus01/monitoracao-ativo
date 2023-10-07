
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;

namespace Cotacoes{
    class Program{
        static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
        static readonly string ApplicationName = "Dot Tutorials";
        static readonly string sheet = "pagina";
        static readonly string SpreadsheetId = "1uypMIKinfs1VS8p2U6y-zzQqqnrPfPkc6V-WTOc1CyI";
        static SheetsService service;

        static void Main(string[] args){
            Init();
            try{
                int rowIndex = FindAssetRow("SOND3");
                float price = GetPriceAsset(rowIndex);
                Console.WriteLine(price);
            }catch(KeyNotFoundException e){
                Console.WriteLine(e.Message);
            }catch(FormatException e){
                Console.WriteLine(e.Message);
            }            
        }

        static void Init(){
            GoogleCredential credential;
            //Reading Credentials File...
            using (var stream = new FileStream("../app_client_secret.json", FileMode.Open, FileAccess.Read)){
                credential = GoogleCredential.FromStream(stream)
                    .CreateScoped(Scopes);
            }
            // Creating Google Sheets API service...
            service = new SheetsService(new BaseClientService.Initializer(){
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
        }   

        static IList<IList<object>> GetValues(string range){
            SpreadsheetsResource.ValuesResource.GetRequest request =
                service.Spreadsheets.Values.Get(SpreadsheetId, range);
            var response = request.Execute();
            return response.Values;
        }

        static float GetPriceAsset(int rowIndex){
            string range = $"{sheet}!B{rowIndex}:B{rowIndex}";
            IList<IList<object>> values = GetValues(range);

            string tempPrice = (string) values[0][0];
            float price = 0;
            try{
                price = Single.Parse(tempPrice);
            }catch(FormatException){
                throw new FormatException("Não possível retornar o preço do ativo");
            }
        
            return price;
        }

        static int FindAssetRow(string asset){
            var range = $"{sheet}!A:A";
            IList<IList<object>> values = GetValues(range);
            int numLines = values.Count;

            if (values != null && numLines > 0){
                for(int i = 0; i < numLines; i++){
                   var tempAsset = values[i][0];
                   if(asset.CompareTo(tempAsset) == 0){
                    return i+1;
                   }
                }
            }
            
            throw new KeyNotFoundException("Não foi possível encontrar o ativo.");
        }   
    }
}



