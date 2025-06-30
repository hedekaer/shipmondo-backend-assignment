using System;
using Microsoft.EntityFrameworkCore;
using ShipmondoTask.Models;

namespace ShipmondoTask.Data;

public class DataContext : DbContext
{
    public DbSet<Shipment> Shipments => Set<Shipment>();
    public DbSet<BalanceHistory> Balances => Set<BalanceHistory>();

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite("Data Source=shipmondo.db");
}
