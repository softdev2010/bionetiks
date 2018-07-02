using System.Collections.Generic;

namespace FitnessApp.Data.Entities
{
    public class Group
    {
        public int Id {get;set;}
        public string Name {get;set;}
        public List<UsersGroups> Users {get;set;}
    }
}