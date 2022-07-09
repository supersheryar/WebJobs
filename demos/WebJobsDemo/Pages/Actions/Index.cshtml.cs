using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UkrGuru.SqlJson;
using UkrGuru.WebJobs.Data;

namespace WebJobsDemo.Pages.Actions;

public class IndexModel : PageModel
{
    public List<Action> Actions { get; set; }

    public async Task OnGetAsync()
    {
        Actions = await DbHelper.FromProcAsync<List<Action>>("WJbActions_Grd_Demo");
    }
}
