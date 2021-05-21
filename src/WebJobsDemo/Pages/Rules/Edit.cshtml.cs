using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using UkrGuru.SqlJson;
using UkrGuru.WebJobs.Data;

namespace WebJobsDemo.Pages.Rules
{
    public class EditModel : PageModel
    {
        [BindProperty]
        public Rule Rule { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            Rule = await DbHelper.FromProcAsync<Rule>("WJbRules_Item_Demo", id);

            if (Rule.RuleId == 0) return NotFound();

            var actions = await DbHelper.FromProcAsync<List<Action>>("WJbActions_List_Demo");

            ViewData["Actions"] = new SelectList(actions, "Id", "Name");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            await DbHelper.ExecProcAsync("WJbRules_Upd_Demo", Rule);

            return RedirectToPage("./Index");
        }


    }
}
