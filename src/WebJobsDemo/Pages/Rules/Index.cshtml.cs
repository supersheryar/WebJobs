using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UkrGuru.SqlJson;
using UkrGuru.WebJobs.Models;

namespace WebJobsDemo.Pages.Rules
{
    public class IndexModel : PageModel
    {
        public List<Rule> Rules { get; set; }

        public async Task OnGetAsync()
        {
            Rules = await DbHelper.FromProcAsync<List<Rule>>("WJbRules_List_Demo");
        }
    }
}
