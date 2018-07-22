
namespace FitnessApp.Models
{
    public class UserModel
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Gender {get;set;}
        public int? Age {get;set;}
        public string Nationality {get;set;}
        public float? Height {get;set;}
        public float? Weight {get;set;}
        public string ProfileImage { get; set; }
        public bool Visibility { get; set; }
    }
}
