using FriendFace.Data;
using FriendFace.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FriendFace.Services.DatabaseService;

public class UserQueryService
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    public UserQueryService(ApplicationDbContext context, UserManager<User> userManager,
        SignInManager<User> signInManager)
    {
        _context = context;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public User GetLoggedInUser()
    {
        // Check if user is logged in
        if (_signInManager.IsSignedIn(_signInManager.Context.User))
        {
            // Get the user from the database
            var user = _userManager.GetUserAsync(_signInManager.Context.User).Result;
            if (user != null)
            {
                // Eagerly load the Following collection
                user = _context.Users
                    .Include(u => u.Following)
                    .FirstOrDefault(u => u.Id == user.Id);
            }
            return user;
        }
        else
        {
            // User is not logged in
            return null;
        }
    }

    public User GetUserById(int userId)
    {
        return _context.Users
            .Include(u => u.Following) // Load the users UserA follows
            .Include(u => u.Followers) // Load the users following UserA
            .FirstOrDefault(u => u.Id == userId) ?? throw new InvalidOperationException();
    }

    public User GetSimpleUserById(int userId)
    {
        return _context.Users
            .FirstOrDefault(u => u.Id == userId);
    }

    public List<int> GetFollowingUserIds(int userId)
    {
        var user = GetUserById(userId);
        return user.Following.Select(f => f.FollowingId).ToList();
    }
    
    public List<User> GetUsersBySearch(string search)
    {
        // Split the search string into terms
        var searchTerms = search.ToLower().Split(" ").ToList();
        var searchResult = new List<User>();
        
        foreach (var term in searchTerms)
        {
            var resultsForTerm = _context.Users
                .Where(user =>
                    user.UserName.ToLower().Contains(term) ||
                    user.FirstName.ToLower().Contains(term) ||
                    user.LastName.ToLower().Contains(term) ||
                    user.Email.ToLower().Contains(term)
                )
                .ToList();

            searchResult.AddRange(resultsForTerm);
        }
    
        return searchResult.Distinct().ToList();
    }

    public async Task<bool> FollowUser(int currentUserId, int userIdToFollow)
    {
        var currentUser = await _context.Users
            .Include(u => u.Following)
            .FirstOrDefaultAsync(u => u.Id == currentUserId);

        var userToFollow = await _context.Users
            .Include(u => u.Followers)
            .FirstOrDefaultAsync(u => u.Id == userIdToFollow);

        if (currentUser == null || userToFollow == null)
        {
            return false;
        }

        // Check if the follow relationship already exists
        if (!currentUser.Following.Any(f => f.FollowingId == userIdToFollow))
        {
            currentUser.Following.Add(new UserFollowsUser
            {
                FollowerId = currentUserId,
                FollowingId = userIdToFollow
            });

            await _context.SaveChangesAsync();
        }

        return true;
    }
    public async Task<bool> UnfollowUser(int currentUserId, int userIdToUnfollow)
    {
        // Find the existing follow relationship
        var followRelation = await _context.UserFollowsUsers
            .FirstOrDefaultAsync(f => f.FollowerId == currentUserId && f.FollowingId == userIdToUnfollow);

        if (followRelation != null)
        {
            // Remove the follow relationship from the context
            _context.UserFollowsUsers.Remove(followRelation);
            await _context.SaveChangesAsync();
            return true;
        }

        return false;
    }

}