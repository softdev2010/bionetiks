using System;

namespace FitnessApp.Models
{
    public class FriendRequestModel
    {
        public int Id {get;set;}
        public DateTime DateSent {get;set;}
        public FriendModel Friend {get;set;}
    }
}