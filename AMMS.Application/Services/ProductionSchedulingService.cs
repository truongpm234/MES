using AMMS.Application.Interfaces;
using AMMS.Infrastructure.Entities;
using AMMS.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMMS.Application.Services
{
    public class ProductionSchedulingService : IProductionSchedulingService
    {
        private readonly IProductionRepository _prodRepo;
        private readonly IProductTypeProcessRepository _ptpRepo;
        private readonly IMachineRepository _machineRepo;
        private readonly ITaskRepository _taskRepo;

        public ProductionSchedulingService(
            IProductionRepository prodRepo,
            IProductTypeProcessRepository ptpRepo,
            IMachineRepository machineRepo,
            ITaskRepository taskRepo)
        {
            _prodRepo = prodRepo;
            _ptpRepo = ptpRepo;
            _machineRepo = machineRepo;
            _taskRepo = taskRepo;
        }

        private static bool IsManual(machine? m, string? stepMachine)
        {
            if (string.IsNullOrWhiteSpace(stepMachine)) return true; 

            if (m == null) return true; 

            var note = (m.note ?? "");
            return note.Contains("thủ công", StringComparison.OrdinalIgnoreCase)
                || note.Contains("thu cong", StringComparison.OrdinalIgnoreCase);
        }

        public async Task<int> ScheduleOrderAsync(int orderId, int productTypeId, int? managerId = null)
        {
            // 1) create production
            var prod = new production
            {
                code = $"PRD-{DateTime.UtcNow:yyyyMMddHHmmss}",
                order_id = orderId,
                manager_id = managerId,
                status = "Scheduled",
                product_type_id = productTypeId,
                start_date = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
            };

            await _prodRepo.AddAsync(prod);
            await _prodRepo.SaveChangesAsync(); 

            // 2) load routing steps
            var steps = await _ptpRepo.GetActiveByProductTypeIdAsync(productTypeId);
            if (steps == null || steps.Count == 0)
                throw new Exception("No routing (product_type_process) found. Seed first.");

            // 3) create tasks
            var tasks = new List<task>();
            var firstSeq = steps.Min(x => x.seq_num);

            foreach (var s in steps.OrderBy(x => x.seq_num))
            {
                machine? m = null;

                if (!string.IsNullOrWhiteSpace(s.machine))
                {
                    m = await _machineRepo.FindMachineByProcess(s.machine);
                }

                var printersNeeded = m?.quantity ?? 1;
                var manual = IsManual(m, s.machine);

                var status = s.seq_num == firstSeq ? "Ready" : "Unassigned";

                var noteSuffix = manual
                    ? $"(Thủ công - cần {printersNeeded} Printer)"
                    : m != null
                        ? $"(Máy {m.machine_code} - cần {printersNeeded} Printer)"
                        : $"(Máy cho công đoạn {s.machine} chưa cấu hình - cần {printersNeeded} Printer)";

                var taskMachine = manual ? null : m?.machine_code;

                tasks.Add(new task
                {
                    prod_id = prod.prod_id,
                    process_id = s.process_id,
                    seq_num = s.seq_num,
                    name = $"{s.process_name} {noteSuffix}",
                    status = status,
                    machine = taskMachine,   
                    start_time = null,
                    end_time = null
                });
            }

            await _taskRepo.AddRangeAsync(tasks);
            await _taskRepo.SaveChangesAsync();

            return prod.prod_id;
        }
    }
}
