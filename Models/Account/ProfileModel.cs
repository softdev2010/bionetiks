
using System.ComponentModel.DataAnnotations;

namespace FitnessApp.Models.Account
{
    public class ProfileModel
    {
        public string Gender {get;set;}
        public int Age {get;set;}
        public string Username {get;set;}
        public float Height {get;set;}
        public float Weight {get;set;}
        public string Nationality {get;set;}
        public string PictureUrl { get; set; }
        public bool? ProfileComplete {get;set;}
    }
}
