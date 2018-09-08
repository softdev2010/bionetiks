using System;
using FitnessApp.Data.Entities;
using Newtonsoft.Json;

namespace FitnessApp.Models {
    public class UpdateWeight {
         public double Weight { get; set; }
        public int SuccessfullDays { get; set; }
        public int IncreaseCount { get; set; }
        public DateTime? LastIncreaseDay { get; set; }
        public int FailCount { get; set; }
    }
}