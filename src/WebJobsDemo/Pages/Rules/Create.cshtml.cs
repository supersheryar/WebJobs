using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using UkrGuru.SqlJson;
using UkrGuru.WebJobs.Data;

namespace WebJobsDemo.Pages.Rules
{
    public class CreateModel : PageModel
    {
        public async Task<IActionResult> OnGet()
        {
            Rule = new Rule();

            var actions = await DbHelper.FromProcAsync<List<Action>>("WJbActions_List_Demo");

            ViewData["Actions"] = new SelectList(actions, "Id", "Name");

            return Page();
        }

        [BindProperty]
        public Rule Rule { get; set; } 

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            await DbHelper.ExecProcAsync("WJbRules_Ins_Demo", Rule);

            return RedirectToPage("./Index");
        }
    }
}
