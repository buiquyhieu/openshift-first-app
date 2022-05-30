using System.Diagnostics;
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
}
