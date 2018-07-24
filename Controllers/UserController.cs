using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FitnessApp.Data;
using FitnessApp.Data.Entities;
using FitnessApp.Extensions;
using FitnessApp.Models;
using FitnessApp.Models.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitnessApp.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public UserController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        [HttpGet("me")]
        public async Task<IActionResult> GetRegisteredUser()
        {
            var user = await _userManager.FindByNameAsync(this.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            if (user == null)
                return new UnauthorizedResult();
            else
                return new OkObjectResult(user.MapToUserModel());
        }

        [HttpGet("search/{searchWord}")]
        public async Task<IActionResult> SearchUsers(string searchWord)
        {
            var users = await _context.Users
                .Where(x => x.UserName.ToLower().Contains(searchWord.ToLower())).ToListAsync();
            var userModels = users.Select(x => x.MapToFriendUserModel(0)).ToList();
            var loggedUser = this.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            foreach(var user in userModels) {
                var friendStatuses = await _context.FriendRequests
                                    .Include(x => x.Creator)
                                    .Include(x => x.TargetUser)
                                    .Where(x => ((x.CreatorId.ToString().Equals(user.Id) &&
                                                x.TargetUser.UserName.Equals(loggedUser)) ||
                                            (x.TargetUserId.ToString().Equals(user.Id) &&
                                            x.Creator.UserName.Equals(loggedUser))) &&
                                            x.State != RequestState.Denied)
                                    .ToListAsync();
            
                if(friendStatuses.Count == 0) {
                    user.FriendStatus = FriendStatus.NotFriends;
                } else {
                    var receivedRequestExists = friendStatuses
                        .FirstOrDefault(x => x.CreatorId.ToString().Equals(user.Id) &&
                        x.TargetUser.UserName.Equals(loggedUser) && x.State == RequestState.Pending);
                    if(receivedRequestExists != null) {
                        user.FriendStatus = FriendStatus.FriendRequestReceived;
                    } else {
                        var sentRequestExists = friendStatuses
                            .FirstOrDefault(x => x.TargetUserId.ToString().Equals(user.Id) &&
                                x.Creator.UserName.Equals(loggedUser) && x.State == RequestState.Pending);
                        if(sentRequestExists != null) {
                            user.FriendStatus = FriendStatus.FriendRequestSent;
                        } else {
                            user.FriendStatus = FriendStatus.Friends;
                        }
                    }
                }
            }
            return new OkObjectResult(userModels);
        }
 
        [HttpGet("friends")]
        public async Task<IActionResult> GetFriends()
        {
            var username = this.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var friends = await _context.FriendRequests.Include(x => x.TargetUser)
                                    .Include(x => x.Creator)
                                    .Where(x =>
                                        (x.Creator.UserName.Equals(username) || x.TargetUser.UserName.Equals(username))
                                        && x.State == RequestState.Accepted)
                                    .ToListAsync();
            return new OkObjectResult(friends.Select(x => x.MapToFriendModel(username, 3)));
        }

        [HttpGet("requests/incoming")]
        public async Task<IActionResult> GetIncomingFriendRequests()
        {
            var username = this.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var requests = await _context.FriendRequests.Include(x => x.TargetUser)
                                    .Include(x => x.Creator)
                                    .Where(x => x.TargetUser.UserName.Equals(username)
                                        && x.State == RequestState.Pending)
                                    .ToListAsync();
            return new OkObjectResult(requests.Select(x => x.MapRequestToModel(x.Creator, false)));
        }

        [HttpGet("requests/outgoing")]
        public async Task<IActionResult> GetOutgoingFriendRequests()
        {
            var username = this.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var requests = await _context.FriendRequests.Include(x => x.TargetUser)
                                    .Include(x => x.Creator)
                                    .Where(x => x.Creator.UserName.Equals(username)
                                        && x.State == RequestState.Pending)
                                    .ToListAsync();
            return new OkObjectResult(requests.Select(x => x.MapRequestToModel(x.TargetUser, true)));
        }

        [HttpGet("groups/{searchWord}/search")]
        public async Task<IActionResult> GetUserGroups(string searchWord)
        {
            try
            {
                var userGroups = await _context.UsersGroups
                                    .Include(x => x.User)
                                    .Include(x => x.Group)
                                    .Include(x => x.Group.Users)
                                    .Where(x => x.Group.Name.ToLower().Contains(searchWord.ToLower()))
                                    .ToListAsync();

                var groups = userGroups.Select(x => x.Group).Select(x => new { Id = x.Id, Name = x.Name});
                
                return new OkObjectResult(groups);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [HttpGet("groups")]
        public async Task<IActionResult> GetUserGroups()
        {
            try
            {
                var userGroups = await _context.UsersGroups
                                    .Include(x => x.User)
                                    .Include(x => x.Group)
                                    .Include(x => x.Group.Users)
                                    .Include("Group.Users.User")
                                    .Where(x => x.User.UserName.Equals(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value))
                                    .ToListAsync();

                var groups = userGroups.Select(x => x.Group.MapGroupToModel(_context, User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value));
                
                return new OkObjectResult(groups);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet("groups/{id}")]
        public async Task<IActionResult> GetUserGroupById(string id)
        {
            try
            {
                var group = await _context.Groups
                                    .Include("Users.User")
                                    .FirstOrDefaultAsync(x => x.Id.ToString().Equals(id));

                
                return new OkObjectResult(group.MapGroupToModel(_context, User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost("groups")]
        public async Task<IActionResult> CreateGroup([FromBody]CreateGroupModel groupModel)
        {
            try
            {
                
                var user = await _context.Users
                                        .FirstOrDefaultAsync(x => x.UserName.Equals(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value));
                var group = new Group();
                group.Name = groupModel.GroupName;
                _context.Groups.Add(group);
                await _context.SaveChangesAsync();
                var userGroups = new UsersGroups();
                userGroups.UserId = user.Id;
                userGroups.GroupId = group.Id;
                _context.UsersGroups.Add(userGroups);
                await _context.SaveChangesAsync();
                group.Users = new List<UsersGroups>() { userGroups };
                return new OkObjectResult(group.MapGroupToModel(_context, User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPut("groups/{id}")]
        public async Task<IActionResult> RenameGroup(string id, [FromBody]CreateGroupModel groupModel)
        {
            try
            {
                
                var group = await _context.Groups
                                    .Include("Users.User")
                                    .FirstOrDefaultAsync(x => x.Id.ToString().Equals(id));
                
                if(group == null) {
                    return NotFound();
                }
                var userExists = group.Users.FirstOrDefault(x => x.User.UserName.Equals(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value));
                if(userExists == null) {
                    return new BadRequestObjectResult(new List<ErrorViewModel>() { new ErrorViewModel() {Code = "UserNotFound", Description = "User is not group member."}});
                }
                group.Name = groupModel.GroupName;
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPut("{userId}/groups/{id}")]
        public async Task<IActionResult> AddUserToGroup(string id, string userId)
        {
            try
            {
                var permissionExists = await _context.UsersGroups
                    .Include(x => x.User)
                    .FirstOrDefaultAsync(x => x.GroupId.ToString().Equals(id) && x.User.UserName.Equals(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value));
                if(permissionExists == null) {
                    return new BadRequestObjectResult(new List<ErrorViewModel>() { new ErrorViewModel() {Code = "NotAllowed", Description = "Action is not allowed."}});
                }
                var userExists = await _context.UsersGroups
                    .FirstOrDefaultAsync(x => x.GroupId.ToString().Equals(id) && x.UserId.Equals(userId));
                if(userExists != null) {
                    return new BadRequestObjectResult(new List<ErrorViewModel>() { new ErrorViewModel() {Code = "UserExists", Description = "User is already group member."}});
                }
                var userGroups = new UsersGroups();
                userGroups.UserId = userId;
                userGroups.GroupId = new Guid(id);
                _context.UsersGroups.Add(userGroups);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpDelete("{userId}/groups/{id}")]
        public async Task<IActionResult> RemoveUserFromGroup(string id, string userId)
        {
            try
            {

                var permissionExists = await _context.UsersGroups
                    .Include(x => x.User)
                    .FirstOrDefaultAsync(x => x.GroupId.ToString().Equals(id) && x.User.UserName.Equals(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value));
                if(permissionExists == null) {
                    return new BadRequestObjectResult(new List<ErrorViewModel>() { new ErrorViewModel() {Code = "NotAllowed", Description = "Action is not allowed."}});
                }
                var userExists = await _context.UsersGroups
                                        .FirstOrDefaultAsync(x => x.GroupId.ToString().Equals(id) && x.UserId.Equals(userId));
                if(userExists == null) {
                    return new BadRequestObjectResult(new List<ErrorViewModel>() { new ErrorViewModel() {Code = "UserNotFound", Description = "User is not group member."}});
                }
                _context.UsersGroups.Remove(userExists);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost("{userId}/request")]
        public async Task<IActionResult> SendFriendRequest(string userId = "")
        {
            try
            {
                if (userId.ToString().Equals(""))
                {
                    return NotFound();
                }
                var friend = await _context.Users.Where(x => x.Id.ToString().Equals(userId)).FirstOrDefaultAsync();
                if (friend == null)
                {
                    return NotFound();
                }
                var creator = await _context.Users.Select(x => new { Id = x.Id, UserName = x.UserName }).FirstOrDefaultAsync(x => x.UserName.Equals(this.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value));
                if (creator.Id.Equals(friend.Id))
                {
                    return new BadRequestObjectResult(new List<ErrorViewModel>() { new ErrorViewModel() { Code = "InvalidFriendRequest", Description = "Friend request cannot be sent to yourself." } });
                }

                var existingRequests = await _context.FriendRequests
                    .Where(x => ((x.CreatorId.Equals(creator.Id) && x.TargetUserId.Equals(friend.Id))
                                            || (x.CreatorId.Equals(friend.Id) && x.TargetUserId.Equals(creator.Id)))
                                            && (x.State == RequestState.Accepted || x.State == RequestState.Pending))
                    .ToListAsync();


                if (existingRequests.Count > 0)
                {
                    var pendingRequestExists = existingRequests.FirstOrDefault(x => x.State == RequestState.Pending);
                    if (pendingRequestExists != null)
                    {
                        if (pendingRequestExists.CreatorId.Equals(creator.Id))
                        {
                            return new BadRequestObjectResult(new List<ErrorViewModel>() { new ErrorViewModel() { Code = "InvalidFriendRequest", Description = "Friend request has already been sent." } });
                        }
                        else
                        {
                            return new BadRequestObjectResult(new List<ErrorViewModel>() { new ErrorViewModel() { Code = "InvalidFriendRequest", Description = "Friend request has already been sent from the targeted user." } });
                        }
                    }

                    var acceptedRequestExists = existingRequests.FirstOrDefault(x => x.State == RequestState.Accepted);
                    if(acceptedRequestExists != null) {
                        return new BadRequestObjectResult(new List<ErrorViewModel>() { new ErrorViewModel() { Code = "InvalidFriendRequest", Description = "Targeted user is already a friend." } });
                    }
                }

                var request = new FriendRequest();
                request.TargetUserId = userId;
                request.CreatorId = creator.Id;
                request.DateSent = DateTime.Now;
                request.State = RequestState.Pending;
                _context.FriendRequests.Add(request);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        [HttpPut("me")]
        public async Task<IActionResult> UpdateUserProfile([FromBody] ProfileModel profileModel)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(this.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
                user.MapProfileToUser(profileModel);
                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                    return new NoContentResult();
                else
                    return new BadRequestObjectResult(result.Errors);
            }

            return new BadRequestObjectResult(ModelState.Values.Select(x => x.Errors));
        }
        [HttpPatch("visibility")]
        public async Task<IActionResult> ToggleVisibility()
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName.Equals(this.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value));
                user.Visibility = !user.Visibility;
                await _context.SaveChangesAsync();
                return new NoContentResult();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPatch("requests/{requestId}")]
        public async Task<IActionResult> RespondToFriendRequest(string requestId, [FromBody] RespondRequestModel respondModel)
        {
            try
            {
                if (string.IsNullOrEmpty(requestId) || string.IsNullOrWhiteSpace(requestId))
                {
                    return BadRequest();
                }
                var me = await _context.Users.FirstOrDefaultAsync(x => x.UserName.Equals(this.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value));

                var request = await _context.FriendRequests
                    .FirstOrDefaultAsync(x => x.Id.ToString().Equals(requestId));
                if (request == null)
                {
                    return NotFound();
                }
                if(request.CreatorId.ToString().Equals(me.Id)) {
                    return new BadRequestObjectResult(new List<ErrorViewModel>() { new ErrorViewModel() { Code = "RequestResponseFailed", Description = "Cannot accept request that is not received." } });
                }
                if(!request.TargetUserId.ToString().Equals(me.Id)) {
                    return new BadRequestObjectResult(new List<ErrorViewModel>() { new ErrorViewModel() { Code = "RequestResponseFailed", Description = "Cannot accept somebody other's request." } });
                }
                if (request.State != RequestState.Pending)
                {
                    return new BadRequestObjectResult(new List<ErrorViewModel>() { new ErrorViewModel() { Code = "RequestResponseFailed", Description = "Friend request has been already processed." } });
                }
                request.State = respondModel.IsAccepted ? RequestState.Accepted : RequestState.Denied;
                await _context.SaveChangesAsync();
                return new NoContentResult();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpDelete("friends/{friendId}")]
        public async Task<IActionResult> RemoveFriend(string friendId = "")
        {
            try
            {
                if (friendId == "")
                {
                    return BadRequest();
                }
                var friend = await _context.Users.FirstOrDefaultAsync(x => x.Id.ToString().Equals(friendId));
                if (friend == null)
                {
                    return NotFound();
                }
                var request = await _context.FriendRequests.Include(x => x.Creator)
                                    .Include(x => x.TargetUser)
                                    .FirstOrDefaultAsync(x => 
                                        ((x.Creator.UserName.Equals(this.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value) && x.TargetUserId.ToString().Equals(friendId))
                                            || (x.TargetUser.UserName.Equals(this.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value) && x.CreatorId.ToString().Equals(friendId)))
                                            && x.State == RequestState.Accepted);

                if(request == null) {
                    return NotFound();
                }
                _context.Remove(request);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
