using FriendFace.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using FriendFace.Data;
using FriendFace.ViewModels;
using Microsoft.Extensions.Logging;
using System.Linq;
using FriendFace.Services;
using FriendFace.Services.DatabaseService;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;

namespace FriendFace.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        private readonly UserQueryService _userQueryService;
        private readonly PostQueryService _postQueryService;

        private readonly CommentService _commentService;
        private readonly PostService _postService;

        private readonly RazorViewEngine _razorViewEngine;
        private readonly ITempDataProvider _tempDataProvider;


        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context,
            UserQueryService userQueryService, PostService postService, CommentService commentService,
            PostQueryService postQueryService)
        {
            _logger = logger;
            _context = context;

            _userQueryService = userQueryService;
            _postQueryService = postQueryService;

            _postService = postService;
            _commentService = commentService;
        }

        public IActionResult Index()
        {
            var loggedInUser = _userQueryService.GetLoggedInUser();
            if (loggedInUser == null)
            {
                var topLikedPosts = _postQueryService.GetPostsByLikes(5);

                var guestViewModel = new GuestIndexViewModel
                {
                    TopLikedPosts = topLikedPosts
                };
                return View("GuestIndex", guestViewModel);
            }
            else
            {
                var model = _postService.GetHomeIndexViewModel(true);

                return View(model);
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public IActionResult ToggleLikePost([FromBody] int postId)
        {
            if (_postService.ToggleLikePost(postId))
            {
                return Json(Ok());
            }
            else
            {
                return BadRequest(new { error = "Failed to toggle like." });
            }
        }

        [HttpGet]
        public IActionResult GetPostLikes([FromQuery] int postId)
        {
            var result = _postService.GetPostLikes(postId);
            return Json(result);
        }

        [HttpPut]
        public IActionResult DeletePost([FromBody] int postId)
        {
            if (_postService.DeletePost(postId))
            {
                return Json(Ok());
            }
            else
            {
                return BadRequest(new { error = "Failed to delete the post." });
            }
        }

        [HttpPut]
        public IActionResult EditPost([FromBody] PostIdContentModel model)
        {
            if (_postService.EditPost(model))
            {
                return Json(Ok());
            }
            else
            {
                return BadRequest(new { error = "Failed to edit the post." });
            }
        }

        [HttpPost]
        public IActionResult CreatePost([FromBody] string content)
        {
            string postAsHtmlElement = _postService.CreatePost(content, ControllerContext);
            if (postAsHtmlElement != null)
            {
                return Json(Ok(postAsHtmlElement));
            }
            else
            {
                return BadRequest(new { error = "Failed to create the post." });
            }
        }
        
        [HttpGet]
        public IActionResult GetCommentsFromPost([FromQuery] int postId)
        {
            string commentsAsHtmlElement = _commentService.GetPostComments(postId, ControllerContext);
            if (commentsAsHtmlElement != null)
            {
                return Json(Ok(commentsAsHtmlElement));
            }
            else
            {
                return BadRequest(new { error = "Failed to get comments." });
            }
        }

        [HttpPost]
        public IActionResult CreateComment([FromBody] PostIdContentModel model)
        {
            string postAsHtmlElement = _commentService.CreateComment(model.Content, model.PostId, ControllerContext);
            if (postAsHtmlElement != null)
            {
                return Json(Ok(postAsHtmlElement));
            }
            else
            {
                return BadRequest(new { error = "Failed to create the comment." });
            }
        }
        
        [HttpGet]
        public IActionResult GetFeedPosts([FromQuery] string feedtype)
        {
            FeedType type = Enum.Parse<FeedType>(feedtype, true);

            string postsAsHtmlElement = _postService.GetFeedPosts(type, ControllerContext);
            
            if (postsAsHtmlElement != null)
            {
                return Json(Ok(postsAsHtmlElement));
            }
            else
            {
                return BadRequest(new { error = "Failed to get posts." });
            }
        }
    }
}