using DsBot.Models;

namespace DsBot.Models;

public class Balance
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    public int CurrencyId { get; set; }
    public Currency Currency { get; set; }
    public double Amount { get; set; }
}
