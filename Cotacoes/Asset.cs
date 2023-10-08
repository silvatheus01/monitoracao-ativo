using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;

public class Asset
{ 
    public Price Price {
        get{
            return GetPrice(RowIndex);
        }
    }

    private int RowIndex {get; set;}
    static readonly private string pathCredentialsFile = "../app_client_secret.json";
    static readonly private string assetColumn = "A";
    static readonly private string priceColumn = "B";
    static readonly private string dateColumn = "C";
    static readonly private string[] Scopes = { SheetsService.Scope.Spreadsheets };
    static readonly private string ApplicationName = "InoaPs";
    static readonly private string sheet = "pagina";
    static readonly private string SpreadsheetId = "1uypMIKinfs1VS8p2U6y-zzQqqnrPfPkc6V-WTOc1CyI";
    static private SheetsService  service = new();

    public Asset(string name){
        Init();
        RowIndex = FindRow(name);
    }

    private static void Init(){
        GoogleCredential credential;
        //Reading Credentials File...
        using (var stream = new FileStream(pathCredentialsFile, FileMode.Open, FileAccess.Read)){
            credential = GoogleCredential.FromStream(stream)
                .CreateScoped(Scopes);
        }
        // Creating Google Sheets API service...
        service = new SheetsService(new BaseClientService.Initializer(){
            HttpClientInitializer = credential,
            ApplicationName = ApplicationName,
        });
    }

    private IList<IList<object>> GetValues(string range){
        SpreadsheetsResource.ValuesResource.GetRequest request =
            service.Spreadsheets.Values.Get(SpreadsheetId, range);
        var response = request.Execute();
        return response.Values;
    }

    private Price GetPrice(int rowIndex){
        string range = $"{sheet}!{priceColumn}{rowIndex}:{dateColumn}{rowIndex}";
        IList<IList<object>> values = GetValues(range);

        string tempValue = (string) values[0][0];
        float value;
        try{
            value = Single.Parse(tempValue);
        }catch(FormatException){
            throw new FormatException("Não possível retornar o preço do ativo");
        }

        string date = (string) values[0][1];
        Price price = new(date, value);
        
        return price;
    }

    private int FindRow(string asset){
        var range = $"{sheet}!{assetColumn}:{assetColumn}";
        IList<IList<object>> values = GetValues(range);
        int numLines = values.Count;

        if (values != null && numLines > 0){
            for(int i = 0; i < numLines; i++){
                var TempAsset = values[i][0];
                if(asset.CompareTo(TempAsset) == 0){
                    return i+1;
                }

            }
        }
        
        throw new KeyNotFoundException("Não foi possível encontrar o ativo.");
    }  
}