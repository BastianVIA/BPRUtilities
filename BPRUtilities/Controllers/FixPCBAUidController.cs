using BPRUtilities.Services;
using Microsoft.AspNetCore.Mvc;

namespace BPRUtilities.Controllers;

[ApiController]
[Microsoft.AspNetCore.Mvc.Route("[controller]")]

public class FixPCBAUidController : ControllerBase
{
    private IFixPCBAUidService fixPcbaUidService { get; set; }
    
    public FixPCBAUidController(IFixPCBAUidService fixPcbaUidService)
    {
        this.fixPcbaUidService = fixPcbaUidService;
    }

    [HttpPost(Name = "FixPCBAUid")]
    public void Execute()
    {
        fixPcbaUidService.ExecuteService();
    }
}