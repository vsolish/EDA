using System;
using System.Collections.Generic;
using System.Text;

namespace Hotel.Payments.Infrastructure.Configuration;

public class ServiceUrlsOptions
{
    public string ReservationApi { get; set; } = "https://localhost:5000";
    public string InventoryApi { get; set; } = "https://localhost:5001";
}