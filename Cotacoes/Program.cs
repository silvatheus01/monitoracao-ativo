
using System.Globalization;

namespace Cotacoes{
    class Program{
        static void Main(string[] args){
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
            
            string assetName = args[0];
            Asset asset = new(assetName);
            Courier courier = new(assetName);

            try{
                bool salesNoticeSent = false;
                bool purchaseNoticeSent = false;

                while(true){
                    float price = asset.Price;
                    if(refPriceSale < price){
                        Console.WriteLine($"({assetName}) Preço: {price} | Agora é o momento de vender.");
                        if(!salesNoticeSent){
                            courier.SendEmailForSale(price);
                            salesNoticeSent = true;
                            purchaseNoticeSent = false;
                        } 
                    }else if(refPricePurchase > price){
                        Console.WriteLine($"({assetName}) Preço: {price} | Agora é o momento de comprar.");
                        if(!purchaseNoticeSent){
                            courier.SendEmailForPurchase(price);
                            purchaseNoticeSent = true;
                            salesNoticeSent = false;
                        }
                    }else{
                        Console.WriteLine($"({assetName}) Preço: {price}");  
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
    }
}