using ClosedXML.Excel;
using SD_Ajans.Core.Entities;
using SD_Ajans.Core.Repositories;
using SD_Ajans.Business.Services;

namespace SD_Ajans.Business.Services
{
    public class ReportService : IReportService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAccountingService _accountingService;

        public ReportService(IUnitOfWork unitOfWork, IAccountingService accountingService)
        {
            _unitOfWork = unitOfWork;
            _accountingService = accountingService;
        }

        public async Task<byte[]> GenerateOrganizationReportAsync(int organizationId)
        {
            var organization = await _unitOfWork.Organizations.GetByIdAsync(organizationId);
            if (organization == null)
                return new byte[0];

            var assignments = await _unitOfWork.Assignments.GetAllAsync(a => a.OrganizationId == organizationId);

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Organizasyon Raporu");

            worksheet.Cell("A1").Value = "SD AJANS - ORGANİZASYON RAPORU";
            worksheet.Cell("A1").Style.Font.FontSize = 16;
            worksheet.Cell("A1").Style.Font.Bold = true;
            worksheet.Range("A1:F1").Merge();

            worksheet.Cell("A3").Value = "Organizasyon Adı:";
            worksheet.Cell("B3").Value = organization.Name;
            worksheet.Cell("A4").Value = "Tarih:";
            worksheet.Cell("B4").Value = organization.Date.ToShortDateString();
            worksheet.Cell("A5").Value = "Konum:";
            worksheet.Cell("B5").Value = organization.Location;

            worksheet.Cell("A7").Value = "Görevlendirme Geçmişi";
            worksheet.Cell("A7").Style.Font.Bold = true;
            worksheet.Range("A7:F7").Merge();

            worksheet.Cell("A8").Value = "Manken";
            worksheet.Cell("B8").Value = "Tarih";
            worksheet.Cell("C8").Value = "Durum";
            worksheet.Cell("D8").Value = "Günlük Ücret";
            worksheet.Cell("E8").Value = "Toplam Ücret";
            worksheet.Cell("F8").Value = "Ödeme Durumu";

            int row = 9;
            foreach (var assignment in assignments)
            {
                var manken = await _unitOfWork.Mankens.GetByIdAsync(assignment.MankenId);
                var payment = await _unitOfWork.Payments.GetAllAsync(p => p.AssignmentId == assignment.Id);
                
                if (manken != null)
                {
                    worksheet.Cell($"A{row}").Value = $"{manken.FirstName} {manken.LastName}";
                    worksheet.Cell($"B{row}").Value = assignment.StartTime.ToShortDateString();
                    worksheet.Cell($"C{row}").Value = GetStatusName(assignment.Status);
                    worksheet.Cell($"D{row}").Value = assignment.DailyRate;
                    worksheet.Cell($"E{row}").Value = assignment.TotalPayment;
                    worksheet.Cell($"F{row}").Value = payment.Any() ? GetPaymentStatusName(payment.First().Status) : "Ödeme Yok";
                    row++;
                }
            }

            worksheet.Range($"A8:F{row - 1}").Style.Font.Bold = true;
            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        public async Task<byte[]> GenerateMankenReportAsync(int mankenId)
        {
            var manken = await _unitOfWork.Mankens.GetByIdAsync(mankenId);
            if (manken == null)
                return new byte[0];

            var assignments = await _unitOfWork.Assignments.GetAllAsync(a => a.MankenId == mankenId);

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Manken Raporu");

            worksheet.Cell("A1").Value = "SD AJANS - MANKEN RAPORU";
            worksheet.Cell("A1").Style.Font.FontSize = 16;
            worksheet.Cell("A1").Style.Font.Bold = true;
            worksheet.Range("A1:E1").Merge();

            worksheet.Cell("A3").Value = "Ad Soyad:";
            worksheet.Cell("B3").Value = $"{manken.FirstName} {manken.LastName}";
            worksheet.Cell("A4").Value = "Email:";
            worksheet.Cell("B4").Value = manken.Email;
            worksheet.Cell("A5").Value = "Kategori:";
            worksheet.Cell("B5").Value = GetCategoryName(manken.Category);

            worksheet.Cell("A7").Value = "Görev Geçmişi";
            worksheet.Cell("A7").Style.Font.Bold = true;
            worksheet.Range("A7:E7").Merge();

            worksheet.Cell("A8").Value = "Organizasyon";
            worksheet.Cell("B8").Value = "Tarih";
            worksheet.Cell("C8").Value = "Durum";
            worksheet.Cell("D8").Value = "Günlük Ücret";
            worksheet.Cell("E8").Value = "Toplam Ücret";

            int row = 9;
            foreach (var assignment in assignments)
            {
                var organization = await _unitOfWork.Organizations.GetByIdAsync(assignment.OrganizationId);
                if (organization != null)
                {
                    worksheet.Cell($"A{row}").Value = organization.Name;
                    worksheet.Cell($"B{row}").Value = assignment.StartTime.ToShortDateString();
                    worksheet.Cell($"C{row}").Value = GetStatusName(assignment.Status);
                    worksheet.Cell($"D{row}").Value = assignment.DailyRate;
                    worksheet.Cell($"E{row}").Value = assignment.TotalPayment;
                    row++;
                }
            }

            worksheet.Range($"A8:E{row - 1}").Style.Font.Bold = true;
            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        public async Task<byte[]> GenerateMonthlyReportAsync(int year, int month)
        {
            var assignments = await _unitOfWork.Assignments.GetAllAsync(a => a.StartTime.Year == year && a.StartTime.Month == month);
            var payments = await _unitOfWork.Payments.GetAllAsync(p => p.PaymentDate.Year == year && p.PaymentDate.Month == month);

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Aylık Rapor");

            worksheet.Cell("A1").Value = $"SD AJANS - {GetMonthName(month)} {year} AYLIK RAPORU";
            worksheet.Cell("A1").Style.Font.FontSize = 16;
            worksheet.Cell("A1").Style.Font.Bold = true;
            worksheet.Range("A1:D1").Merge();

            worksheet.Cell("A3").Value = "Toplam Görevlendirme:";
            worksheet.Cell("B3").Value = assignments.Count();
            worksheet.Cell("A4").Value = "Toplam Ödeme:";
            worksheet.Cell("B4").Value = payments.Where(p => p.Status == PaymentStatus.Completed).Sum(p => p.Amount).ToString("C");
            worksheet.Cell("A5").Value = "Toplam Gider:";
            worksheet.Cell("B5").Value = assignments.Sum(a => a.TotalPayment).ToString("C");

            worksheet.Cell("A7").Value = "Görevlendirme Detayları";
            worksheet.Cell("A7").Style.Font.Bold = true;
            worksheet.Range("A7:D7").Merge();

            worksheet.Cell("A8").Value = "Manken";
            worksheet.Cell("B8").Value = "Organizasyon";
            worksheet.Cell("C8").Value = "Tarih";
            worksheet.Cell("D8").Value = "Ücret";

            int row = 9;
            foreach (var assignment in assignments)
            {
                var manken = await _unitOfWork.Mankens.GetByIdAsync(assignment.MankenId);
                var organization = await _unitOfWork.Organizations.GetByIdAsync(assignment.OrganizationId);
                
                if (manken != null && organization != null)
                {
                    worksheet.Cell($"A{row}").Value = $"{manken.FirstName} {manken.LastName}";
                    worksheet.Cell($"B{row}").Value = organization.Name;
                    worksheet.Cell($"C{row}").Value = assignment.StartTime.ToShortDateString();
                    worksheet.Cell($"D{row}").Value = assignment.TotalPayment;
                    row++;
                }
            }

            worksheet.Range($"A8:D{row - 1}").Style.Font.Bold = true;
            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        public async Task<byte[]> GenerateFinancialReportAsync(int year)
        {
            var monthlyRevenue = await _accountingService.GetMonthlyRevenueAsync(year);
            var monthlyExpenses = await _accountingService.GetMonthlyExpensesAsync(year);

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Finansal Rapor");

            worksheet.Cell("A1").Value = $"SD AJANS - {year} YILI FİNANSAL RAPORU";
            worksheet.Cell("A1").Style.Font.FontSize = 16;
            worksheet.Cell("A1").Style.Font.Bold = true;
            worksheet.Range("A1:D1").Merge();

            worksheet.Cell("A3").Value = "Ay";
            worksheet.Cell("B3").Value = "Gelir";
            worksheet.Cell("C3").Value = "Gider";
            worksheet.Cell("D3").Value = "Kar/Zarar";

            int row = 4;
            foreach (var month in monthlyRevenue.Keys)
            {
                var revenue = monthlyRevenue[month];
                var expense = monthlyExpenses.ContainsKey(month) ? monthlyExpenses[month] : 0;
                var profit = revenue - expense;

                worksheet.Cell($"A{row}").Value = month;
                worksheet.Cell($"B{row}").Value = revenue;
                worksheet.Cell($"C{row}").Value = expense;
                worksheet.Cell($"D{row}").Value = profit;
                row++;
            }

            worksheet.Range($"A3:D{row - 1}").Style.Font.Bold = true;
            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        public async Task<byte[]> GenerateAssignmentReportAsync(DateTime startDate, DateTime endDate)
        {
            var assignments = await _unitOfWork.Assignments.GetAllAsync(a => a.StartTime >= startDate && a.StartTime <= endDate);

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Görevlendirme Raporu");

            worksheet.Cell("A1").Value = $"SD AJANS - GÖREVLENDİRME RAPORU ({startDate.ToShortDateString()} - {endDate.ToShortDateString()})";
            worksheet.Cell("A1").Style.Font.FontSize = 16;
            worksheet.Cell("A1").Style.Font.Bold = true;
            worksheet.Range("A1:F1").Merge();

            worksheet.Cell("A3").Value = "Manken";
            worksheet.Cell("B3").Value = "Organizasyon";
            worksheet.Cell("C3").Value = "Başlangıç";
            worksheet.Cell("D3").Value = "Bitiş";
            worksheet.Cell("E3").Value = "Durum";
            worksheet.Cell("F3").Value = "Toplam Ücret";

            int row = 4;
            foreach (var assignment in assignments)
            {
                var manken = await _unitOfWork.Mankens.GetByIdAsync(assignment.MankenId);
                var organization = await _unitOfWork.Organizations.GetByIdAsync(assignment.OrganizationId);
                
                if (manken != null && organization != null)
                {
                    worksheet.Cell($"A{row}").Value = $"{manken.FirstName} {manken.LastName}";
                    worksheet.Cell($"B{row}").Value = organization.Name;
                    worksheet.Cell($"C{row}").Value = assignment.StartTime.ToShortDateString();
                    worksheet.Cell($"D{row}").Value = assignment.EndTime.ToShortDateString();
                    worksheet.Cell($"E{row}").Value = GetStatusName(assignment.Status);
                    worksheet.Cell($"F{row}").Value = assignment.TotalPayment;
                    row++;
                }
            }

            worksheet.Range($"A3:F{row - 1}").Style.Font.Bold = true;
            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        private string GetCategoryName(MankenCategory category)
        {
            return category switch
            {
                MankenCategory.Category1 => "Kategori 1 (40 TL/gün)",
                MankenCategory.Category2 => "Kategori 2 (100 TL/gün)",
                MankenCategory.Category3 => "Kategori 3 (%20)",
                _ => "Bilinmeyen"
            };
        }

        private string GetStatusName(AssignmentStatus status)
        {
            return status switch
            {
                AssignmentStatus.Scheduled => "Planlandı",
                AssignmentStatus.InProgress => "Devam Ediyor",
                AssignmentStatus.Completed => "Tamamlandı",
                AssignmentStatus.Cancelled => "İptal Edildi",
                _ => "Bilinmeyen"
            };
        }

        private string GetPaymentStatusName(PaymentStatus status)
        {
            return status switch
            {
                PaymentStatus.Pending => "Beklemede",
                PaymentStatus.Completed => "Tamamlandı",
                PaymentStatus.Cancelled => "İptal Edildi",
                _ => "Bilinmeyen"
            };
        }

        private string GetMonthName(int month)
        {
            return month switch
            {
                1 => "Ocak",
                2 => "Şubat",
                3 => "Mart",
                4 => "Nisan",
                5 => "Mayıs",
                6 => "Haziran",
                7 => "Temmuz",
                8 => "Ağustos",
                9 => "Eylül",
                10 => "Ekim",
                11 => "Kasım",
                12 => "Aralık",
                _ => "Bilinmeyen"
            };
        }
    }
} 