namespace BPRUtilities.Models;

public class Actuator
{
    public string WorkOrderNumber { get; set; }
    public int SerialNumber { get; set; }
    public int PCBAUid { get; set; }
    public string CreationDate { get; set; }
    public string CreationTime { get; set; }
    
    public int SumOfPassedTests { get; set; }
    public int SumOfFailedTests { get; set; }
}