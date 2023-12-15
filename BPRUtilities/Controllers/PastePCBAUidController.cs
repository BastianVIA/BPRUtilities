using BPRUtilities.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;

namespace BPRUtilities.Controllers;

[ApiController]
[Microsoft.AspNetCore.Mvc.Route("[controller]")]

public class PastePCBAUidController : ControllerBase
{
    private IPastePCBAUidService pastePcbaUidService { get; set; }
    
    public PastePCBAUidController(IPastePCBAUidService pastePcbaUidService)
    {
        this.pastePcbaUidService = pastePcbaUidService;
    }

    [HttpPost(Name = "PastePCBAUid")]
    public void Execute()
    {
        pastePcbaUidService.ExecuteService();
    }
}