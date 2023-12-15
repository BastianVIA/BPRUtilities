using CsvHelper.Configuration;

namespace BPRUtilities.Models;

public class CsvFileMap : ClassMap<CsvFile>
{
    public CsvFileMap()
    {
        Map(m => m.Timestamp).Index(0);
        Map(m => m.BayNo).Index(1);
        Map(m => m.StepType).Index(2);
        Map(m => m.StepNo).Index(3);
        Map(m => m.Info).Index(4);
        Map(m => m.Value).Index(5);
    }
}