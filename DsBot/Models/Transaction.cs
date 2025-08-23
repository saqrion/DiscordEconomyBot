﻿using DsBot.Models;


namespace DsBot.Models;

public class Transaction
{
    public int Id { get; set; }
    public int? FromUserId { get; set; }
    public int ToUserId { get; set; }
    public decimal Amount { get; set; }
    public string? Reason { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User? FromUser { get; set; }
    public User ToUser { get; set; } = null!;
}