using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using FriendFace.Models;
using Microsoft.AspNetCore.Identity;


public class User : IdentityUser<int>
{
    [MaxLength(50)]
    public string FirstName { get; set; }

    [MaxLength(50)]
    public string LastName { get; set; }

    // Navigation properties
    [JsonIgnore]
    public virtual ICollection<UserFollowsUser> Following { get; set; }
    
    [JsonIgnore]
    public virtual ICollection<UserFollowsUser> Followers { get; set; }
    
    [JsonIgnore]
    public virtual ICollection<Post> Posts { get; set; }
    
    [JsonIgnore]
    public virtual ICollection<UserLikesPost> Likes { get; set; }
    
    [JsonIgnore]
    public virtual ICollection<Comment> Comments { get; set; }
}