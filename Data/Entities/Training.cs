using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitnessApp.Data.Entities
{
    public enum Days { Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday }
    public class Training
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }
        public Days Day {get;set;}
        public string MuscleGroup {get;set;}
        public string UserId {get;set;}
        public ApplicationUser User {get;set;}
        public double Weight {get;set;}
        public OptimalWeight OptimalWeight {get;set;}
        public bool IsRoutine {get;set;}
        public bool IsDeleted {get;set;}
        public List<Workout> Workouts {get;set;}
    }
}