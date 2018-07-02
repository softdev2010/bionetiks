using System.Collections.Generic;

namespace FitnessApp.Data.Entities
{
    public enum Days { Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday }
    public class Training
    {
        public int Id { get; set; }
        public Days Day {get;set;}
        public double Weight {get;set;}
        public string MuscleGroup {get;set;}
        public string UserId {get;set;}
        public ApplicationUser User {get;set;}
        public bool IsRoutine {get;set;}
        public List<Workout> Workouts {get;set;}
    }
}