using System;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using ShipmondoTask.Data;
using ShipmondoTask.Models;

namespace ShipmondoTask.Services;

public class ShipmondoService
{
    private readonly HttpClient _httpClient = new();
    private readonly DataContext _db;

    public ShipmondoService(DataContext db, string apiKey)
    {
        _db = db;
        var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{apiKey}"));
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        _httpClient.BaseAddress = new Uri("https://sandbox.shipmondo.com/api/public/v3/");
    }

    public async Task<decimal> GetBalanceAsync()
    {
        var res = await _httpClient.GetAsync("account/balance");
        res.EnsureSuccessStatusCode();

        var json = await res.Content.ReadAsStringAsync();
        var doc = JsonDocument.Parse(json);
        var balance = doc.RootElement.GetProperty("amount").GetDecimal();

        _db.Balances.Add(new BalanceHistory { Balance = balance });
        await _db.SaveChangesAsync();

        return balance;
    }

    public async Task CreateShipmentAsync()
    {

        var body = new
        {
            own_agreement = false,
            label_format = "a4_pdf",
            product_code = "GLSDK_SD",
            service_codes = "EMAIL_NT,SMS_NT",
            reference = "Order 10001",
            automatic_select_service_point = true,
            sender = new
            {
                name = "Min Virksomhed ApS",
                attention = "Lene Hansen",
                address1 = "Hvilehøjvej 25",
                address2 = (string?)null,
                zipcode = "5220",
                city = "Odense SØ",
                country_code = "DK",
                email = "info@minvirksomhed.dk",
                mobile = "70400407"
            },
            receiver = new
            {
                name = "Lene Hansen",
                attention = (string?)null,
                address1 = "Skibhusvej 52",
                address2 = (string?)null,
                zipcode = "5000",
                city = "Odense C",
                country_code = "DK",
                email = "lene@email.dk",
                mobile = "12345678"
            },
            parcels = new[]
            {
                new { weight = 1000 }
            }
        };



        var content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
        var res = await _httpClient.PostAsync("shipments", content);
        res.EnsureSuccessStatusCode();

        var json = await res.Content.ReadAsStringAsync();
        var doc = JsonDocument.Parse(json);
        var shipmentId = doc.RootElement.GetProperty("id").GetInt32()!;
        var packageNumber = doc.RootElement.GetProperty("pkg_no").GetString()!;

        _db.Shipments.Add(new Shipment
        {
            ShipmentApiId = shipmentId,
            PackageNumber = packageNumber
        });

        await _db.SaveChangesAsync();
        await GetBalanceAsync();
    }
}
