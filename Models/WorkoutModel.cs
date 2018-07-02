using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FitnessApp.Helpers;
using FitnessApp.Models;

namespace FitnessApp.Data.Entities
{
    public class WorkoutModel
    {
        public int Id { get; set; }
        [Required]
        public TrainingModel Template {get;set;}
        public int NumberOfRepetitions {get;set;}
        public double Duration {get;set;}
        public DateTime Date {get;set;}
        public bool Successfull {get;set;}
        public double AverageRepetitionDuration {get;set;}
        public double AverageRepetitionAcceleration {get;set;}
        public double AverageTilt {get;set;}
        public List<TimedDouble> AccelerationValues {get;set;}
        public List<TimedDouble> VelocityValues {get;set;}
        public List<TimedDouble> TiltValues {get;set;}

    }
}