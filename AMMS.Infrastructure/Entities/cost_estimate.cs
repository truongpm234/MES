using System;
using System.Collections.Generic;

namespace AMMS.Infrastructure.Entities;

public partial class cost_estimate
{
    public int estimate_id { get; set; }

    public int order_request_id { get; set; }

    public decimal base_cost { get; set; }

    public bool is_rush { get; set; }

    public decimal rush_percent { get; set; }

    public decimal rush_amount { get; set; }

    public decimal system_total_cost { get; set; }

    public DateTime estimated_finish_date { get; set; }

    public DateTime desired_delivery_date { get; set; }

    public DateTime created_at { get; set; }

    public virtual order_request order_request { get; set; } = null!;
}
