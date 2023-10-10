using System.Globalization;

namespace Quotes{
    class Program{

        //In seconds
        private static int MONITORING_FREQUENCY = 1;

        static void CheckNumArgs(string[] args){
            if(args.Length != 3){
                Console.WriteLine(
                    "Parâmetros inválidos: Entre com o ativo a ser monitorado, "
                    + "o preço de referência para venda e " 
                    + "o preço de referência para compra."
                );
                Environment.Exit(1);
            }
        }

        static void CheckRefValues(Decimal salePriceRef, Decimal purchasePriceRef){
            if(salePriceRef < purchasePriceRef){
                Console.WriteLine(
                    "O preço de referência para venda " 
                    + "deve ser maior do que o preço de referência para compra."
                );
                Environment.Exit(1);
            } 
        }

        static void PrintSalesMessage(string date, string asset, Decimal value){
            Console.BackgroundColor = ConsoleColor.Yellow;
            Console.ForegroundColor = ConsoleColor.Black;

            string formattedValue = Util.FormatValue(value);
            Console.WriteLine($"[{date}]({asset}) Preço: R${formattedValue} | Agora é o momento de vender.");
            Console.ResetColor();
        }

        static void PrintPurchaseMessage(string date, string asset, Decimal value){
            Console.BackgroundColor = ConsoleColor.Yellow;
            Console.ForegroundColor = ConsoleColor.Black;

            string formattedValue =  Util.FormatValue(value);
            Console.WriteLine($"[{date}]({asset}) Preço: R${formattedValue} | Agora é o momento de comprar.");
            Console.ResetColor();
        }

        static void PrintMessage(string date, string asset, Decimal value){
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"[{date}]");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write($"({asset}) ");

            Console.ResetColor();
            string formattedValue = Util.FormatValue(value);
            Console.WriteLine($"Preço: R${formattedValue}.");
        }

        static void ClearTerminal(){
            Console.Clear();
        }

        static void Wait(){
            Thread.Sleep(MONITORING_FREQUENCY*1000);
        }

        static void Main(string[] args){
            
            CheckNumArgs(args);

            Decimal refPriceSale, refPricePurchase;
            try{
                refPriceSale = Decimal.Parse(args[1], CultureInfo.InvariantCulture);
                refPricePurchase = Decimal.Parse(args[2], CultureInfo.InvariantCulture);
            }           
            catch (FormatException){
                Console.WriteLine("Os parâmetros são inválidos.");
                return;
            }

            CheckRefValues(refPriceSale, refPricePurchase);
            ClearTerminal();
            
            try{
                string assetName = args[0];
                Asset asset = new(assetName);
                Courier courier = new(assetName);

                bool salesNoticeSent = false;
                bool purchaseNoticeSent = false;

                while(true){
                    Decimal value = asset.Price.Value;
                    string date = asset.Price.Date;

                    if(refPriceSale < value){
                        PrintSalesMessage(date, assetName, value);
                        if(!salesNoticeSent){
                            courier.SendEmailForSale(value);
                            salesNoticeSent = true;
                            purchaseNoticeSent = false;
                        } 
                    }else if(refPricePurchase > value){
                        PrintPurchaseMessage(date, assetName, value);
                        if(!purchaseNoticeSent){
                            courier.SendEmailForPurchase(value);
                            purchaseNoticeSent = true;
                            salesNoticeSent = false;
                        }
                    }else{
                        PrintMessage(date, assetName, value);
                        salesNoticeSent = false;
                        purchaseNoticeSent = false;                      
                    }
                    
                    Wait();
                    
                }
            }catch(KeyNotFoundException e){
                Console.WriteLine(e.Message);
            }catch(FormatException e){
                Console.WriteLine(e.Message);
            }catch(Exception e){
                Console.WriteLine(e.Message);
            }            
        }
    }
}