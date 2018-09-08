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
    public class LeaderboardsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public LeaderboardsController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet("{muscleGroup}")]

        public async Task<IActionResult> Get(string muscleGroup, [FromQuery] bool isLocal = false,
            [FromQuery] bool isProfessional = false, [FromQuery] bool isNational = false, [FromQuery] string groupName = null)
        {
            var loggedUser = this.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var currentUser = await _context.Users.Select(x => new
            { x.Id, x.UserName, x.Nationality, x.Latitude, x.Longitude }).FirstOrDefaultAsync(x => x.UserName.Equals(loggedUser));

            var users = new List<ApplicationUser>();
            var leaderboard = new Leaderboard();
            if (groupName == null)
            {
                users = await _context.Users.Include(x => x.Workouts)
                    .Include("Workouts.Template").Include("Workouts.Template.OptimalWeight")
                    .Where(x => x.Workouts.Where(y => y.Template.MuscleGroup.Equals(muscleGroup)).Count() > 0)
                    .ToListAsync();
                if (isLocal)
                {
                    users = users.Select(x => new GeoUser() { User = x, Dist =  x.GetUserDistance(currentUser.Latitude, currentUser.Longitude) })
                        .Where(x => x.Dist <= 100).Select(x => x.User).ToList();
                }
                else if (isProfessional)
                {
                    users = users.Where(x => x.IsProfessional).ToList();
                }
                else if (isNational)
                {
                    users = users.Where(x => x.Nationality != null && x.Nationality.Equals(currentUser.Nationality)).ToList();
                }
            }
            else
            {
               var groups = await _context.UsersGroups
                                    .Include(x => x.User)
                                    .Include(x => x.User.Workouts)
                                    .Include("User.Workouts.Template.OptimalWeight")
                                    .Include(x => x.Group)
                                    .Where(x => x.User.Workouts.Where(y => y.Template.MuscleGroup.Equals(muscleGroup)).Count() > 0 && 
                                            x.Group.Name.Equals(groupName))
                                    .ToListAsync();
                users = groups.Select(x=> x.User).ToList();
            }

            foreach (var user in users)
            {
                user.Workouts = user.Workouts.Where(x => x.Template.MuscleGroup.Equals(muscleGroup))
                    .OrderByDescending(x => x.Template.Weight).ToList();
            }

            leaderboard.Users = users.OrderByDescending(x => x.Workouts.First().Template.Weight).Select(x=>x.MapToLeaderboardUserModel()).ToList();
            var me = leaderboard.Users.FirstOrDefault(x => x.Username.Equals(loggedUser));
            if(me != null)
                leaderboard.CurrentUserPosition = leaderboard.Users.IndexOf(me) + 1;
            return Ok(leaderboard);
        }
    }
}