using FriendFace.Data;
using FriendFace.Models;

namespace FriendFace.Services.DatabaseService;

public class PostUpdateService
{
    private readonly ApplicationDbContext _context;

    public PostUpdateService(ApplicationDbContext context)
    {
        _context = context;
    }

    public bool UpdatePost(int postId, string updatedContent)
    {
        if (_context.Posts.Find(postId) == null) throw new KeyNotFoundException();

        try
        {
            var orgPost = _context.Posts.Find(postId);

            var post = orgPost;
            post.Content = updatedContent;
            _context.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
}