using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AMMS.Infrastructure.Entities;

[Table("order_items", Schema = "AMMS_DB")]
public partial class order_item
{
    public int item_id { get; set; }

    public int? order_id { get; set; }

    public string? product_name { get; set; }

    public int quantity { get; set; }

    public string? paper_code { get; set; }

    public string? paper_name { get; set; }

    public string? glue_type { get; set; }

    public string? wave_type { get; set; }

    public int? est_paper_sheets_total { get; set; }

    public decimal? est_ink_weight_kg { get; set; }

    public decimal? est_coating_glue_weight_kg { get; set; }

    public decimal? est_mounting_glue_weight_kg { get; set; }

    public decimal? est_lamination_weight_kg { get; set; }

    public string? paper_type { get; set; }

    public string? post_processing { get; set; }

    public string? design_url { get; set; }

    public int? product_type_id { get; set; }

    public virtual ICollection<bom> boms { get; set; } = new List<bom>();

    public virtual order? order { get; set; }

    public virtual product_type? product_type { get; set; }
}
