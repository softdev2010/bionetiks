using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace FitnessApp.Data.Entities
{
    public class OptimalWeight
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }
        public double Weight { get; set; }
        public int SuccessfullDays { get; set; }
        public int IncreaseCount { get; set; }
        public DateTime? LastIncreaseDay { get; set; }
        public int FailCount { get; set; }
        [JsonIgnore]
        public Guid TrainingId { get; set; }
        [JsonIgnore]
        public Training Training { get; set; }
    }
}