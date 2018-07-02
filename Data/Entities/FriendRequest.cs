using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FitnessApp.Data.Entities
{
    public enum RequestState { Pending, Accepted, Denied}
    public class FriendRequest
    {
        public int Id {get;set;}
        public DateTime DateSent {get;set;}
        public string CreatorId {get;set;}
        public ApplicationUser Creator {get;set;}
        public string TargetUserId {get;set;}
        public ApplicationUser TargetUser {get;set;}
        public RequestState State {get;set;}
    }
}