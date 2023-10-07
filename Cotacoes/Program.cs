
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using System.Globalization;
using MailKit.Net.Smtp;
using MimeKit;
using System.Text.Json;

namespace Cotacoes{
    class Program{
        static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
        static readonly string ApplicationName = "InoaPs";
        static readonly string sheet = "pagina";
        static readonly string SpreadsheetId = "1uypMIKinfs1VS8p2U6y-zzQqqnrPfPkc6V-WTOc1CyI";
        static SheetsService service = new();

        static void Main(string[] args){
            Init();
            if(args.Length != 3){
                Console.WriteLine("Parâmetros inválidos: Entre com o ativo a ser monitorado, "
                    + "o preço de referência para venda e " 
                    + "o preço de referência para compra."
                );
                return;
            }

            float refPriceSale, refPricePurchase;
            try{
                refPriceSale = float.Parse(args[1], CultureInfo.InvariantCulture);
                refPricePurchase = float.Parse(args[2], CultureInfo.InvariantCulture);
            }
            catch (FormatException){
                Console.WriteLine("Os parâmetros são inválidos.");
                return;
            }

            string asset = args[0];

            try{
                int rowIndex = FindAssetRow(asset);
                bool salesNoticeSent = false;
                bool purchaseNoticeSent = false;

                while(true){
                    float price = GetPriceAsset(rowIndex);
                    if(refPriceSale < price){
                        Console.WriteLine($"({asset}) Preço: {price} | Agora é o momento de vender.");
                        if(!salesNoticeSent){
                            SendEmail(asset, price, true);
                            salesNoticeSent = true;
                            purchaseNoticeSent = false;
                        } 
                    }else if(refPricePurchase > price){
                        Console.WriteLine($"({asset}) Preço: {price} | Agora é o momento de comprar.");
                        if(!purchaseNoticeSent){
                            SendEmail(asset, price, false);
                            purchaseNoticeSent = true;
                            salesNoticeSent = false;
                        }
                    }else{
                        Console.WriteLine($"({asset}) Preço: {price}");  
                        salesNoticeSent = false;
                        purchaseNoticeSent = false;                      
                    }

                    Thread.Sleep(1000);
                }
            }catch(KeyNotFoundException e){
                Console.WriteLine(e.Message);
            }catch(FormatException e){
                Console.WriteLine(e.Message);
            }            
        }

        static void SendEmail(
            string asset, 
            float currentPrice, 
            bool isSale
        ){
            string text = File.ReadAllText("../email_config.json");
            var config = JsonSerializer.Deserialize<EmailConfig>(text);
            
            if(config != null){
                var email = new MimeMessage();
                email.From.Add(new MailboxAddress(config.Sender, config.From));
                email.To.Add(new MailboxAddress(config.Receiver, config.To));

                email.Subject = "Alerta de preço";
                string body;
                if(isSale){
                    body = $"O preço de <b>{asset}</b> é de <b>{currentPrice}</b>. Esse é o momento de vender.";
                }else{
                    body = $"O preço de <b>{asset}</b> é de <b>{currentPrice}</b>. Esse é o momento de comprar.";
                }

                email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { 
                    Text = body
                };
                
                using var smtp = new SmtpClient();
                smtp.Connect(config.Server, config.Port, false);

                // Note: only needed if the SMTP server requires authentication
                string username = config.Username;
                string password = config.Password;
                if(username != string.Empty && password != string.Empty){
                    smtp.Authenticate(config.Username, config.Password);
                }
                    
                smtp.Send(email);
                smtp.Disconnect(true);
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
            float price;
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