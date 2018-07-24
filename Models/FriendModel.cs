using System;

namespace FitnessApp.Models
{
    public enum FriendStatus {NotFriends = 0, FriendRequestSent = 1, FriendRequestReceived = 2, Friends = 3}
    public class FriendModel
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Gender {get;set;}
        public int? Age {get;set;}
        public string Nationality {get;set;}
        public float? Height {get;set;}
        public double? Latitude {get;set;}
        public double? Longitude {get;set;}
        public float? Weight {get;set;}
        public string ProfileImage { get; set; }
        public bool Visibility { get; set; }
        public FriendStatus? FriendStatus {get;set;}
    }
}