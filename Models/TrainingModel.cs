using System;
using Newtonsoft.Json;

namespace FitnessApp.Models {
    public class TrainingModel {
        public string Id { get; set; }
        public int Day {get;set;}
        public double Weight {get;set;}
        public string MuscleGroup {get;set;}
        public bool IsPersonalizedRoutine {get;set;}
    }
}