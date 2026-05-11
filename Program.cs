// See https://aka.ms/new-console-template for more information

using System.IO;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;



var currentDirectory = Directory.GetCurrentDirectory();
var storesDirectory = Path.Combine(currentDirectory, "stores");

var salesTotalDir = Path.Combine(currentDirectory, "salesTotalDir");
Directory.CreateDirectory(salesTotalDir);

var salesFiles = FindFiles(storesDirectory);

var salesTotal = CalculateSalesTotal(salesFiles);

File.AppendAllText(Path.Combine(salesTotalDir, "totals.txt"), $"{salesTotal}{Environment.NewLine}");

// Sales summary report
GenerateSalesSummaryReport(salesFiles, salesTotal, salesTotalDir);


IEnumerable<string> FindFiles(string folderName)
{
    List<string> salesFiles = new List<string>();

    var foundFiles = Directory.EnumerateFiles(folderName, "*", SearchOption.AllDirectories);

    foreach (var file in foundFiles)
    {
        var extension = Path.GetExtension(file);
        if (extension == ".json")
        {
            salesFiles.Add(file);
        }
    }

    return salesFiles;
}

double CalculateSalesTotal(IEnumerable<string> salesFiles)
{
    double salesTotal = 0;

    // Loop over each file path in salesFiles
    foreach (var file in salesFiles)
    {
        // Read the contents of the file
        string salesJson = File.ReadAllText(file);

        // Parse the contents as JSON
        SalesData? data = JsonConvert.DeserializeObject<SalesData?>(salesJson);

        // Add the amount found in the Total field to the salesTotal variable
        salesTotal += data?.Total ?? 0;
    }

    return salesTotal;
}

void GenerateSalesSummaryReport(
    IEnumerable<string> salesFiles,
    double grandTotal,
    string outputDirectory)
{
    var report = new StringBuilder();

    report.AppendLine("Sales Summary");
    report.AppendLine("----------------------------");
    report.AppendLine($"Total Sales: {grandTotal:C}");
    report.AppendLine();
    report.AppendLine("Details:");

    foreach (var file in salesFiles)
    {
        string salesJson = File.ReadAllText(file);

        SalesData? data =
            JsonConvert.DeserializeObject<SalesData?>(salesJson);

        double fileTotal = data?.Total ?? 0;

        string fileName = Path.GetFileName(file);

        report.AppendLine($"  {fileName}: {fileTotal:C}");
    }

    string reportPath =
        Path.Combine(outputDirectory, "sales-summary.txt");

    File.WriteAllText(reportPath, report.ToString());

    Console.WriteLine($"Sales summary created: {reportPath}");
}



record SalesData(double Total);