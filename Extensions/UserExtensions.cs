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
            return new UserModel()
            {
                Name = user.FirstName + " " + user.LastName,
                Age = user.Age,
                Nationality = user.Nationality,
                Gender = user.Gender == Gender.Female ? "Female" : user.Gender == Gender.Male ? "Male" : "Other",
                Height = user.Height,
                Weight = user.Weight,
                ProfileImage = user.PictureUrl,
                Visibility = user.Visibility == Visibility.Visible ? "Visible" : "Unvisible"
            };
        }

        public static void MapProfileToUser(this ApplicationUser user, ProfileModel profile)
        {
            if(profile.Username != null) {
                if (!profile.Username.Equals(user.UserName) && user.UserName.Equals(user.Email))
                {
                    user.UserName = profile.Username;
                }
            }
            if (profile.FirstName != null)
            {
                user.FirstName = profile.FirstName;
            }
            if (profile.LastName != null)
            {
                user.LastName = profile.LastName;
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
                Id = request.Id,
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
                Id = group.Id,
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
