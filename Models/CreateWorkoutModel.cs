using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FitnessApp.Helpers;
using FitnessApp.Models;

namespace FitnessApp.Data.Entities
{
    public class CreateWorkoutModel
    {
        public string Id { get; set; }
        [Required]
        public CreateTrainingModel Template {get;set;}
        public UserModel User {get;set;}
        public int NumberOfRepetitions {get;set;}
        public double Duration {get;set;}
        public DateTime Date {get;set;}
        public bool Successfull {get;set;}
        public double AverageRepetitionDuration {get;set;}
        public double AverageVelocity {get;set;}
        public double AverageTilt {get;set;}
        public List<TimedDouble> AccelerationValues {get;set;}
        public List<TimedDouble> VelocityValues {get;set;}
        public List<TimedDouble> TiltValues {get;set;}

    }
}