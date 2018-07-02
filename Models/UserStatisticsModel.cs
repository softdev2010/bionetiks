
using System.Collections.Generic;

namespace FitnessApp.Models
{
    public class UserStatisticsModel
    {
        public int TotalWorkouts {get;set;}
        public int TotalRepetitions {get;set;}
        public Dictionary<string, int> TotalRepetitionsByMuscleGroup {get;set;}
        public Dictionary<string, int> TotalWorkoutsByMuscleGroup {get;set;}
    }
}
