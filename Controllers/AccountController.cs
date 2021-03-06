﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using FitnessApp.Models;
using FitnessApp.Models.AccountViewModels;
using FitnessApp.Services;
using FitnessApp.Services.Intefaces;
using Newtonsoft.Json;
using FitnessApp.Helpers;
using FitnessApp.Data;
using FitnessApp.Models.Account;
using FitnessApp.Data.Entities;
using System.Net;
using FitnessApp.Extensions;

namespace FitnessApp.Controllers
{
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtFactory _jwtFactory;
        private readonly JwtIssuerOptions _jwtOptions;
        private readonly ApplicationDbContext _appDbContext;
        private readonly GoogleAuthModel _googleAuthModel;
        private static readonly HttpClient Client = new HttpClient();

        public AccountController(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext appDbContext,
            IJwtFactory jwtFactory,
            IOptions<JwtIssuerOptions> jwtOptions,
            IOptions<GoogleAuthModel> googleAuthOptions)
        {
            _userManager = userManager;
            _appDbContext = appDbContext;
            _jwtFactory = jwtFactory;
            _jwtOptions = jwtOptions.Value;
            _googleAuthModel = googleAuthOptions.Value;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Username, Email = model.Email, Gender = Gender.None, Visibility = true, Nationality = "US" };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    var identity = await GetClaimsIdentity(model.Username, model.Password);
                    var jwt = await Tokens.GenerateJwt(identity, _jwtFactory, model.Username, _jwtOptions, new JsonSerializerSettings { Formatting = Formatting.Indented }, false);
                    return new OkObjectResult(jwt);
                }
                else
                {
                    var duplicateUsername = result.Errors.FirstOrDefault(x => x.Code.Equals("DuplicateUserName"));
                    if (duplicateUsername != null)
                    {
                        duplicateUsername.Description = "Username '" + user.UserName + "' is already taken.";
                    }
                    return new BadRequestObjectResult(result.Errors);
                }
            }

            return new BadRequestObjectResult(ModelState.Values.Select(x => x.Errors));
        }

        // POST api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]LoginModel credentials)
        {
            if (credentials.Username == null)
            {
                return new BadRequestObjectResult(new List<ErrorViewModel>() { new ErrorViewModel() { Code = "InvalidLogin", Description = "Username is required." } });
            }

            if (credentials.Password == null)
            {
                return new BadRequestObjectResult(new List<ErrorViewModel>() { new ErrorViewModel() { Code = "InvalidLogin", Description = "Password is required." } });
            }

            var identity = await GetClaimsIdentity(credentials.Username, credentials.Password);
            if (identity == null)
            {
                return new BadRequestObjectResult(new List<ErrorViewModel>() { new ErrorViewModel() { Code = "InvalidLogin", Description = "Incorrect username or password." } });
            }

            var user = await _userManager.FindByNameAsync(credentials.Username);

            var jwt = await Tokens.GenerateJwt(identity, _jwtFactory, credentials.Username, _jwtOptions, new JsonSerializerSettings { Formatting = Formatting.Indented }, user.ProfileComplete);
            return new OkObjectResult(jwt);
        }


        [HttpPost("externalauth/facebook")]
        public async Task<IActionResult> Facebook([FromBody]FacebookModel model)
        {
            var userInfo = new FacebookUserData();
            try
            {
                var userInfoResponse = await Client.GetStringAsync($"https://graph.facebook.com/v3.0/me?fields=id,email,first_name,last_name,gender,locale,birthday,picture&access_token={model.AccessToken}");
                userInfo = JsonConvert.DeserializeObject<FacebookUserData>(userInfoResponse);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(new List<ErrorViewModel>() { new ErrorViewModel() { Code = "InvalidToken", Description = "Facebook token is not valid." } });
            }
            if (userInfo.Picture != null)
            {
                try
                {
                    var imageResponse = await Client.GetStringAsync($"https://graph.facebook.com/v3.0/{userInfo.Id}/picture?type=album&redirect=false");
                    var image = JsonConvert.DeserializeObject<FacebookPictureData>(imageResponse);
                    userInfo.Picture = image;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            var existingUser = await _userManager.FindByEmailAsync(userInfo.Email);
            if (existingUser == null)
            {
                var user = new ApplicationUser
                {
                    FacebookId = userInfo.Id,
                    Email = userInfo.Email,
                    UserName = userInfo.FirstName + userInfo.LastName,
                    PictureUrl = userInfo.Picture?.Data?.Url,
                    Visibility = true,
                    Nationality = "US"
                };

                if (userInfo.Gender != null)
                    user.Gender = userInfo.Gender.Equals("male") ? Gender.Male : userInfo.Gender.Equals("female") ? Gender.Female : Gender.Other;
                else
                    user.Gender = Gender.None;
                if (userInfo.Birthday != DateTime.MinValue)
                {
                    var dateOfBirth = new DateTime(day: userInfo.Birthday.Day, month: userInfo.Birthday.Month, year: DateTime.Now.Year);
                    if (DateTime.Now >= dateOfBirth)
                    {
                        user.Age = DateTime.Now.Year - userInfo.Birthday.Year;
                    }
                    else
                    {
                        user.Age = DateTime.Now.Year - userInfo.Birthday.Year - 1;
                    }
                }

                user.UserName = UserExtensions.RemoveDiacritics(user.UserName);
                var result = await _userManager.CreateAsync(user, Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Substring(0, 8));
                if (!result.Succeeded) {
                    return new BadRequestObjectResult(result.Errors);
                }
                else {
                    existingUser = await _userManager.FindByNameAsync(user.UserName);
                }
            }

            var jwt = await Tokens.GenerateJwt(_jwtFactory.GenerateClaimsIdentity(existingUser.UserName, existingUser.Id),
              _jwtFactory, existingUser.UserName, _jwtOptions, new JsonSerializerSettings { Formatting = Formatting.Indented }, existingUser.ProfileComplete);

            return new OkObjectResult(jwt);
        }

        [HttpPost("externalauth/google")]
        public async Task<IActionResult> Google([FromBody] GoogleModel googleModel)
        {
            var appAccessToken = new AppAccessToken();
            var tokenResponse = await Client.PostAsync($"https://www.googleapis.com/oauth2/v4/token?code={googleModel.Code}&client_id={_googleAuthModel.ClientId}&client_secret={_googleAuthModel.ClientSecret}&redirect_uri=http://localhost/oauth2callback&grant_type=authorization_code", null);
            if (tokenResponse.StatusCode != HttpStatusCode.OK)
            {
                return new BadRequestObjectResult(new { code = "InvalidCode", description = tokenResponse.Content.ReadAsStringAsync().Result });// "Google authorization code is not valid."}); 
            }
            appAccessToken = JsonConvert.DeserializeObject<AppAccessToken>(tokenResponse.Content.ReadAsStringAsync().Result);
            var userResponse = await Client.GetStringAsync($"https://www.googleapis.com/oauth2/v2/userinfo?access_token={appAccessToken.AccessToken}");
            var userInfo = JsonConvert.DeserializeObject<GoogleUserData>(userResponse);
            var existingUser = await _userManager.FindByEmailAsync(userInfo.Email);
            if (existingUser == null)
            {
                var user = new ApplicationUser
                {
                    GoogleId = userInfo.Id,
                    Email = userInfo.Email,
                    UserName = userInfo.FirstName + userInfo.LastName,
                    PictureUrl = userInfo.Picture,
                    Visibility = true,
                    Nationality = "US"
                };
                if (userInfo.Gender != null)
                    user.Gender = userInfo.Gender.Equals("male") ? Gender.Male : userInfo.Gender.Equals("female") ? Gender.Female : Gender.Other;
                else
                    user.Gender = Gender.None;

                user.UserName = UserExtensions.RemoveDiacritics(user.UserName);
                var result = await _userManager.CreateAsync(user, Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Substring(0, 8));
                if (!result.Succeeded)
                {
                    return new BadRequestObjectResult(result.Errors);
                }
                else
                {
                    existingUser = await _userManager.FindByNameAsync(user.UserName);
                }
            }

            var jwt = await Tokens.GenerateJwt(_jwtFactory.GenerateClaimsIdentity(existingUser.UserName, existingUser.Id),
              _jwtFactory, existingUser.UserName, _jwtOptions, new JsonSerializerSettings { Formatting = Formatting.Indented }, existingUser.ProfileComplete);

            return new OkObjectResult(jwt);
        }

        private async Task<ClaimsIdentity> GetClaimsIdentity(string userName, string password)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
                return await Task.FromResult<ClaimsIdentity>(null);

            var userToVerify = await _userManager.FindByNameAsync(userName);
            if (userToVerify == null) return await Task.FromResult<ClaimsIdentity>(null);
            if (await _userManager.CheckPasswordAsync(userToVerify, password))
            {
                return await Task.FromResult(_jwtFactory.GenerateClaimsIdentity(userName, userToVerify.Id));
            }
            return await Task.FromResult<ClaimsIdentity>(null);
        }
    }
}