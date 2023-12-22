using FriendFace.Services.DatabaseService;
using FriendFace.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FriendFace.Services;

public class SearchService
{
    private readonly UserQueryService _userQueryService;
    
    public SearchService(UserQueryService userQueryService)
    {
        _userQueryService = userQueryService;
    }
    
    public UserSearchListViewModel SearchUsers(string search)
    {
        var users = _userQueryService.GetUsersBySearch(search);
        
        var loggedInUser = _userQueryService.GetLoggedInUser();
        User loggedInUserIdWithFollowing = null;
        
        if (loggedInUser != null)
        {
            loggedInUserIdWithFollowing = _userQueryService.GetUserById(loggedInUser.Id);;
        }
        
        var model = new UserSearchListViewModel
        {
            Users = users,
            LoggedInUser = loggedInUserIdWithFollowing
        };
        
        return model;
    }
}