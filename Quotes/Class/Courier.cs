using MailKit.Net.Smtp;
using MimeKit;
using System.Text.Json;

public class Courier{
    private string Asset {get; set;}
    private EmailConfig Config {get; set;} 

    static readonly private string PATH_CONFIG_PATH = "./email_config.json"; 

    public Courier(string asset){
        Asset = asset;
        Config = GetConfig();
    }

    private EmailConfig GetConfig(){
        string errorMsg = "A configuração de envio de email não estão disponível.";
        try{
            string text = File.ReadAllText(PATH_CONFIG_PATH);
            var config = JsonSerializer.Deserialize<EmailConfig>(text);
            if(config != null) return config;
            else throw new Exception(errorMsg);
        }
        catch (Exception){
            throw new Exception(errorMsg);
        }       
    }

    public void SendEmailForSale(Decimal currentPrice){
        SendEmail(currentPrice, true);
    }

    public void SendEmailForPurchase(Decimal currentPrice){
        SendEmail(currentPrice, false);
    }

    private void SendEmail(Decimal currentValue, bool isSale){            
        var email = new MimeMessage();
        email.From.Add(new MailboxAddress(Config.Sender, Config.From));
        email.To.Add(new MailboxAddress(Config.Receiver, Config.To));

        email.Subject = "Alerta de preço";
        string body;

        string formattedValue = Util.FormatValue(currentValue);
        if(isSale){
            body = $"O preço de <b>{Asset}</b> é de <b>R${formattedValue}</b>. Esse é o momento de vender.";
        }else{
            body = $"O preço de <b>{Asset}</b> é de <b>R${formattedValue}</b>. Esse é o momento de comprar.";
        }

        email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { 
            Text = body
        };
        
        using var smtp = new SmtpClient();
        smtp.Connect(Config.Server, Config.Port, false);

        // Note: only needed if the SMTP server requires authentication
        string username = Config.Username;
        string password = Config.Password;
        if(username != string.Empty && password != string.Empty){
            smtp.Authenticate(Config.Username, Config.Password);
        }
            
        smtp.Send(email);
        smtp.Disconnect(true);     
    }
}