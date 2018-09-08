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
using FitnessApp.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using FitnessApp.Extensions;
using FitnessApp.Data.Entities;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace FitnessApp.Controllers
{
    [Route("api/upload")]
    [Authorize]
    public class UploadController : Controller
    {
        private IHostingEnvironment _environment;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        public UploadController(IHostingEnvironment env, ApplicationDbContext context, IConfiguration configuration)
        {
            _environment = env;
            _context = context;
            _configuration = configuration;
        }
        [HttpPost]
        [RequestSizeLimit(100_000_000)]
        public async Task<IActionResult> Post(IFormFile file)
        {
            if (file.Length > 0)
            {
                var loggedUser = this.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
                var currentUser = await _context.Users.FirstOrDefaultAsync(x => x.UserName.Equals(loggedUser));
                var uploads = Path.Combine(_environment.WebRootPath, "Uploads");
                var checkFile = true;
                var newId = Guid.NewGuid().ToString();
                while(checkFile) {
                    var fileExists = System.IO.File.Exists(Path.Combine(uploads, newId + "." + file.ContentType.Split("/")[1]));
                    if(fileExists) {
                        newId = Guid.NewGuid().ToString();
                    } else {
                        checkFile = false;
                    }
                }
                var fileName = newId + "." + file.ContentType.Split("/")[1];
                using (var fileStream = new FileStream(Path.Combine(uploads, fileName), FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                    var baseUrl = _configuration.GetSection("BaseUrl");
                    currentUser.PictureUrl = baseUrl.Value + "Uploads/" + fileName;
                    await _context.SaveChangesAsync();
                    return Ok(new { count = 1, file.Length, currentUser.PictureUrl });
                }
            }
            else
            {
                return BadRequest();
            }

        }
    }
}