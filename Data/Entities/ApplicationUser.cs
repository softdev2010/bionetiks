using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FitnessApp.Data.Entities
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public enum Gender { Male, Female, Other }
    public enum Visibility {Visible, Unvisible}
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Gender Gender {get;set;}
        public int Age {get;set;}
        public long FacebookId {get;set;}
        public string GoogleId {get;set;}
        public string Nationality {get;set;}
        public float Height {get;set;}
        public float Weight {get;set;}
        public string PictureUrl { get; set; }
        public bool ProfileComplete {get;set;}
        public Visibility Visibility {get;set;}
        public List<Training> Trainings {get;set;}
        public List<UsersGroups> Groups {get;set;}
    }
}
