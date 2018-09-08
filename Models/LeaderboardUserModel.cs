
using FitnessApp.Data.Entities;

namespace FitnessApp.Models
{
    public class LeaderboardUserModel
    {
        public string Id {get;set;}
        public string Username { get; set; }
        public string Email { get; set; }
        public string Gender {get;set;}
        public int? Age {get;set;}
        public string Nationality {get;set;}
        public bool Professional {get;set;}
        public float? Height {get;set;}
        public double? Latitude {get;set;}
        public double? Longitude {get;set;}
        public float? Weight {get;set;}
        public string ProfileImage { get; set; }
        public bool Visibility { get; set; }
        public WorkoutModel Workout {get;set;}
    }
}
