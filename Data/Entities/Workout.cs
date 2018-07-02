using System;

namespace FitnessApp.Data.Entities
{
    public class Workout
    {
        public int Id { get; set; }
        public int TemplateId {get;set;}
        public Training Template {get;set;}
        public string UserId {get;set;}
        public ApplicationUser User {get;set;}
        public int NumberOfRepetitions {get;set;}
        public double Duration {get;set;}
        public DateTime Date {get;set;}
        public bool IsSuccessfull {get;set;}
        public double AverageRepetitionDuration {get;set;}
        public double AverageRepetitionAcceleration {get;set;}
        public double AverageTilt {get;set;}
        public string AccelerationValues {get;set;}
        public string VelocityValues {get;set;}
        public string TiltValues {get;set;}
    }
}