using System.Diagnostics;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.AspNetCore.Mvc;
using MvcProj.Models;
using OfficeOpenXml;

namespace MvcProj.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {


        return View();
    }

    public IActionResult GetExcel()
    {
        if (!Directory.Exists("./Temp"))
            Directory.CreateDirectory("./Temp");

        string fileName = $"./Temp/file_{DateTime.UtcNow:ddMMyyyy_HHmmss}.xlsx";
        FileInfo excelFile = new(fileName);

        using var excel = new ExcelPackage();
        var sheet = excel.Workbook.Worksheets.Add("sheet 1");
        sheet.Cells[1, 1].Value = "test value";

        excel.SaveAs(excelFile);

        ZipFolder("./Temp", "abc.zip");

        if (System.IO.File.Exists(fileName))
        {
            var bytes = System.IO.File.ReadAllBytes(fileName);

            return File(bytes, "application/vnd.ms-excel", $"file_{DateTime.UtcNow:ddMMyyyy_HHmmss}.xlsx");
        }

        return Json("Fail!");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public static void ZipFolder(string directoryPath, string outputFilePath, int compressionLevel = 9)
    {
        try
        {
            if (!Directory.Exists(directoryPath)) return;

            string[] filenames = Directory.GetFiles(directoryPath);
            using (ZipOutputStream outputStream = new ZipOutputStream(System.IO.File.Create(outputFilePath)))
            {
                outputStream.SetLevel(compressionLevel);
                byte[] buffer = new byte[4096];
                foreach (string filename in filenames)
                {
                    ZipEntry zipEntry = new ZipEntry(Path.GetFileName(filename));
                    zipEntry.DateTime = DateTime.Now;
                    outputStream.PutNextEntry(zipEntry);
                    using (FileStream fs = System.IO.File.OpenRead(filename))
                    {

                        // Using a fixed size buffer here makes no noticeable difference for output
                        // but keeps a lid on memory usage.
                        int sourceBytes;

                        do
                        {
                            sourceBytes = fs.Read(buffer, 0, buffer.Length);
                            outputStream.Write(buffer, 0, sourceBytes);
                        } while (sourceBytes > 0);
                    }
                }
                outputStream.Finish();

                // Close is important to wrap things up and unlock the file.
                outputStream.Close();
            }
        }
        catch (Exception ex)
        {
        }
    }
}
