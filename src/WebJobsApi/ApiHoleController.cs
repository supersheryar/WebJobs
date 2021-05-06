// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using UkrGuru.SqlJson;
using WebJobsApi.Helpers;

namespace WebJobsApi
{
    [ApiController]
    [Route("")]
    public class ApiHoleController : ControllerBase
    {
        private const string ApiProcPefix = "WJa_";

        private readonly AuthService _auth;
        private readonly DbService _db;

        public ApiHoleController(AuthService auth, DbService db)
        {
            _auth = auth;
            _db = db;
        }

        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody] string apiholekey)
        {
            var response = _auth.Authenticate(apiholekey);

            if (response == null)
                return BadRequest(new { message = "ApiHoleKey is incorrect" });

            return Ok(response);
        }

        // GET: <proc_list>
        [HttpGet("{proc_list}")]
        [Authorize]
        public async Task<string> Get(string proc_list)
        {
            return await _db.FromProcAsync($"{ApiProcPefix}{proc_list}");
        }

        // GET <proc_item>/<id>
        [HttpGet("{proc_item}/{id}")]
        [Authorize]
        public async Task<string> Get(string proc_item, string id)
        {
            return await _db.FromProcAsync($"{ApiProcPefix}{proc_item}", id);
        }

        // POST <proc_ins>
        [HttpPost("{proc_ins}")]
        [Authorize]
        public async Task<string> Post(string proc_ins, [FromBody] string item)
        {
            return await _db.FromProcAsync($"{ApiProcPefix}{proc_ins}", item);
        }

        // PUT <proc_upd>/<id>
        [HttpPut("{proc_upd}/{id}")]
        [Authorize]
        public async Task Put(string proc_upd, string id, [FromBody] string item)
        {
            await _db.ExecProcAsync($"{ApiProcPefix}{proc_upd}", item);
        }

        // DELETE <proc_del>/<id>
        [HttpDelete("{proc_del}/{id}")]
        [Authorize]
        public async Task Delete(string proc_del, string id)
        {
            await _db.ExecProcAsync($"{ApiProcPefix}{proc_del}", id);
        }
    }
}