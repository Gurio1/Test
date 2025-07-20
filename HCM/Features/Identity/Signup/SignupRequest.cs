namespace HCM.Features.Identity.Signup;

public record SignupRequest(
    string FirstName,
    string LastName,
    string Email,
    string JobTitle,
    decimal Salary,
    string Department,
    string Password,
    string ConfirmedPassword);
