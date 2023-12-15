using CsvHelper.Configuration.Attributes;

namespace BPRUtilities.Models;

public class CsvFile
{
    [Index(0)]
    public string Timestamp { get; set; }
    [Index(1)]
    public string BayNo { get; set; }
    [Index(2)]
    public string StepType { get; set; }
    [Index(3)]
    public string StepNo { get; set; }
    [Index(4)]
    public string Info { get; set; }
    [Index(5)]
    public string Value { get; set; }
}