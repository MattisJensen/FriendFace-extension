using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FriendFace.Models;

public class Post
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(280)]
    public string Content { get; set; }

    [Required]
    public DateTime Time { get; set; }

    [Required]
    public bool IsDeleted { get; set; }

    [ForeignKey("UserId")]
    public int UserId { get; set; }

    // Navigation property for the user who made the post
    public virtual User User { get; set; }

    // Navigation property for post likes
    public virtual ICollection<UserLikesPost> Likes { get; set; }

    // Navigation property for post comments
    public virtual ICollection<Comment> Comments { get; set; }
}