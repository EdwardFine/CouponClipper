#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;
using System;
namespace Coupon_Clipper.Models;

public class User{
    [Key]
    public int UserId {get;set;}
    [Required]
    [MinLength(3,ErrorMessage ="Username must be at least 3 characters.")]
    public string Username {get;set;}
    [Required]
    [EmailAddress]
    [UniqueEmail]
    public string Email {get;set;}
    [Required]
    [DataType(DataType.Password)]
    [VerifyPassword]
    public string Password {get;set;}
    public DateTime CreatedAt {get;set;} = DateTime.Now;
    public DateTime UpdatedAt {get;set;} = DateTime.Now;
    public List<Expired> AllExpired {get;set;} = new List<Expired>();
    public List<Coupon> AllCoupons {get;set;} = new List<Coupon>();
    public List<Clip> AllClips {get;set;} = new List<Clip>();
    [NotMapped]
    [Required]
    [DataType(DataType.Password)]
    [Compare("Password")]
    public string PasswordConfirm { get; set; }
}

public class UniqueEmailAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
    	// Though we have Required as a validation, sometimes we make it here anyways
    	// In which case we must first verify the value is not null before we proceed
        if(value == null)
        {
    	    // If it was, return the required error
            return new ValidationResult("Email is required!");
        }
    
    	// This will connect us to our database since we are not in our Controller
        MyContext _context = (MyContext)validationContext.GetService(typeof(MyContext));
        // Check to see if there are any records of this email in our database
    	if(_context.Users.Any(e => e.Email == value.ToString()))
        {
    	    // If yes, throw an error
            return new ValidationResult("Email must be unique!");
        } else {
    	    // If no, proceed
            return ValidationResult.Success;
        }
    }
}

public class VerifyPassword : ValidationAttribute{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if(value == null)
        {
            return new ValidationResult("Password is required!");
        }
        string str = value.ToString();
        if(str.Length <8){
            return new ValidationResult("Password must be at least 8 characters long!");
        }
        Regex passwordValidation = new Regex("^(?=.*?[A-Z/a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$");
        if(!passwordValidation.IsMatch(str)){
            return new ValidationResult("Must have at least 1 character, 1 number, and 1 special character!");
        }return ValidationResult.Success;
    }
}