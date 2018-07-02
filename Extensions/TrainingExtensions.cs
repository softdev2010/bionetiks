using System;
using System.Collections.Generic;
using FitnessApp.Data.Entities;
using FitnessApp.Helpers;
using FitnessApp.Models;
using FitnessApp.Models.Account;

namespace FitnessApp.Extensions
{
    public static class TrainingExtensions
    {
        public static TrainingModel MapToTrainingModel(this Training training)
        {
            return new TrainingModel()
            {
                Id = training.Id,
                Day = (int)training.Day,
                MuscleGroup = training.MuscleGroup,
                IsPersonalizedRoutine = training.IsRoutine,
                Weight = training.Weight
            };
        }

        public static Training MapToTraining(this TrainingModel trainingModel)
        {
            return new Training()
            {
                Id = trainingModel.Id,
                Day = (Days)trainingModel.Day,
                MuscleGroup = trainingModel.MuscleGroup,
                IsRoutine = trainingModel.IsPersonalizedRoutine,
                Weight = trainingModel.Weight
            };
        }

        public static Workout MapToWorkout(this WorkoutModel workoutModel)
        {
            var workout = new Workout()
            {
                TemplateId = workoutModel.Template.Id,
                NumberOfRepetitions = workoutModel.NumberOfRepetitions,
                IsSuccessfull = workoutModel.Successfull,
                AverageRepetitionDuration = workoutModel.AverageRepetitionDuration,
                AverageRepetitionAcceleration = workoutModel.AverageRepetitionAcceleration,
                AverageTilt = workoutModel.AverageTilt
            };

            foreach (var value in workoutModel.AccelerationValues)
            {
                workout.AccelerationValues += value.Duration + "," + value.Time.ToString("yyyy-MM-ddTHH:mm:ssZ") + ";";
            }

            foreach (var value in workoutModel.VelocityValues)
            {
                workout.VelocityValues += value.Duration + "," + value.Time.ToString("yyyy-MM-ddTHH:mm:ssZ") + ";";
            }

            foreach (var value in workoutModel.TiltValues)
            {
                workout.TiltValues += value.Duration + "," + value.Time.ToString("yyyy-MM-ddTHH:mm:ssZ") + ";";
            }

            return workout;
        }

        public static WorkoutModel MapToWorkoutModel(this Workout workout)
        {
            var workoutModel = new WorkoutModel()
            {
                Id = workout.Id,
                Template = workout.Template.MapToTrainingModel(),
                NumberOfRepetitions = workout.NumberOfRepetitions,
                Successfull = workout.IsSuccessfull,
                AverageRepetitionDuration = workout.AverageRepetitionDuration,
                AverageRepetitionAcceleration = workout.AverageRepetitionAcceleration,
                AverageTilt = workout.AverageTilt
            };
            var tmp = workout.AccelerationValues.Split(";");
            workoutModel.AccelerationValues = new List<TimedDouble>();
            foreach (var value in tmp)
            {
                if (value != "")
                {
                    var values = value.Split(",");
                    workoutModel.AccelerationValues.Add(new TimedDouble() { Duration = Convert.ToDouble(values[0]), Time = DateTime.Parse(values[1]) });
                }
            }
            tmp = workout.VelocityValues.Split(";");
            workoutModel.VelocityValues = new List<TimedDouble>();
            foreach (var value in tmp)
            {
                if (value != "")
                {
                    var values = value.Split(",");
                    workoutModel.VelocityValues.Add(new TimedDouble() { Duration = Convert.ToDouble(values[0]), Time = DateTime.Parse(values[1]) });
                }
            }
            tmp = workout.TiltValues.Split(";");
            workoutModel.TiltValues = new List<TimedDouble>();
            foreach (var value in tmp)
            {
                if (value != "")
                {
                    var values = value.Split(",");
                    workoutModel.TiltValues.Add(new TimedDouble() { Duration = Convert.ToDouble(values[0]), Time = DateTime.Parse(values[1]) });
                }
            }

            return workoutModel;
        }
    }
}
