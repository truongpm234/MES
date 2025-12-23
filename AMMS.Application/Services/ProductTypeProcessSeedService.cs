using AMMS.Application.Interfaces;
using AMMS.Infrastructure.Entities;
using AMMS.Infrastructure.Interfaces;
using AMMS.Shared.DTOs.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMMS.Application.Services
{
    public class ProductTypeProcessSeedService : IProductTypeProcessSeedService
    {
        private readonly IProductTypeRepository _productTypeRepo;
        private readonly IProductTypeProcessRepository _ptpRepo;
        private readonly IMachineRepository _machineRepo;

        public ProductTypeProcessSeedService(
            IProductTypeRepository productTypeRepo,
            IProductTypeProcessRepository ptpRepo,
            IMachineRepository machineRepo)
        {
            _productTypeRepo = productTypeRepo;
            _ptpRepo = ptpRepo;
            _machineRepo = machineRepo;
        }

        public async Task SeedAsync()
        {
            // load machines active
            // (LINQ inside repo; repo trả list)
            var machines = await _machineRepo.GetActiveMachinesAsync();

            string? ResolveMachineCode(string processDisplay)
            {
                // chọn machine đầu tiên match process_name (case-insensitive)
                var m = machines.FirstOrDefault(x =>
                    string.Equals(x.process_name.Trim(), processDisplay.Trim(), StringComparison.OrdinalIgnoreCase));

                return m?.machine_code;
            }

            foreach (var kv in RoutingDefinitions.Routing)
            {
                var code = kv.Key.ToString();
                var pt = await _productTypeRepo.GetByCodeAsync(code);
                if (pt == null) continue;

                await _ptpRepo.DeleteAllByProductTypeIdAsync(pt.product_type_id);

                var list = new List<product_type_process>();
                int seq = 1;

                foreach (var p in kv.Value)
                {
                    var display = RoutingDefinitions.ProcessDisplay[p];
                    var machineCode = ResolveMachineCode(display);

                    list.Add(new product_type_process
                    {
                        product_type_id = pt.product_type_id,
                        seq_num = seq++,
                        process_name = display,
                        machine = machineCode,
                        is_active = true
                    });
                }

                await _ptpRepo.AddRangeAsync(list);
                await _ptpRepo.SaveChangesAsync();
            }
        }
    }
}
