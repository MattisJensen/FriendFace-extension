using FriendFace.Data;
using FriendFace.Models;
using Microsoft.EntityFrameworkCore;

namespace FriendFace.Services.DatabaseService;

public class CommentQueryService
{
    private readonly ApplicationDbContext _context;

    public CommentQueryService(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<Comment> GetCommentsByPostId(int postId)
    {
        return _context.Comments
            .Include(c => c.User)
            .Include(c => c.Post)
            .OrderByDescending(c => c.Time)
            .Where(p => p.PostId == postId)
            .ToList();
    }
}