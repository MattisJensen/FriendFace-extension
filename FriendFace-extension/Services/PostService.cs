using FriendFace.Data;
using FriendFace.Models;
using FriendFace.Services;
using FriendFace.Services.DatabaseService;
using FriendFace.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

public class PostService
{
    private readonly PostCreateService _postCreateService;
    private readonly PostQueryService _postQueryService;
    private readonly PostUpdateService _postUpdateService;
    private readonly PostDeleteService _postDeleteService;
    private readonly UserQueryService _userQueryService;

    public PostService(PostCreateService postCreateService, PostQueryService postQueryService,
        PostUpdateService postUpdateService, PostDeleteService postDeleteService, UserQueryService userQueryService)
    {
        _postCreateService = postCreateService;
        _postQueryService = postQueryService;
        _postUpdateService = postUpdateService;
        _postDeleteService = postDeleteService;
        _userQueryService = userQueryService;
    }

    public HomeIndexViewModel GetHomeIndexViewModel(bool followingPosts)
    {
        var loggedInUser = _userQueryService.GetLoggedInUser();

        List<Post> postsInFeed;

        postsInFeed = followingPosts ? _postQueryService.GetLatestPostsFromFeed(loggedInUser.Id) : _postQueryService.GetPostsFromUserId(loggedInUser.Id);

        var postCharLimit = _postQueryService.GetPostCharacterLimit();

        var model = new HomeIndexViewModel
        {
            User = loggedInUser,
            PostsInFeed = postsInFeed,
            FollowingPosts = followingPosts,
            PostCharLimit = postCharLimit
        };
        return model;
    }

    public void ToggleLikePost(int postId)
    {
        var post = _postQueryService.GetPostFromId(postId);

        if (post == null) throw new InvalidOperationException("Post not found");

        var user = _userQueryService.GetLoggedInUser();

        try
        {
            var existingLike = _postQueryService.HasUserLikedPost(user.Id, postId);

            if (existingLike)
            {
                // If a like by the user already exists, remove it
                _postDeleteService.RemoveLikeFromPost(postId, user.Id);
            }
            else
            {
                _postCreateService.AddLikeToPost(postId, user.Id);
            }
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while toggling the like on the post. ", ex);
        }
    }

    public object GetPostLikes(int postId)
    {
        var post = _postQueryService.GetPostFromId(postId);

        if (post == null) throw new InvalidOperationException("Post not found");

        var user = _userQueryService.GetLoggedInUser();

        try
        {
            var isLiked = _postQueryService.HasUserLikedPost(user.Id, postId);
            var likeCount = _postQueryService.GetNumberOfLikes(postId);

            return new { likeCount = likeCount, isLiked = isLiked };
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while retrieving post likes", ex);
        }
    }

    public void DeletePost(int postId)
    {
        try
        {
            // Check if the logged-in user is the owner of the post
            var loggedInUser = _userQueryService.GetLoggedInUser();
            if (loggedInUser == null)
            {
                throw new Exception("You must be logged in to delete a post.");
            }
            
            var post = _postQueryService.GetPostFromId(postId);

            if (post.UserId == loggedInUser.Id)
            {
                if (!_postDeleteService.SoftDeletePost(postId))
                {
                    throw new Exception("An error occurred while deleting the post.");
                }
            }
            else
            {
                throw new Exception("You do not have permission to delete this post.");
            }
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while deleting the post.", ex);
        }
    }

    public void EditPost(PostIdContentModel model)
    {
        try
        {
            int postId = model.PostId;
            string editedContent = model.Content;

            var loggedInUser = _userQueryService.GetLoggedInUser();
            if (loggedInUser == null)
            {
                throw new Exception("You must be logged in to edit a post.");
            }
            
            var post = _postQueryService.GetPostFromId(postId);
            
            if (post.UserId == loggedInUser.Id)
            {
                // Check if the edited content is within the character limit
                if (editedContent.Length <= _postQueryService.GetPostCharacterLimit())
                {
                    if (!_postUpdateService.UpdatePost(postId, editedContent)) 
                    {
                        throw new Exception("An error occurred while editing the post.");
                    }
                }
                else
                {
                    throw new Exception("Edited content exceeds " + _postQueryService.GetPostCharacterLimit() + " characters.");
                }
            }
            else
            {
                throw new Exception("You do not have permission to edit this post.");
            }
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while editing the post.", ex);
        }
    }

    public void CreatePost(string content, ControllerContext controllerContext)
    {
        try
        {
            // Check if the logged-in user is the owner of the post
            var loggedInUser = _userQueryService.GetLoggedInUser();

            // Check if logged in user is valid and if edited content is within character limit
            if (loggedInUser != null && loggedInUser.Id > 0 &&
                content.Length <= _postQueryService.GetPostCharacterLimit())
            {
                if (!_postCreateService.CreatePost(content, loggedInUser))
                {
                    throw new Exception("An error occurred while creating the post.");
                }
            }
            else
            {
                throw new Exception("Content exceeds " + _postQueryService.GetPostCharacterLimit() +
                                    " characters.");
            }
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while creating the post.", ex);
        }
    }
}