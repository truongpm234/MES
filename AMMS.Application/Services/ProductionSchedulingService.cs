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

        /// <summary>
        /// Công đoạn thủ công nếu:
        /// - note của machine có chứa "thủ công" / "thu cong"
        /// - hoặc không tìm thấy machine
        /// - hoặc routing step không khai machine (null/empty)
        /// </summary>
        private static bool IsManual(machine? m, string? stepMachine)
        {
            if (string.IsNullOrWhiteSpace(stepMachine)) return true; // routing không gán máy => thủ công

            if (m == null) return true; // routing có process nhưng DB machines chưa cấu hình => coi là thủ công để không crash

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
            await _prodRepo.SaveChangesAsync(); // để có prod.prod_id

            // 2) load routing steps
            var steps = await _ptpRepo.GetActiveByProductTypeIdAsync(productTypeId);
            if (steps == null || steps.Count == 0)
                throw new Exception("No routing (product_type_process) found. Seed first.");

            // 3) create tasks
            var tasks = new List<task>();
            var firstSeq = steps.Min(x => x.seq_num); // an toàn hơn First()

            foreach (var s in steps.OrderBy(x => x.seq_num))
            {
                // s.machine ở đây là PROCESS_NAME (IN, BE, DAN...) hoặc null nếu thủ công
                machine? m = null;

                if (!string.IsNullOrWhiteSpace(s.machine))
                {
                    // tìm 1 máy theo process (ưu tiên theo repo bạn implement)
                    m = await _machineRepo.FindMachineByProcess(s.machine);
                }

                var printersNeeded = m?.quantity ?? 1;
                var manual = IsManual(m, s.machine);

                var status = s.seq_num == firstSeq ? "Ready" : "Unassigned";

                // noteSuffix an toàn tuyệt đối (không dùng m!)
                var noteSuffix = manual
                    ? $"(Thủ công - cần {printersNeeded} Printer)"
                    : m != null
                        ? $"(Máy {m.machine_code} - cần {printersNeeded} Printer)"
                        : $"(Máy cho công đoạn {s.machine} chưa cấu hình - cần {printersNeeded} Printer)";

                // ✅ Task.machine nên lưu MACHINE_CODE (IN-01, BE-TAY-01...), không lưu process_name
                var taskMachine = manual ? null : m?.machine_code;

                tasks.Add(new task
                {
                    prod_id = prod.prod_id,
                    process_id = s.process_id,
                    seq_num = s.seq_num,
                    name = $"{s.process_name} {noteSuffix}",
                    status = status,
                    machine = taskMachine,   // ✅ sửa: lưu machine_code hoặc null
                    assigned_to = null,
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
