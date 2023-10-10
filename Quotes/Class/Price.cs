public class Price{
    public Decimal Value {get; set;}
    public string Date {get; set;}

    public Price(string date, Decimal value){
        Value = value;
        Date = date;
    }
}