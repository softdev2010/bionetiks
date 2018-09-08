using System;
using System.Globalization;
using System.Linq;
using System.Text;
using FitnessApp.Data;
using FitnessApp.Data.Entities;
using FitnessApp.Models;
using FitnessApp.Models.Account;
using Microsoft.EntityFrameworkCore;

namespace FitnessApp.Extensions
{
    public static class UserExtensions
    {
        public static UserModel MapToUserModel(this ApplicationUser user)
        {
            var userModel = new UserModel()
            {
                Id = user.Id,
                Username = user.UserName,
                Email = user.Email,
                Nationality = user.Nationality,
                Professional = user.IsProfessional,
                Gender = user.Gender == Gender.None ? null : (user.Gender == Gender.Female ? "Female" : user.Gender == Gender.Male ? "Male" : "Other"),
                ProfileImage = user.PictureUrl,
                Visibility = user.Visibility
            };
            if (user.Age != 0)
            {
                userModel.Age = user.Age;
            }
            if (user.Height != 0)
            {
                userModel.Height = user.Height;
            }
            if (user.Weight != 0)
            {
                userModel.Weight = user.Weight;
            }
            if (user.Latitude != 0)
            {
                userModel.Latitude = user.Latitude;
            }
            if (user.Longitude != 0)
            {
                userModel.Longitude = user.Longitude;
            }
            return userModel;
        }

        public static LeaderboardUserModel MapToLeaderboardUserModel(this ApplicationUser user)
        {
            var userModel = new LeaderboardUserModel()
            {
                Id = user.Id,
                Username = user.UserName,
                Email = user.Email,
                Nationality = user.Nationality,
                Professional = user.IsProfessional,
                Gender = user.Gender == Gender.None ? null : (user.Gender == Gender.Female ? "Female" : user.Gender == Gender.Male ? "Male" : "Other"),
                ProfileImage = user.PictureUrl,
                Visibility = user.Visibility,
                Workout = user.Workouts.First().MapToWorkoutModel(false)
            };
            if (user.Age != 0)
            {
                userModel.Age = user.Age;
            }
            if (user.Height != 0)
            {
                userModel.Height = user.Height;
            }
            if (user.Weight != 0)
            {
                userModel.Weight = user.Weight;
            }
            if (user.Latitude != 0)
            {
                userModel.Latitude = user.Latitude;
            }
            if (user.Longitude != 0)
            {
                userModel.Longitude = user.Longitude;
            }
            return userModel;
        }

