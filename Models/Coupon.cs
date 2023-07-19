#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;
using System;
namespace Coupon_Clipper.Models;

public class Coupon{
    [Key]
    public int CouponId {get;set;}
    [Required]
    public string Code {get;set;}
    [Required]
    public string Source {get;set;}
    [Required]
    [MinLength(10,ErrorMessage ="Description must be at least 10 characters.")]
    public string Description {get;set;}
    public DateTime CreatedAt {get;set;} = DateTime.Now;
    public DateTime UpdatedAt {get;set;} = DateTime.Now;
    public int UserId {get;set;}
    public User? Creator {get;set;}
    public List<Expired> AllExpired {get;set;} = new List<Expired>();
    public List<Clip> AllClips {get;set;} = new List<Clip>();
}