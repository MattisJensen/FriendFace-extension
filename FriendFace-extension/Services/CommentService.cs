using FriendFace.Services.DatabaseService;

namespace FriendFace.Services;

public class CommentService
{
    private readonly UserQueryService _userQueryService;
    private readonly PostQueryService _postQueryService;
    private readonly CommentCreateService _commentCreateService;

    public CommentService(UserQueryService userQueryService, PostQueryService postQueryService, CommentCreateService commentCreateService)
    {
        _userQueryService = userQueryService;
        _postQueryService = postQueryService;
        _commentCreateService = commentCreateService;
    }
    
    public object CreateComment(string content, int postId)
    {
        try
        {
            var loggedInUser = _userQueryService.GetLoggedInUser();

            if (loggedInUser != null && loggedInUser.Id > 0)
            {
                // Check if the edited content is within the character limit
                if (content.Length <= _postQueryService.GetPostCharacterLimit())
                {
                    return new { success = _commentCreateService.CreateComment(content, postId, loggedInUser) };
                }
                else
                {
                    return new { success = false, message = "Content exceeds " + _postQueryService.GetPostCharacterLimit() + " characters." };
                }
            }
            else
            {
                return new { success = false, message = "You do not have permission to create this post." };
            }
        }
        catch (Exception ex)
        {
            return new { success = false, message = "An error occurred while creating the post." };
        }
    }
}