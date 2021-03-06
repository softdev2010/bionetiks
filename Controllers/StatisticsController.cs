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

namespace FitnessApp.Controllers
{
    [Route("api/statistics")]
    [Authorize]
    public class StatisticsController : Controller
    {
        private IHostingEnvironment _env;
         private readonly ApplicationDbContext _context;
        public StatisticsController(IHostingEnvironment env, ApplicationDbContext context) {
            _env = env;
            _context = context;
        }
        [HttpGet("")]
        public async Task<IActionResult> GetUserStatistics([FromQuery] string id = null)
        {
            try {
                Expression<Func<Workout, bool>> condition = null;
                if(id == null) {
                    condition = t => t.User.UserName.Equals(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
                } else {
                    condition = t  => t.User.Id.Equals(id);
                }
                var allWorkoutsByUser = await _context.Workouts
                                                .Include(s => s.User)
                                                .Include(s => s.Template)
                                                .AsNoTracking()
                                                .Where(condition)
                                                .Select(x => new { NumberOfRepetitions = x.NumberOfRepetitions, Muscle = x.Template.MuscleGroup })
                                                .ToListAsync();

                var statisticsModel = new UserStatisticsModel();
                statisticsModel.TotalWorkouts = allWorkoutsByUser.Count;
                statisticsModel.TotalRepetitions = allWorkoutsByUser.Sum(x => x.NumberOfRepetitions);
                statisticsModel.TotalWorkoutsByMuscleGroup = new Dictionary<string, int>();
                statisticsModel.TotalRepetitionsByMuscleGroup = new Dictionary<string, int>();
                foreach(var workout in allWorkoutsByUser) {
                    if(statisticsModel.TotalWorkoutsByMuscleGroup.ContainsKey(workout.Muscle)) {
                        statisticsModel.TotalWorkoutsByMuscleGroup[workout.Muscle] += 1;
                    } else {
                        statisticsModel.TotalWorkoutsByMuscleGroup.Add(workout.Muscle, 1);
                    }

                    if(statisticsModel.TotalRepetitionsByMuscleGroup.ContainsKey(workout.Muscle)) {
                        statisticsModel.TotalRepetitionsByMuscleGroup[workout.Muscle] += workout.NumberOfRepetitions;
                    } else {
                        statisticsModel.TotalRepetitionsByMuscleGroup.Add(workout.Muscle, workout.NumberOfRepetitions);
                    }
                }

                return Ok(statisticsModel);
            }catch(Exception ex) {
                throw ex;
            }
        }
    }
}