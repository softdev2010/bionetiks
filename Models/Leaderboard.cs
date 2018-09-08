using System;
using System.Collections.Generic;
using FitnessApp.Data.Entities;
using Newtonsoft.Json;

namespace FitnessApp.Models {
    public class Leaderboard {
        public List<LeaderboardUserModel> Users {get;set;}
        public int CurrentUserPosition {get;set;}
    }
}