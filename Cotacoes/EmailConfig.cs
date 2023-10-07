public class EmailConfig
{
  public string To { get; set; } = string.Empty;
  public string From { get; set; } = string.Empty;
  public string Server { get; set; } = string.Empty;
  public int Port { get; set; } = 0;
  public string Username { get; set; } = string.Empty;
  public string Password { get; set; } = string.Empty;
  public string Sender { get; set; } = string.Empty;
  public string Receiver { get; set; } = string.Empty;
}