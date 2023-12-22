using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FriendFace.Models;

public class Comment
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(280)] // Assuming a character limit for comments
    public string Content { get; set; }

    [Required]
    public DateTime Time { get; set; }

    [ForeignKey("UserId")]
    public int UserId { get; set; }

    [ForeignKey("PostId")]
    public int PostId { get; set; }

    // Navigation property for the user who made the comment
    public virtual User User { get; set; }

    // Navigation property for the post on which the comment was made
    public virtual Post Post { get; set; }
}