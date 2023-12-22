using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FriendFace.Models;

public class UserLikesPost
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("UserId")]
    public int UserId { get; set; }

    [ForeignKey("PostId")]
    public int PostId { get; set; }

    // Navigation property for the user who liked the post
    public virtual User User { get; set; }

    // Navigation property for the post that was liked
    public virtual Post Post { get; set; }
}