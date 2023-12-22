using FriendFace.Services.DatabaseService;
using FriendFace.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FriendFace.Controllers
{
    public class UserController : Controller
    {
        private readonly UserQueryService _userQueryService;
        private readonly ILogger<UserController> _logger;
        private readonly UserManager<User> _userManager;


        public UserController(ILogger<UserController> logger, UserQueryService userQueryService, UserManager<User> userManager)
        {
            _userQueryService = userQueryService;
            _logger = logger;
            _userManager = userManager;

        }

        [HttpGet("User/Profile/{userId}")]
        public IActionResult Profile(int userId)
        {
            // Retrieve the profile user based on the userId.
            _logger.LogInformation("Profile action called with userId: {UserId}", userId);

            var profileUser = _userQueryService.GetSimpleUserById(userId); 
            if (profileUser == null)
            {
                _logger.LogWarning("No user found with userId: {UserId}", userId);
                return NotFound();
            }

            var loggedInUser = _userQueryService.GetLoggedInUser();
            bool isLoggedIn = loggedInUser != null;
            bool isCurrentUser = loggedInUser != null && loggedInUser.Id == profileUser.Id;
            bool isFollowing = loggedInUser != null &&
                    loggedInUser.Following.Any(f => f.FollowingId == profileUser.Id);

            _logger.LogInformation("Profile found. Current user: {IsCurrentUser}", isCurrentUser);

            // Create the ViewModel for the view.
            var viewModel = new UserProfileViewModel
            {
                user = profileUser,
                isCurrentUser = isCurrentUser,
                isFollowing = isFollowing,
                isLoggedIn = isLoggedIn,

                // Populate other properties of the ViewModel as needed.
            };

            // Pass the ViewModel to the view.
            return View("ViewProfile", viewModel);
        }


        [HttpGet("User/Profile/EditProfile")]
        public IActionResult EditProfile()
        {
            // Create an empty UserProfileViewModel or populate it with initial data
            var model = new UserProfileViewModel();

            // Optionally, you can pre-populate the model with user data if needed.

            return View("EditProfile", model);
        }


        [HttpPost("User/Profile/SaveEditProfile")]
        public async Task<IActionResult> SaveEditProfile(UserProfileViewModel model)
        {
            var user = await _userManager.GetUserAsync(User); // Retrieve the current user
            if (user == null)
            {
                // Handle the case where the user is not found
                return NotFound(); // or another appropriate response
            }

            // Update Email if it's provided and different from the current email
            if (!string.IsNullOrWhiteSpace(model.user.Email) && model.user.Email != user.Email)
            {
                user.Email = model.user.Email;
                // You may also want to handle email confirmation logic here
            }

            // Update other fields similarly...
            // if (!string.IsNullOrWhiteSpace(model.user.FirstName) && model.user.FirstName != user.FirstName)
            // {
            //     user.FirstName = model.user.FirstName;
            // }

            // Handle password change if provided
            if (!string.IsNullOrWhiteSpace(model.ChangePasswordViewModel?.NewPassword))
            {
                if (string.IsNullOrWhiteSpace(model.ChangePasswordViewModel.CurrentPassword))
                {
                    ModelState.AddModelError("ChangePasswordViewModel.CurrentPassword", "Current password is required.");
                    return View("EditProfile", model);
                }

                var checkPassword = await _userManager.CheckPasswordAsync(user, model.ChangePasswordViewModel.CurrentPassword);
                if (!checkPassword)
                {
                    ModelState.AddModelError("ChangePasswordViewModel.CurrentPassword", "Current password is not correct.");
                    return View("EditProfile", model);
                }

                if (model.ChangePasswordViewModel.NewPassword != model.ChangePasswordViewModel.ConfirmNewPassword)
                {
                    ModelState.AddModelError("ChangePasswordViewModel.ConfirmNewPassword", "The new password and confirmation password do not match.");
                    return View("EditProfile", model);
                }

                var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.ChangePasswordViewModel.CurrentPassword, model.ChangePasswordViewModel.NewPassword);
                if (!changePasswordResult.Succeeded)
                {
                    foreach (var error in changePasswordResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View("EditProfile", model);
                }
            }


            // Save changes to the user
            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                // Handle errors
                foreach (var error in updateResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View("EditProfile", model);
            }

            // Redirect or return a view upon successful update
            return RedirectToAction("Profile", "User", new {userId = _userQueryService.GetLoggedInUser().Id }); // or another appropriate view
        }

        [HttpGet] // Change to HttpGet for the integration with your JavaScript
        public async Task<IActionResult> FollowUser(int userIdToFollow)
        {
            var loggedInUserId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(loggedInUserId))
            {
                return Unauthorized();
            }

            var result = await _userQueryService.FollowUser(int.Parse(loggedInUserId), userIdToFollow);
            if (!result)
            {
                return BadRequest();
            }

            return RedirectToAction("Profile", new { userId = userIdToFollow });
        }

        [HttpGet] // Change to HttpGet for the integration with your JavaScript
        public async Task<IActionResult> UnfollowUser(int userIdToUnfollow)
        {
            var loggedInUserId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(loggedInUserId))
            {
                return Unauthorized();
            }

            var result = await _userQueryService.UnfollowUser(int.Parse(loggedInUserId), userIdToUnfollow);
            if (!result)
            {
                return BadRequest();
            }

            return RedirectToAction("Profile", new { userId = userIdToUnfollow });
        }
    }

}
