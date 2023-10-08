public class Price{
    public float Value {get; set;}
    public string Date {get; set;}

    public Price(string date, float value){
        Value = value;
        Date = date;
    }
}