using System.Linq;
using FitnessApp.Data;
using FitnessApp.Data.Entities;
using FitnessApp.Models;
using FitnessApp.Models.Account;

namespace FitnessApp.Extensions
{
    public static class UserExtensions
    {
        public static UserModel MapToUserModel(this ApplicationUser user)
        {
            var userModel = new UserModel() {
                Username = user.UserName,
                Email = user.Email,
                Nationality = user.Nationality,
                Gender = user.Gender == Gender.None ? null : (user.Gender == Gender.Female ? "Female" : user.Gender == Gender.Male ? "Male" : "Other"),
                ProfileImage = user.PictureUrl,
                Visibility = user.Visibility
            };
            if(user.Age != 0) {
                userModel.Age = user.Age;
            }
            if(user.Height != 0) {
                userModel.Height = user.Height;
            }
            if(user.Weight != 0) {
                userModel.Weight = user.Weight;
            }
            return userModel;
        }

        public static void MapProfileToUser(this ApplicationUser user, ProfileModel profile)
        {
            if(profile.Username != null) {
                if (!profile.Username.Equals(user.UserName) && user.UserName.Equals(user.Email))
                {
                    user.UserName = profile.Username;
                }
            }
            if (profile.Nationality != null)
            {
                user.Nationality = profile.Nationality;
            }
            if (profile.Weight != 0)
            {
                user.Weight = profile.Weight;
            }
            if (profile.Height != 0)
            {
                user.Height = profile.Height;
            }
            if (profile.Age != 0)
            {
                user.Age = profile.Age;
            }
            if (profile.Gender != null)
            {
                profile.Gender = profile.Gender.ToLower();
                user.Gender = profile.Gender.Equals("male") ? Gender.Male : profile.Gender.Equals("female") ? Gender.Female : Gender.Other;
            }
            if (profile.ProfileComplete != null)
            {
                user.ProfileComplete = (bool)profile.ProfileComplete;
            }
        }
        public static FriendModel MapToFriendModel(this FriendRequest request, string username)
        {
            ApplicationUser user = null;

            if(request.Creator.UserName.Equals(username)) {
                user = request.TargetUser;
            } else if(request.TargetUser.UserName.Equals(username)) {
                user = request.Creator;
            }
            return new FriendModel()
            {
                Id = user.Id,
                Username = user.UserName,
                ProfileImage = user.PictureUrl
            };
        }
        public static FriendRequestModel MapRequestToModel(this FriendRequest request, ApplicationUser user)
        {
            return new FriendRequestModel()
            {
                Id = request.Id.ToString(),
                DateSent = request.DateSent,
                Friend = new FriendModel()
                {
                    Id = user.Id,
                    Username = user.UserName,
                    ProfileImage = user.PictureUrl
                }
            };
        }

        public static GroupModel MapGroupToModel(this Group group)
        {
            var users = group.Users.Select(x=> x.User);
            return new GroupModel()
            {
                Id = group.Id.ToString(),
                Name = group.Name,
                Users = users.Select(x => new FriendModel() {
                    Id = x.Id,
                    Username = x.UserName,
                    ProfileImage = x.PictureUrl
                }).ToList()
            };
        }
    }
}
