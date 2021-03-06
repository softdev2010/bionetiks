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
                Id = training.Id.ToString(),
                Day = (int)training.Day,
                MuscleGroup = training.MuscleGroup,
                IsPersonalizedRoutine = training.IsRoutine,
                Weight = training.Weight,
                OptimalWeight = training.OptimalWeight
            };
        }

        public static Training MapToTraining(this TrainingModel trainingModel)
        {
            var training = new Training()
            {
                Day = (Days)trainingModel.Day,
                MuscleGroup = trainingModel.MuscleGroup,
                Weight = trainingModel.Weight,
                IsRoutine = trainingModel.IsPersonalizedRoutine
            };
            if (trainingModel.Id != null)
            {
                training.Id = new Guid(trainingModel.Id);
            }
            return training;

        }

        public static Training MapToTraining(this CreateTrainingModel trainingModel)
        {
            var training = new Training()
            {
                Day = (Days)trainingModel.Day,
                MuscleGroup = trainingModel.MuscleGroup,
                Weight = trainingModel.Weight,
                IsRoutine = trainingModel.IsPersonalizedRoutine
            };
            if (trainingModel.Id != null)
            {
                training.Id = new Guid(trainingModel.Id);
            }
            return training;

        }

        public static Workout MapToWorkout(this WorkoutModel workoutModel)
        {
            var workout = new Workout()
            {
                TemplateId = new Guid(workoutModel.Template.Id),
                NumberOfRepetitions = workoutModel.NumberOfRepetitions,
                IsSuccessfull = workoutModel.Successfull,
                AverageRepetitionDuration = workoutModel.AverageRepetitionDuration,
                AverageRepetitionAcceleration = workoutModel.AverageVelocity,
                AverageTilt = workoutModel.AverageTilt,
                Date = workoutModel.Date,
                Duration = workoutModel.Duration
            };

            foreach (var value in workoutModel.AccelerationValues)
            {
                workout.AccelerationValues += value.Value + "," + value.Timestamp + ";";
            }

            foreach (var value in workoutModel.VelocityValues)
            {
                workout.VelocityValues += value.Value + "," + value.Timestamp + ";";
            }

            foreach (var value in workoutModel.TiltValues)
            {
                workout.TiltValues += value.Value + "," + value.Timestamp + ";";
            }

            return workout;
        }

        public static Workout MapToWorkout(this CreateWorkoutModel workoutModel)
        {
            var workout = new Workout()
            {
                TemplateId = new Guid(workoutModel.Template.Id),
                NumberOfRepetitions = workoutModel.NumberOfRepetitions,
                IsSuccessfull = workoutModel.Successfull,
                AverageRepetitionDuration = workoutModel.AverageRepetitionDuration,
                AverageRepetitionAcceleration = workoutModel.AverageVelocity,
                AverageTilt = workoutModel.AverageTilt,
                Date = workoutModel.Date,
                Duration = workoutModel.Duration
            };

            foreach (var value in workoutModel.AccelerationValues)
            {
                workout.AccelerationValues += value.Value + "," + value.Timestamp + ";";
            }

            foreach (var value in workoutModel.VelocityValues)
            {
                workout.VelocityValues += value.Value + "," + value.Timestamp + ";";
            }

            foreach (var value in workoutModel.TiltValues)
            {
                workout.TiltValues += value.Value + "," + value.Timestamp + ";";
            }

            return workout;
        }

        public static WorkoutModel MapToWorkoutModel(this Workout workout, bool includeWorkoutData = true)
        {
            var workoutModel = new WorkoutModel()
            {
                Id = workout.Id.ToString(),
                Template = workout.Template.MapToTrainingModel(),
                User = workout.User.MapToUserModel(),
                NumberOfRepetitions = workout.NumberOfRepetitions,
                Successfull = workout.IsSuccessfull,
                AverageRepetitionDuration = workout.AverageRepetitionDuration,
                AverageVelocity = workout.AverageRepetitionAcceleration,
                AverageTilt = workout.AverageTilt,
                Date = workout.Date,
                Duration = workout.Duration
            };

            if (includeWorkoutData)
            {
                var tmp = workout.AccelerationValues.Split(";");
                workoutModel.AccelerationValues = new List<TimedDouble>();
                foreach (var value in tmp)
                {
                    if (value != "")
                    {
                        var values = value.Split(",");
                        workoutModel.AccelerationValues.Add(new TimedDouble() { Value = Convert.ToDouble(values[0]), Timestamp = Convert.ToDouble(values[1]) });
                    }
                }
                tmp = workout.VelocityValues.Split(";");
                workoutModel.VelocityValues = new List<TimedDouble>();
                foreach (var value in tmp)
                {
                    if (value != "")
                    {
                        var values = value.Split(",");
                        workoutModel.VelocityValues.Add(new TimedDouble() { Value = Convert.ToDouble(values[0]), Timestamp = Convert.ToDouble(values[1]) });
                    }
                }
                tmp = workout.TiltValues.Split(";");
                workoutModel.TiltValues = new List<TimedDouble>();
                foreach (var value in tmp)
                {
                    if (value != "")
                    {
                        var values = value.Split(",");
                        workoutModel.TiltValues.Add(new TimedDouble() { Value = Convert.ToDouble(values[0]), Timestamp = Convert.ToDouble(values[1]) });
                    }
                }
            }

            return workoutModel;
        }
    }
}