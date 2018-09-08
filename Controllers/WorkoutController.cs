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
        public async Task<IActionResult> GetAllWorkouts([FromQuery] bool includeWorkoutData = true, [FromQuery] string id = null, [FromQuery]string workoutName = null)
        {
            try
            {
                var workouts = await _context.Workouts
                    .Include(s => s.User)
                    .Include(s => s.Template)
                    .Include(s => s.Template.OptimalWeight)
                    .AsNoTracking()
                    .Where(t => id == null ? t.User.UserName.Equals(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value): t.UserId.Equals(id))
                    .ToListAsync();
                if(workoutName != null) {
                    workouts = workouts.Where(x => x.Template.MuscleGroup.Equals(workoutName)).ToList();
                }
                return new OkObjectResult(workouts.Select(x => x.MapToWorkoutModel(includeWorkoutData)));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpGet("{muscleGroup}")]
        public async Task<IActionResult> GetUserStatisticsByMuscleGroup(string muscleGroup="")
        {
            try {
                var allWorkoutsByUser = await _context.Workouts
                                                .Include(s => s.User)
                                                .Include(s => s.Template)
                                                .Include(s => s.Template.OptimalWeight)
                                                .AsNoTracking()
                                                .Where(t => t.User.UserName.Equals(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value) && t.Template.MuscleGroup.Equals(muscleGroup))
                                                .ToListAsync();
                return Ok(allWorkoutsByUser.Select(x=>x.MapToWorkoutModel(true)));
            }catch(Exception ex) {
                throw ex;
            }
        }
        [HttpPost("")]
        public async Task<IActionResult> CreateWorkout([FromBody]CreateWorkoutModel workoutModel)
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
                    .Include(s => s.OptimalWeight)
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
        [HttpGet("musclegroups")]
        public async Task<IActionResult> GetAllMuscleGroups()
        {
            try
            {
                var trainings = await _context.Trainings
                    .AsNoTracking()
                    .ToListAsync();
                var muscleGroups = trainings.Select(x => x.MuscleGroup).ToList();
                return new OkObjectResult(muscleGroups.Distinct().ToList());
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
        public async Task<IActionResult> CreateTrainingTemplate([FromBody]CreateTrainingModel trainingModel)
        {
            try
            {
                trainingModel.Id = null;
                var training = trainingModel.MapToTraining();
                training.User = await _context.Users
                                        .FirstOrDefaultAsync(x => x.UserName.Equals(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value));
                training.UserId = training.User.Id;
                training.OptimalWeight = new OptimalWeight() {
                    FailCount = trainingModel.OptimalWeight != null ? trainingModel.OptimalWeight.FailCount : 0,
                    IncreaseCount = trainingModel.OptimalWeight != null ? trainingModel.OptimalWeight.IncreaseCount : 0,
                    LastIncreaseDay = trainingModel.OptimalWeight != null ? trainingModel.OptimalWeight.LastIncreaseDay : null,
                    SuccessfullDays = trainingModel.OptimalWeight != null ? trainingModel.OptimalWeight.SuccessfullDays : 0,
                    Weight = trainingModel.OptimalWeight != null ? trainingModel.OptimalWeight.Weight + 5 : trainingModel.Weight + 5
                };
                _context.Trainings.Add(training);
                await _context.SaveChangesAsync();
                return new OkObjectResult(training.MapToTrainingModel());
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
                var training = await _context.Trainings.Include(x=> x.OptimalWeight).FirstOrDefaultAsync(x => x.Id.ToString().Equals(trainingModel.Id));
                training.IsRoutine = trainingModel.IsPersonalizedRoutine;
                training.MuscleGroup = trainingModel.MuscleGroup;
                training.Weight = trainingModel.Weight;
                training.Day = (Days)trainingModel.Day;
                if(trainingModel.OptimalWeight != null) {
                    training.OptimalWeight.FailCount = trainingModel.OptimalWeight.FailCount;
                    training.OptimalWeight.IncreaseCount = trainingModel.OptimalWeight.IncreaseCount;
                    training.OptimalWeight.LastIncreaseDay = trainingModel.OptimalWeight.LastIncreaseDay;
                    training.OptimalWeight.SuccessfullDays = trainingModel.OptimalWeight.SuccessfullDays;
                    training.OptimalWeight.Weight = trainingModel.OptimalWeight.Weight;
                }
                await _context.SaveChangesAsync();
                return new OkObjectResult(training.MapToTrainingModel());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPut("templates/weigths/{id}")]
        public async Task<IActionResult> UpdateTrainingTemplate([FromBody]UpdateWeight weightModel, string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id) || string.IsNullOrWhiteSpace(id))
                {
                    return BadRequest();
                }
                var weight = await _context.Weights.FirstOrDefaultAsync(x => x.Id.ToString().Equals(id));
                if(weight == null) {
                    return NotFound();
                }
                weight.FailCount = weightModel.FailCount;
                weight.IncreaseCount = weightModel.IncreaseCount;
                weight.LastIncreaseDay = weightModel.LastIncreaseDay;
                weight.SuccessfullDays = weightModel.SuccessfullDays;
                weight.Weight = weightModel.Weight;
                await _context.SaveChangesAsync();
                return new NoContentResult();
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