        public static void MapProfileToUser(this ApplicationUser user, ProfileModel profile)
        {
            if (profile.Username != null)
            {
                if (!profile.Username.Equals(user.UserName) && user.UserName.Equals(user.Email))
                {
                    user.UserName = profile.Username;
                }
            }
            if (profile.Nationality != null)
            {
                user.Nationality = profile.Nationality;
            }
            if (profile.Professional != null)
            {
                user.IsProfessional = (bool)profile.Professional;
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

            if (profile.Latitude != 0)
            {
                user.Latitude = profile.Latitude;
            }

            if (profile.Longitude != 0)
            {
                user.Longitude = profile.Longitude;
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
        public static FriendModel MapToFriendModel(this FriendRequest request, string username, int? state)
        {
            ApplicationUser user = null;

            if (request.Creator.UserName.Equals(username))
            {
                user = request.TargetUser;
            }
            else if (request.TargetUser.UserName.Equals(username))
            {
                user = request.Creator;
            }
            return user.MapToFriendUserModel(state);
        }
        public static FriendRequestModel MapRequestToModel(this FriendRequest request, ApplicationUser user, bool isSent)
        {
            return new FriendRequestModel()
            {
                Id = request.Id.ToString(),
                DateSent = request.DateSent,
                Friend = user.MapToFriendUserModel(isSent ? 1 : 2)
            };
        }

        public static GroupModel MapGroupToModel(this Group group, ApplicationDbContext context, string username)
        {
            var users = group.Users.Select(x => x.User);
            var groupModel = new GroupModel()
            {
                Id = group.Id.ToString(),
                Name = group.Name,
                Users = users.Select(x => x.MapToFriendUserModel(0)).ToList()
            };

            foreach (var user in groupModel.Users)
            {
                if (!user.Username.Equals(username))
                {

                    var friendStatuses = context.FriendRequests
                                        .Include(x => x.Creator)
                                        .Include(x => x.TargetUser)
                                        .Where(x => ((x.CreatorId.ToString().Equals(user.Id) &&
                                                    x.TargetUser.UserName.Equals(username)) ||
                                                (x.TargetUserId.ToString().Equals(user.Id) &&
                                                x.Creator.UserName.Equals(username))) &&
                                                x.State != RequestState.Denied)
                                        .ToListAsync().Result;
                    user.FriendStatus = new FriendStatus();
                    if (friendStatuses.Count == 0)
                    {
                        user.FriendStatus.State = FriendStatusState.NotFriends;
                    }
                    else
                    {
                        var receivedRequestExists = friendStatuses
                            .FirstOrDefault(x => x.CreatorId.ToString().Equals(user.Id) &&
                            x.TargetUser.UserName.Equals(username) && x.State == RequestState.Pending);
                        if (receivedRequestExists != null)
                        {
                            user.FriendStatus.State = FriendStatusState.FriendRequestReceived;
                            user.FriendStatus.RequestId = receivedRequestExists.Id.ToString();
                        }
                        else
                        {
                            var sentRequestExists = friendStatuses
                                .FirstOrDefault(x => x.TargetUserId.ToString().Equals(user.Id) &&
                                    x.Creator.UserName.Equals(username) && x.State == RequestState.Pending);
                            if (sentRequestExists != null)
                            {
                                user.FriendStatus.State = FriendStatusState.FriendRequestSent;
                                user.FriendStatus.RequestId = sentRequestExists.Id.ToString();
                            }
                            else
                            {
                                user.FriendStatus.State = FriendStatusState.Friends;
                            }
                        }
                    }
                }
            }

            return groupModel;
        }

        public static FriendModel MapToFriendUserModel(this ApplicationUser user, int? state)
        {
            var userModel = new FriendModel()
            {
                Id = user.Id,
                Username = user.UserName,
                Email = user.Email,
                Nationality = user.Nationality,
                Gender = user.Gender == Gender.None ? null : (user.Gender == Gender.Female ? "Female" : user.Gender == Gender.Male ? "Male" : "Other"),
                ProfileImage = user.PictureUrl,
                Professional = user.IsProfessional,
                Visibility = user.Visibility,
                FriendStatus = new FriendStatus()
                {
                    State = (FriendStatusState)state
                }
            };
            if (user.Age != 0)
            {
                userModel.Age = user.Age;
            }
            if (user.Height != 0)
            {
                userModel.Height = user.Height;
            }
            if (user.Weight != 0)
            {
                userModel.Weight = user.Weight;
            }
            if (user.Latitude != 0)
            {
                userModel.Latitude = user.Latitude;
            }
            if (user.Longitude != 0)
            {
                userModel.Longitude = user.Longitude;
            }
            return userModel;
        }

        public static String RemoveDiacritics(String s)
        {
            String normalizedString = s.Normalize(NormalizationForm.FormD);
            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < normalizedString.Length; i++)
            {
                Char c = normalizedString[i];
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    stringBuilder.Append(c);
            }

            return stringBuilder.ToString();
        }
        public static double GetUserDistance(this ApplicationUser user, double lat1, double longt)
        {
            var lat2  = user.Latitude;
            double theta = longt - user.Longitude;
            double dist = Math.Sin(deg2rad(lat1)) * Math.Sin(deg2rad(lat2)) + Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) * Math.Cos(deg2rad(theta));
            dist = Math.Acos(dist);
            dist = rad2deg(dist);
            dist = dist * 60 * 1.1515;
            return dist;
        }

        private static double deg2rad(double deg)
        {
            return (deg * Math.PI / 180.0);
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //::  This function converts radians to decimal degrees             :::
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        private static double rad2deg(double rad)
        {
            return (rad / Math.PI * 180.0);
        }

    }
}
