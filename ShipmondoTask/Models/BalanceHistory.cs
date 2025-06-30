using System;

namespace ShipmondoTask.Models;

public class BalanceHistory
{
    public int Id { get; set; }
    public decimal Balance { get; set; }
    public DateTime RetrievedAt { get; set; } = DateTime.Now;
}
