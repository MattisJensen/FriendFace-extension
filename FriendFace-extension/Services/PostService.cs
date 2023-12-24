using FriendFace.Data;
using FriendFace.Models;
using FriendFace.Services;
using FriendFace.Services.DatabaseService;
using FriendFace.ViewModels;
using Microsoft.AspNetCore.Http.HttpResults;
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

        postsInFeed = followingPosts
            ? _postQueryService.GetLatestPostsFromFeed(loggedInUser.Id)
            : _postQueryService.GetPostsFromUserId(loggedInUser.Id);

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

    public bool ToggleLikePost(int postId)
    {
        var post = _postQueryService.GetPostFromId(postId);

        if (post == null) return false;

        var user = _userQueryService.GetLoggedInUser();

        try
        {
            var existingLike = _postQueryService.HasUserLikedPost(user.Id, postId);

            if (existingLike)
            {
                // If a like by the user already exists, remove it
                return _postDeleteService.RemoveLikeFromPost(postId, user.Id);
            }
            else
            {
                return _postCreateService.AddLikeToPost(postId, user.Id);
            }
        }
        catch (Exception ex)
        {
            return false;
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

    public bool DeletePost(int postId)
    {
        try
        {
            // Check if the logged-in user is the owner of the post
            var loggedInUser = _userQueryService.GetLoggedInUser();
            if (loggedInUser == null) return false;

            var post = _postQueryService.GetPostFromId(postId);

            if (post.UserId == loggedInUser.Id)
            {
                return _postDeleteService.SoftDeletePost(postId);
            }
            else
            {
                return false;
            }
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    public bool EditPost(PostIdContentModel model)
    {
        try
        {
            int postId = model.PostId;
            string editedContent = model.Content;

            var loggedInUser = _userQueryService.GetLoggedInUser();
            if (loggedInUser == null) return false;

            var post = _postQueryService.GetPostFromId(postId);

            if (post.UserId == loggedInUser.Id && editedContent.Length <= _postQueryService.GetPostCharacterLimit())
            {
                return _postUpdateService.UpdatePost(postId, editedContent);
            }
            else
            {
                return false;
            }
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    public string CreatePost(string content, ControllerContext controllerContext)
    {
        try
        {
            // Check if the logged-in user is the owner of the post
            var loggedInUser = _userQueryService.GetLoggedInUser();
            var charLimit = _postQueryService.GetPostCharacterLimit();

            // Check if logged in user is valid and if edited content is within character limit
            if (loggedInUser == null || content.Length > charLimit) return null;
                if (_postCreateService.CreatePost(content, loggedInUser))
                {
                    var createdPost = _postQueryService.GetLatestPostFromUserId(loggedInUser.Id);
                    List<Post> posts = new List<Post>();
                    posts.Add(createdPost);
                    
                    return FileLoader.LoadFile(controllerContext, "_PostFeedPartial", new HomeIndexViewModel()
                    {
                        User = loggedInUser,
                        PostsInFeed = posts,
                        FollowingPosts = false,
                        PostCharLimit = charLimit
                    });
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

    public string GetFeedPosts(FeedType type, ControllerContext controllerContext)
    {
        var loggedInUser = _userQueryService.GetLoggedInUser();
        if (loggedInUser == null) return null;

        List<Post> posts;
        bool followingPosts = false;
        
        switch (type)
        {
            case FeedType.Profile:
                posts = _postQueryService.GetPostsFromUserId(loggedInUser.Id);
                followingPosts = false;
                break;
            case FeedType.Following:
                posts = _postQueryService.GetLatestPostsFromFeed(loggedInUser.Id);
                followingPosts = true;
                break;
            default:
                posts = null;
                break;
        }

        if (posts == null) return null;
        
        return FileLoader.LoadFile(controllerContext, "_PostFeedPartial", new HomeIndexViewModel()
        {
            User = loggedInUser,
            PostsInFeed = posts,
            FollowingPosts = followingPosts,
            PostCharLimit = _postQueryService.GetPostCharacterLimit()
        });
    }
}