using FriendFace.Data;
using FriendFace.Models;

namespace FriendFace.Services.DatabaseService;

public class PostCreateService
{
    private readonly ApplicationDbContext _context;
    private readonly PostQueryService _postQueryService;

    public PostCreateService(ApplicationDbContext context, PostQueryService postQueryService)
    {
        _context = context;
        _postQueryService = postQueryService;
    }

    public bool CreatePost(string content, User sourceUser)
    {
        if (content.Length > _postQueryService.GetPostCharacterLimit()) throw new Exception("Post content string too long.");

        try
        {
            var post = new Post
            {
                Content = content,
                Time = DateTime.UtcNow, // Uses UtcNow, such that the view can calculate the posts createTime in localtime, by comparing local timezone to UTC.
                IsDeleted = false,
                UserId = sourceUser.Id
            };

            _context.Posts.Add(post);
            _context.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    public bool AddLikeToPost(int postId, int userId)
    {
        try
        {
            var like = new UserLikesPost
            {
                PostId = postId,
                UserId = userId
            };
            
            _context.UserLikesPosts.Add(like);
            _context.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
}