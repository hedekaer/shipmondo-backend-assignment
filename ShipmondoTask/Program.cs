using ShipmondoTask.Data;
using ShipmondoTask.Services;


using var db = new DataContext();
db.Database.EnsureCreated();

Console.WriteLine("=== Shipmondo Shipment CLI ===");

// Replace with your actual sandbox API key
var apiKey = "3f364daa-5abe-4852-bbd4-1370e0db6274:9f8ae772-2f68-4958-923e-bacf15616681";
var service = new ShipmondoService(db, apiKey);
Console.WriteLine(DateTime.Now);
while (true)
{
    Console.WriteLine("\nChoose an action:");
    Console.WriteLine("1. Fetch current balance");
    Console.WriteLine("2. Create a shipment");
    Console.WriteLine("3. View balance history");
    Console.WriteLine("4. View all shipments");
    Console.WriteLine("5. Exit");

    Console.Write("> ");
    var choice = Console.ReadLine();

    switch (choice)
    {
        case "1":
            var balance = await service.GetBalanceAsync();
            Console.WriteLine($"Current balance: {balance} DKK");
            break;

        case "2":
            try
            {
                await service.CreateShipmentAsync();
                Console.WriteLine("Shipment created successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating shipment: {ex.Message}");
            }
            break;

        case "3":
            var balances = db.Balances.OrderByDescending(b => b.RetrievedAt).ToList();
            if (!balances.Any()) Console.WriteLine("No balance history.");
            else
            {
                Console.WriteLine("Balance history:");
                foreach (var b in balances)
                    Console.WriteLine($"{b.RetrievedAt:u} — {b.Balance} DKK");
            }
            break;

        case "4":
            var shipments = db.Shipments.OrderByDescending(s => s.CreatedAt).ToList();
            if (!shipments.Any()) Console.WriteLine("No shipments found.");
            else
            {
                Console.WriteLine("Shipments:");
                foreach (var s in shipments)
                    Console.WriteLine($"{s.CreatedAt:u} — ID: {s.ShipmentApiId}, Package: {s.PackageNumber}");
            }
            break;

        case "5":
            Console.WriteLine("Goodbye!");
            return;

        default:
            Console.WriteLine("Invalid option. Try again.");
            break;
    }
}
