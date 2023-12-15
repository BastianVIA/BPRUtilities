using System.Globalization;
using BPRUtilities.Models;
using CsvHelper;
using CsvHelper.Configuration;

namespace BPRUtilities.Services;

public class PastePCBAUidService : IPastePCBAUidService
{
    private List<int> PCBAUids { get; set; } = new();
    private int uidIndex { get; set; } = 0;
    public IGetPCBAUidService GetPcbaUidService { get; set; }
    
    public PastePCBAUidService(IGetPCBAUidService getPcbaUidService)
    {
        GetPcbaUidService = getPcbaUidService;
    }
    
    public async Task ExecuteService()
    {
        PCBAUids = GetPcbaUidService.GetPCBAUids();
        var folderPath = "C:\\Users\\Bastian\\Downloads\\666-0000";
        //var folderPath = "C:\\Users\\Bastian\\RiderProjects\\BPR2\\LINTest\\CSVLogs";
        var filePaths = Directory.GetFiles(folderPath);
        
        foreach (var filePath in filePaths)
        {
            var rows = GetRowsWithNewPCBAUid(filePath);
            CreateUpdatedCsvFile(rows, GetFileName(filePath, 5));
        }
    }

    private IEnumerable<CsvFile> GetRowsWithNewPCBAUid(string filePath)
    {
        var rowsToReturn = new List<CsvFile>();
        var pcbaUidInfo = "UniqueID from Actuator";
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = false,
            MissingFieldFound = null,
            Delimiter = ";"
        };
        using var reader = new StreamReader(filePath);
        using var csv = new CsvReader(reader, config);
        csv.Context.RegisterClassMap<CsvFileMap>();
        var rows = csv.GetRecords<CsvFile>();
        foreach (var row in rows)
        {
            if (row.Info == pcbaUidInfo)
            {
                row.Value = PCBAUids[uidIndex].ToString();
            }
            rowsToReturn.Add(row);
        }
        uidIndex++;
        return rowsToReturn;
    }
    
    private void CreateUpdatedCsvFile(IEnumerable<CsvFile> rows, string fileName)
    {
        var newFilePath = "C:\\Users\\Bastian\\Desktop\\UpdatedCSVLogs\\" + fileName;
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = false,
            MissingFieldFound = null,
            Delimiter = ";"
        };
        using var writer = new StreamWriter(newFilePath);
        using var csv = new CsvWriter(writer, config);
        csv.WriteRecords(rows);
    }
    
    static string GetFileName(string path, int index)
    {
        string[] parts = path.Split('\\');
        
        if (index >= 0 && index < parts.Length)
        {
            string result = string.Join("\\", parts, index, parts.Length - index);
            return result;
        }
        return string.Empty;
    }
    
}