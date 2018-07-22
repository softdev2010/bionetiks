using System;

namespace FitnessApp.Models
{
    public class FriendRequestModel
    {
        public string Id {get;set;}
        public DateTime DateSent {get;set;}
        public FriendModel Friend {get;set;}
    }
}