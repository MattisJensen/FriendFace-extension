using FriendFace.Data;

namespace FriendFace.Services.DatabaseService;

public class UserUpdateService
{
    private readonly ApplicationDbContext _context;
    
    public UserUpdateService(ApplicationDbContext context)
    {
        _context = context;
    }
}