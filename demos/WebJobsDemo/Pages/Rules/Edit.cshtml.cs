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
        public RuleInput Rule { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            Rule = await DbHelper.FromProcAsync<RuleInput>("WJbRules_Get_Demo", id);

            if (Rule.RuleId == 0) return NotFound();

            var actions = await DbHelper.FromProcAsync<List<Action>>("WJbActions_Lst_Demo");

            ViewData["Actions"] = new SelectList(actions, "ActionId", "ActionName");

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
