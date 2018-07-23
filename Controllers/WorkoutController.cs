using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FitnessApp.Data;
using FitnessApp.Data.Entities;
using FitnessApp.Extensions;
using FitnessApp.Models;
using FitnessApp.Models.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitnessApp.Controllers
{
    [Route("api/workouts")]
    [Authorize]
    public class WorkoutController : Controller
    {
        private readonly ApplicationDbContext _context;
        public WorkoutController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllWorkouts([FromQuery] bool includeWorkoutData = true)
        {
            try
            {
                var workouts = await _context.Workouts
                    .Include(s => s.User)
                    .Include(s => s.Template)
                    .AsNoTracking()
                    .Where(t => t.User.UserName.Equals(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value))
                    .ToListAsync();

                return new OkObjectResult(workouts.Select(x => x.MapToWorkoutModel(includeWorkoutData)));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost("")]
        public async Task<IActionResult> CreateWorkout([FromBody]WorkoutModel workoutModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var workout = workoutModel.MapToWorkout();
                    workout.User = await _context.Users.FirstOrDefaultAsync(x => x.UserName.Equals(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value));
                    workout.UserId = workout.User.Id;
                    _context.Workouts.Add(workout);
                    await _context.SaveChangesAsync();
                    workoutModel.Id = workout.Id.ToString();
                    return new OkObjectResult(workoutModel);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return new BadRequestObjectResult(ModelState.Values.Select(x => x.Errors));
        }

        [HttpGet("templates")]
        public async Task<IActionResult> GetAllPersonalizedRoutines()
        {
            try
            {
                var trainings = await _context.Trainings
                    .Include(s => s.User)
                    .AsNoTracking()
                    .Where(t => t.User.UserName.Equals(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value) 
                        && t.IsRoutine && t.IsDeleted == false)
                    .ToListAsync();

                return new OkObjectResult(trainings.Select(x => x.MapToTrainingModel()));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet("templates/{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                var training = await _context.Trainings
                    .AsNoTracking()
                    .FirstOrDefaultAsync(m => m.Id.ToString().Equals(id));

                if (training == null)
                {
                    return NotFound();
                }
                if(training.IsDeleted) {
                    return NotFound();
                }

                return new OkObjectResult(training.MapToTrainingModel());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost("templates")]
        public async Task<IActionResult> CreateTrainingTemplate([FromBody]TrainingModel trainingModel)
        {
            try
            {
                trainingModel.Id = null;
                var training = trainingModel.MapToTraining();
                training.User = await _context.Users
                                        .FirstOrDefaultAsync(x => x.UserName.Equals(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value));
                training.UserId = training.User.Id;
                _context.Trainings.Add(training);
                await _context.SaveChangesAsync();
                trainingModel.Id = training.Id.ToString();
                return new OkObjectResult(trainingModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPut("templates")]
        public async Task<IActionResult> UpdateTrainingTemplate([FromBody]TrainingModel trainingModel)
        {
            try
            {
                var training = trainingModel.MapToTraining();
                _context.Entry(training).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return new OkObjectResult(training.MapToTrainingModel());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpDelete("template/{id}")]
        public async Task<IActionResult> DeleteTemplateById(string id)
        {
            try
            {
                var training = await _context.Trainings
                    .FirstOrDefaultAsync(m => m.Id.ToString().Equals(id));

                if (training == null)
                {
                    return NotFound();
                }

                training.IsDeleted = true;

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}