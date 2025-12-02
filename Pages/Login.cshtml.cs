using Guc_Uni_System.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Guc_Uni_System.Pages
{
    public class LoginModel : PageModel
    {
        private readonly AuthService _authService;
        public LoginModel(AuthService authService)
        {
            _authService = authService;
        }

        public string Message { get; set; }

        public void OnGet() { }

        public IActionResult OnPost(string id, string password)
        {
            var result = _authService.ValidateUser(id, password);

            if (result.IsSuccess)
            {
                HttpContext.Session.SetInt32("user_id", result.UserId);
                HttpContext.Session.SetString("user_role", result.Role);

                if (result.Role == "Admin") return RedirectToPage("/AdminDashboard");
                if (result.Role == "HR") return RedirectToPage("/HRDashboard");
                return RedirectToPage("/AcademicDashboard");
            }

            Message = result.ErrorMessage;
            return Page();
        }
    }
}