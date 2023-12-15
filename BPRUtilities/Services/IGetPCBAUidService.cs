using BPRUtilities.Models;

namespace BPRUtilities.Services;

public interface IGetPCBAUidService
{
    public List<int> GetPCBAUids();
    public List<PCBA> GetPCBAs();

    public List<int> GetBadPCBAs();
}