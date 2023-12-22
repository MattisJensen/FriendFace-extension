using FriendFace.Data;

namespace FriendFace.Services.DatabaseService;

public class PostDeleteService
{
    private readonly ApplicationDbContext _context;
    
    public PostDeleteService(ApplicationDbContext context)
    {
        _context = context;
    }
    
    // removes like from post
    public bool RemoveLikeFromPost(int postId, int userId)
    {
        var existingLike = _context.UserLikesPosts.SingleOrDefault(like =>
            like.UserId == userId && like.PostId == postId);

        if (existingLike == null) return false;
        
        try
        {
            _context.UserLikesPosts.Remove(existingLike);
            _context.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
    
    public bool SoftDeletePost(int postId)
    {
        var post = _context.Posts.SingleOrDefault(p => p.Id == postId);
        if (post == null) return false;
        
        try
        {
            post.IsDeleted = true;
            _context.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
    
    public bool UndoDeletePost(int postId)
    {
        var post = _context.Posts.FirstOrDefault(p => p.Id == postId);
        if (post == null) return false;
        
        try
        {
            post.IsDeleted = false;
            _context.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
}