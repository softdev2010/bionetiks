using System;
using FitnessApp.Data.Entities;
using Newtonsoft.Json;

namespace FitnessApp.Models {
    public class TrainingModel {
        public string Id { get; set; }
        public int Day {get;set;}
        public double Weight {get;set;}
        public OptimalWeight OptimalWeight {get;set;}
        public string MuscleGroup {get;set;}
        public bool IsPersonalizedRoutine {get;set;}
    }
}