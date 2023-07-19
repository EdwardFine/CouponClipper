#pragma warning disable CS8618

using Microsoft.EntityFrameworkCore;
namespace Coupon_Clipper.Models;

public class MyContext : DbContext 
{   
    public MyContext(DbContextOptions options) : base(options) { }      
    public DbSet<User> Users { get; set; } 
    public DbSet<Coupon> Coupons { get; set; } 
    public DbSet<Clip> Clips { get; set; } 
    public DbSet<Expired> Expired { get; set; } 
}