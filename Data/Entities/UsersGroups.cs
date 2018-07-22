using System;
using System.Collections.Generic;

namespace FitnessApp.Data.Entities
{
    public class UsersGroups
    {
        public int Id {get;set;}
        public string UserId {get;set;}
        public ApplicationUser User {get;set;}
        public Guid GroupId {get;set;}
        public Group Group {get;set;}
    }
}