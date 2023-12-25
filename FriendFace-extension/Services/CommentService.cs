using FriendFace.Models;
using FriendFace.Services.DatabaseService;
using Microsoft.AspNetCore.Mvc;

namespace FriendFace.Services;

public class CommentService
{
    private readonly UserQueryService _userQueryService;
    private readonly PostQueryService _postQueryService;
    private readonly CommentCreateService _commentCreateService;
    private readonly CommentQueryService _commentQueryService;

    public CommentService(UserQueryService userQueryService, PostQueryService postQueryService,
        CommentCreateService commentCreateService, CommentQueryService commentQueryService)
    {
        _userQueryService = userQueryService;
        _postQueryService = postQueryService;
        _commentCreateService = commentCreateService;
        _commentQueryService = commentQueryService;
    }

    public string CreateComment(string content, int postId, ControllerContext controllerContext)
    {
        try
        {
            var loggedInUser = _userQueryService.GetLoggedInUser();

            if (loggedInUser == null && content.Length > _postQueryService.GetPostCharacterLimit()) return null;

            if (_commentCreateService.CreateComment(content, postId, loggedInUser))
            {
                Comment comment = new Comment
                {
                    Content = content,
                    Time = DateTime.Now,
                    PostId = postId,
                    User = loggedInUser,
                };

                List<Comment> comments = new List<Comment>();
                comments.Add(comment);

                return FileLoader.LoadFile(controllerContext, "_CommentPartial", comments);
            }
            else
            {
                return null;
            }
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public string GetPostComments(int postId, ControllerContext controllerContext)
    {
        try
        {
            var loggedInUser = _userQueryService.GetLoggedInUser();

            if (loggedInUser == null) return null;

            var comments = _commentQueryService.GetCommentsByPostId(postId);
            
            if (comments != null)
            {
                if (comments.Count == 0) return "";
                return FileLoader.LoadFile(controllerContext, "_CommentPartial", comments);
            }
            else
            {
                return null;
            }
        }
        catch (Exception ex)
        {
            return null;
        }
    }
}