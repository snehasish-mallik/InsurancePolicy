using InsurancePolicy.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InsurancePolicy.Controllers
{
    public class AdminController : Controller
    {
        InsuranceCompanyContext dbc = new InsuranceCompanyContext();
        public IActionResult AdminHomePage()
        {
            return View();
        }
        public IActionResult LogIn()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult LogIn(Admin admin)
        {
            var validateAdmin = dbc.Admins.FirstOrDefault(x => x.Name == admin.Name && x.Password == admin.Password);
            if (validateAdmin != null)
            {
                HttpContext.Session.SetInt32("AdminId", validateAdmin.AdminId);
                HttpContext.Session.SetString("IsAdmin", "true");
                return RedirectToAction("Dashboard");
            }
            else
            {
                ViewBag.ErrorMessage = "Invalid Credentials. Check with the management team!";
                return View(admin);
            }
        }

        public IActionResult Dashboard()
        {
            var adminID = HttpContext.Session.GetInt32("AdminId");
            var adminDetails = dbc.Admins.FirstOrDefault(x => x.AdminId == adminID);
            if (adminDetails != null)
            {
                return View(adminDetails);
            }
            else
            {
                return RedirectToAction("LogIn");
            }
        }

        [HttpGet]
        public IActionResult ClaimList()
        {
            var allClaims = dbc.Claims.Include(x => x.Policy).Include(x => x.Policy.User);

            return View(allClaims);
        }

        [HttpGet]
        public IActionResult EditClaim(int ClaimId)
        {
            var claim = dbc.Claims.FirstOrDefault(x => x.ClaimId == ClaimId);
            return View(claim);
        }

        [HttpPost]
        public IActionResult EditClaim(Claim claim)
        {
            var oldClaim = dbc.Claims.FirstOrDefault(x => x.ClaimId == claim.ClaimId);
            if (oldClaim != null)
            {
                oldClaim.Status = claim.Status;
                dbc.SaveChanges();
                return RedirectToAction("ClaimList");
            }
            else
            {
                return RedirectToAction("ClaimList");
            }
        }

        //logout
        public IActionResult Logout()
        {
            // Sign out the user
            HttpContext.Session.Clear();

            // Redirect to the login page or another desired page after logout
            return RedirectToAction("LogIn");

        }
    }
}
