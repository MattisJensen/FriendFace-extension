using FriendFace.Data;

namespace FriendFace.Services.DatabaseService;

public class UserCreateService
{
    private readonly ApplicationDbContext _context;
    
    public UserCreateService(ApplicationDbContext context)
    {
        _context = context;
    }
    
}