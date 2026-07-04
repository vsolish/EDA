using System;
using System.Collections.Generic;
using System.Text;

namespace Hotel.Payments.Infrastructure.Configuration;

public class DatabaseOptions
{
    public string ConnectionString { get; set; } = string.Empty;
}