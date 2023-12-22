namespace FriendFace.ViewModels
{


    public class UserProfileViewModel

    {

        public User user { get; set; }

        public bool isLoggedIn { get; set; }

        public bool isCurrentUser { get; set; }

        public bool isFollowing { get; set; } // Add this property

        public ChangePasswordViewModel ChangePasswordViewModel { get; set; }


        // This will be used just to display the existing profile picture

        // This will be used to upload a new profile picture
        // public HttpPostedFileBase ProfilePicture { get; set; }
    }

}
