using AMMS.Shared.DTOs.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMMS.Shared.DTOs.Suppliers { 
    public record SupplierDetailDto(
        int SupplierId,
        string Name,
        string? ContactPerson,
        string? Phone,
        string? Email,
        string? MainMaterialType,
        PagedResultLite<SupplierMaterialDto> Materials
    );
}
