#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;
using System;
namespace Coupon_Clipper.Models;

public class Expired{
    [Key]
    public int ExpiredId {get;set;}
    [Required]
    public int UserId {get;set;}
    [Required]
    public int CouponId {get;set;}
    public User? User {get;set;}
    public Coupon? Coupon {get;set;}
    public DateTime CreatedAt {get;set;} = DateTime.Now;
    public DateTime UpdatedAt {get;set;} = DateTime.Now;
}