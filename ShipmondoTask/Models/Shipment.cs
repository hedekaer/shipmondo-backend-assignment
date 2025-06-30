using System;

namespace ShipmondoTask.Models;

public class Shipment
{
    public int Id { get; set; }
    public string PackageNumber { get; set; } = string.Empty;
    public int ShipmentApiId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
