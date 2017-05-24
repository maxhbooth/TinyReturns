using System.IO;
using System.Reflection;
using Dimensional.TinyReturns.Core.PortfolioReportingContext.Services.PerformanceReport;
using OfficeOpenXml;

namespace Dimensional.TinyReturns.ExcelRendering
{
    public class PerformanceReportExcelReportView : IPerformanceReportExcelReportView
    {
        public void RenderReport(
            PerformanceReportExcelReportModel model,
            string fullFilePath)
        {
            var excelTemplate = GetExcelTemplate();

            File.Copy(excelTemplate, fullFilePath);

            var excelFile = new FileInfo(fullFilePath);

            using (var package = new ExcelPackage(excelFile))
            {
                var worksheet =  package.Workbook.Worksheets[1];

                worksheet.Cells[2, 1].Value = model.MonthText;

                var rowIndex = 5;
                
                foreach (var record in model.Records)
                {
                    worksheet.Cells[rowIndex, 1].Value = record.EntityNumber;
                    worksheet.Cells[rowIndex, 2].Value = record.Name;
                    worksheet.Cells[rowIndex, 3].Value = record.Type;
                    worksheet.Cells[rowIndex, 4].Value = record.FeeType;
                    worksheet.Cells[rowIndex, 5].Value = record.OneMonth;
                    worksheet.Cells[rowIndex, 6].Value = record.ThreeMonths;
                    worksheet.Cells[rowIndex, 7].Value = record.YearToDate;
                    worksheet.Cells[rowIndex, 8].Value = record.YearToDate;

                    rowIndex++;
                }

                package.Save();
            }
        }

        private string GetExcelTemplate()
        {
            var folder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);

            folder = folder.Replace("file:\\", "");

            return folder + "\\PerformanceTemplate.xlsx";
        }
    }
}