﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UkrGuru.SqlJson;
using UkrGuru.WebJobs.Data;

namespace WebJobsDemo.Pages.Jobs
{
    public class IndexModel : PageModel
    {
        public List<Job> Jobs { get; set; }

        public string Date { get; set; }

        public async Task OnGetAsync()
        {
            Date = HttpContext.Request.Query["date"];

            if (string.IsNullOrEmpty(Date)) Date = DateTime.Today.ToString("yyyy-MM-dd");

            Jobs = await DbHelper.FromProcAsync<List<Job>>("WJbHistory_Grd_Demo", Date);
        }
    }
}
