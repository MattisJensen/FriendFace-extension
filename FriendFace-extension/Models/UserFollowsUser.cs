using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FriendFace.Models;

namespace FriendFace.Models;

public class UserFollowsUser
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("FollowerId")]
    public int FollowerId { get; set; }

    [ForeignKey("FollowingId")]
    public int FollowingId { get; set; }

    // Navigation property for the follower user
    public virtual User Follower { get; set; }

    // Navigation property for the user being followed
    public virtual User Following { get; set; }
}