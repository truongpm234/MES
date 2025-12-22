using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMMS.Shared.DTOs.Suppliers
{
    public record SupplierDto(
        int SupplierId,
        string Name,
        string? ContactPerson,
        string? Phone,
        string? Email,
        string? MainMaterialType
    );
}
