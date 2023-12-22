using FriendFace.Services;
using FriendFace.Services.DatabaseService;
using Microsoft.AspNetCore.Mvc;

namespace FriendFace.Controllers;

public class SearchController : Controller
{
    private readonly SearchService _searchService;
    
    public SearchController(SearchService searchService)
    {
        _searchService = searchService;
    }
    
    public IActionResult UserSearchList(string search)
    {
        var model = _searchService.SearchUsers(search);
        return View(model);
    }
}