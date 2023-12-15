using BPRUtilities.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;

namespace BPRUtilities.Controllers;

[ApiController]
[Microsoft.AspNetCore.Mvc.Route("[controller]")]

public class SQLiteToMSSQLController : ControllerBase
{
    private ISQLiteToMSSQLService SQLiteToMssqlService { get; set; }
    
    public SQLiteToMSSQLController(ISQLiteToMSSQLService sqLiteToMssqlService)
    {
        SQLiteToMssqlService = sqLiteToMssqlService;
    }

    [HttpPost(Name = "SQLiteToMSSQL")]
    public void Execute()
    {
        SQLiteToMssqlService.ExecuteService();
    }
}