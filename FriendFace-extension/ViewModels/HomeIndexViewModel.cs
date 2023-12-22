using FriendFace.Models;
using Microsoft.AspNetCore.Identity;

namespace FriendFace.ViewModels;

public class HomeIndexViewModel
{
    public User User { get; set; }
    public List<Post> PostsInFeed { get; set; }
    public bool FollowingPosts { get; set; }
    public int PostCharLimit { get; set; }
}