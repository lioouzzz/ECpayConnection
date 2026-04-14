public class CreateOrderInputModel
{
    public int TotalAmount { get; set; }
    public string TradeDesc { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
}