using System;
using System.Collections.Generic;

namespace CandyStoreApp.Models;

public partial class Shipment
{
    public int IdShipment { get; set; }

    public int? IdOrder { get; set; }

    public DateTime? ShipmentDate { get; set; }

    public string? ShipmentMethod { get; set; }

    public string? TrackingNumber { get; set; }

    public virtual Order? IdOrderNavigation { get; set; }
}
