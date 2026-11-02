using ClosedXML.Excel;

namespace CyberPulseUso.Backend.Helpers;

public class GeneExcel
{
    public static byte[] GenerarReporte<T>(IEnumerable<T> entity,string HojaName) where T : class
    {
        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add(HojaName);
            var properties = typeof(T).GetProperties();
            // Agregar encabezados
            for (int i = 0; i < properties.Length; i++)
            {
                worksheet.Cell(1, i + 1).Value = properties[i].Name;
                worksheet.Cell(1, i + 1).Style.Font.Bold = true;
            }
            // Agregar datos
            int row = 2;
            foreach (var empleado in entity)
            {
                for (int col = 0; col < properties.Length; col++)
                {
                    var value = properties[col].GetValue(empleado);
                    worksheet.Cell(row, col + 1).Value = value?.ToString() ?? string.Empty;
                }
                row++;
            }
            using (var stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                return stream.ToArray();
            }
        }
    }
}
