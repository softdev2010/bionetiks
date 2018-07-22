using System.Collections.Generic;

namespace FitnessApp.Models {
    public class GroupModel {
        public string Id {get;set;}
        public List<FriendModel> Users{ get;set;}
        public string Name {get;set;}
    }
}