using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Identity;
using Coupon_Clipper.Models;

namespace Coupon_Clipper.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private MyContext db;

    public HomeController(ILogger<HomeController> logger,MyContext context)
    {
        _logger = logger;
        db = context;
    }

    public IActionResult Index()
    {
        return View();
    }

    [SessionCheck]
    [HttpGet("dashboard")]
    public IActionResult Dashboard(){
        User? ActiveUser = db.Users.FirstOrDefault(u=>u.UserId == HttpContext.Session.GetInt32("UserId"));
        ViewBag.User = ActiveUser;
        List<Coupon> AllCoupons = db.Coupons.Include(c=>c.Creator).Include(c=>c.AllClips).Include(c=>c.AllExpired).ToList();
        return View(AllCoupons);
    }

    //User Create, Login, Signout
    [HttpPost("user/create")]
    public IActionResult Register(User newUser){
        if(!ModelState.IsValid){
            return View("Index");
        }
        PasswordHasher<User> Hasher = new PasswordHasher<User>();
        newUser.Password = Hasher.HashPassword(newUser, newUser.Password);
        db.Users.Add(newUser);
        db.SaveChanges();
        HttpContext.Session.SetInt32("UserId",newUser.UserId);
        return RedirectToAction("Dashboard");
    }

    [HttpPost("user/login")]
    public IActionResult Login(LoginUser loginUser){
        if(!ModelState.IsValid){
            return View("Index");
        }
        User? UserInDb = db.Users.FirstOrDefault(u=>u.Email == loginUser.Email);
        if(UserInDb == null){
            ModelState.AddModelError("Email", "Invalid Email/Password");    
            return View("Index");
        }
        PasswordHasher<LoginUser> hasher = new PasswordHasher<LoginUser>();
        var result = hasher.VerifyHashedPassword(loginUser, UserInDb.Password, loginUser.Password);
        if(result ==0){
            ModelState.AddModelError("Email", "Invalid Email/Password");            
            return View("Index");    
        }
        HttpContext.Session.SetInt32("UserId",UserInDb.UserId);
        return RedirectToAction("Dashboard");
    }

    [HttpPost("logout")]
    public IActionResult Logout(){
        HttpContext.Session.Clear();
        return RedirectToAction("Index");
    }

    //Create Coupon
    [SessionCheck]
    [HttpGet("coupon/new")]
    public IActionResult NewCoupon(){
        User? ActiveUser = db.Users.FirstOrDefault(u=>u.UserId == HttpContext.Session.GetInt32("UserId"));
        ViewBag.User = ActiveUser;
        return View();
    }

    [SessionCheck]
    [HttpPost("coupon/create")]
    public IActionResult CreateCoupon(Coupon newCoupon){
        if(!ModelState.IsValid){
            User? ActiveUser = db.Users.FirstOrDefault(u=>u.UserId == HttpContext.Session.GetInt32("UserId"));
            ViewBag.User = ActiveUser;
            return View("NewCoupon");
        }
        db.Coupons.Add(newCoupon);
        db.SaveChanges();
        return RedirectToAction("Dashboard");
    }

    //Clip Coupon
    [SessionCheck]
    [HttpPost("coupon/clip")]
    public IActionResult ClipCoupon(Clip newClip){
        db.Clips.Add(newClip);
        db.SaveChanges();
        return RedirectToAction("Dashboard");
    }

    //Add Expired
    [SessionCheck]
    [HttpPost("expire/add")]
    public IActionResult AddExpire(Expired newExpire){
        db.Expired.Add(newExpire);
        db.SaveChanges();
        return RedirectToAction("Dashboard");
    }

    //Account info
    [SessionCheck]
    [HttpGet("user/{id}")]
    public IActionResult ViewUser(int id){
        User? ActiveUser = db.Users.FirstOrDefault(u=>u.UserId == HttpContext.Session.GetInt32("UserId"));
        ViewBag.User = ActiveUser;
        User? UserWithCoupons = db.Users.Include(u=>u.AllClips).Include(u=>u.AllExpired).Include(u=>u.AllCoupons).ThenInclude(c=>c.AllClips).FirstOrDefault(u=>u.UserId==id);
        return View(UserWithCoupons);
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

public class SessionCheckAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        // Find the session, but remember it may be null so we need int?
        int? userId = context.HttpContext.Session.GetInt32("UserId");
        // Check to see if we got back null
        if(userId == null)
        {
            // Redirect to the Index page if there was nothing in session
            // "Home" here is referring to "HomeController", you can use any controller that is appropriate here
            context.Result = new RedirectToActionResult("Index", "Home", null);
        }
    }
}