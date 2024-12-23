using System;
using System.Collections.Generic;

namespace CandyStoreApp.Models;

public partial class Review
{
    public int IdReview { get; set; }

    public int? IdClient { get; set; }

    public int? IdProduct { get; set; }

    public int? Rating { get; set; }

    public string? ReviewText { get; set; }

    public DateTime? ReviewDate { get; set; }

    public virtual Client? IdClientNavigation { get; set; }

    public virtual Product? IdProductNavigation { get; set; }
}
