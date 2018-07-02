using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using FitnessApp.Models;

namespace FitnessApp.Controllers
{
    [Route("api/nationalities")]
    [Authorize]
    public class NationalityController : Controller
    {
        private IHostingEnvironment _env;
        public NationalityController(IHostingEnvironment env) {
            _env = env;
        }
        [HttpGet("")]
        public async Task<IActionResult> GetAllNations()
        {
            try {
                var content = await System.IO.File.ReadAllTextAsync(System.IO.Path.Combine(_env.WebRootPath,"Data/countries.json"));
                return Ok(JsonConvert.DeserializeObject(content));
            }catch(Exception ex) {
                throw ex;
            }
        }
    }
}