using InsurancePolicy.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Claim = InsurancePolicy.Models.Claim;

namespace InsurancePolicy.Controllers
{
    public class UserController : Controller
    {
        InsuranceCompanyContext dbc = new InsuranceCompanyContext();
        public IActionResult UserHomePage()
        {
            return View();
        }
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(User user)
        {
            if (dbc.Users.Any(x => x.Name == user.Name))
            {
                ModelState.AddModelError("Name", "Username already exists");
                return View(user);
            }
            else
            {
                dbc.Users.Add(user);
                dbc.SaveChanges();
                return RedirectToAction("LogIn");
            }
        }

        //login
        [HttpGet]
        public IActionResult LogIn()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult LogIn(User user)
        {
            var loginUser = dbc.Users.FirstOrDefault(x => x.Name == user.Name && x.Password == user.Password);
            if (loginUser != null)
            {
                // Log in the user and store user information in the session
                HttpContext.Session.SetInt32("UserId", loginUser.UserId);
                HttpContext.Session.SetString("IsAdmin", "false");
                return RedirectToAction("Dashboard");
            }
            else
            {
                ViewBag.ErrorMessage = "User Doesn't Exist. Register First";
                return View(user);
            }
        }

        public IActionResult Dashboard()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var user = dbc.Users.FirstOrDefault(x => x.UserId == userId);
            if (user != null)
            {
                return View(user);
            }
            else
            {
                ViewBag.ErrorMessage = "Log in First";
                return RedirectToAction("LogIn");
            }
        }

        //policy operations
        //policy list
        public IActionResult PolicyList()
        {
            var userID = HttpContext.Session.GetInt32("UserId");
            var user = dbc.Users.FirstOrDefault(x => x.UserId == userID);
            if (user != null)
            {
                var userPolicies = dbc.Policies.Where(x => x.UserId == user.UserId).ToList();
                return View(userPolicies);
            }
            else
            {
                ViewBag.ErrorMessage = "Log in First";
                return RedirectToAction("LogIn");
            }
        }

        //create a policy list
        [HttpGet]
        public IActionResult CreatePolicy()
        {
            var userID = HttpContext.Session.GetInt32("UserId");
            ViewBag.UserID= userID;
            return View();

        }

        [HttpPost]
        public IActionResult CreatePolicy(Policy policy)
        {
            var userID= HttpContext.Session.GetInt32("UserId");
            if (userID != null)
            {
                policy.UserId= userID;
                dbc.Policies.Add(policy);
                dbc.SaveChanges();
                return RedirectToAction("PolicyList");
            }
            else
            {
                ViewBag.ErrorMessage = "Log in First";
                return RedirectToAction("LogIn");
            }
        }

        //Edit policy list
        [HttpGet]
        public IActionResult EditPolicy(int PolicyId)
        {
            var userID = HttpContext.Session.GetInt32("UserId");
            if(userID != null)
            {
                var policyD = dbc.Policies.FirstOrDefault(x => x.PolicyId == PolicyId);
                
                return View(policyD);
            }
            else
            {
                ViewBag.ErrorMessage = "Log in First";
                return RedirectToAction("LogIn");
            }

        }
        [HttpPost]
        public IActionResult EditPolicy(Policy policy)
        {
            var userID= HttpContext.Session.GetInt32("UserId");
            if(User != null)
            {
                var oldPolicy= dbc.Policies.FirstOrDefault(x=>x.PolicyId==policy.PolicyId);
                oldPolicy.PolicyName = policy.PolicyName;
                oldPolicy.PolicyType= policy.PolicyType;
                oldPolicy.UserId = userID;
                if (policy.PolicyName == "Home")
                {
                    oldPolicy.VehicleRegistrationNum = null;
                    oldPolicy.Pincode = policy.Pincode;
                }
                if (policy.PolicyName == "Motor")
                {
                    oldPolicy.Pincode = null;
                    oldPolicy.VehicleRegistrationNum= policy.VehicleRegistrationNum;
                }
                
                dbc.SaveChanges();
                return RedirectToAction("PolicyList");

            }
            else
            {
                ViewBag.ErrorMessage = "Log in First";
                return RedirectToAction("LogIn");

            }
        }

        //claim operations
        //claimlist
        public IActionResult ClaimList()
        {
            var userID = HttpContext.Session.GetInt32("UserId");
            var user = dbc.Users.FirstOrDefault(x => x.UserId == userID);
            if (user != null)
            {
                var userClaims = dbc.Claims.Include(x => x.Policy).Where(x => x.Policy.UserId == user.UserId);
                
                return View(userClaims);
            }
            else
            {
                ViewBag.ErrorMessage = "Log in First";
                return RedirectToAction("LogIn");
            }
        }

        //creating claim
        [HttpGet]
        public IActionResult CreateClaim()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var userPolicies = dbc.Policies.Where(p => p.UserId == userId).ToList();

            ViewBag.UserPolicies = userPolicies;
            return View();
        }

        [HttpPost]
        public IActionResult CreateClaim(Claim claim)
        {
            
            if (ModelState.IsValid)
            {
                var policyID = claim.PolicyId;
                var policyD= dbc.Policies.FirstOrDefault(x=>x.PolicyId == policyID);

                claim.Status = "Pending";
                if (policyD.PolicyType == "Premium")
                {
                    claim.ClaimAmount = (policyD.PolicyAmount) * .90;
                }
                else
                {
                    claim.ClaimAmount = (policyD.PolicyAmount) * 0.70;
                }
                dbc.Add(claim);
                dbc.SaveChanges();
                return RedirectToAction("ClaimList");
               
            }
            else
            {
                return View(claim);
            }
        }
        //editing claim

        [HttpGet]
        public IActionResult DeleteClaim(int claimId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if(userId != null)
            {
                var claim= dbc.Claims.FirstOrDefault(x=>x.ClaimId == claimId);  
                return View(claim);
            }
            else
            {
                ViewBag.ErrorMessage = "Log in First";
                return RedirectToAction("LogIn");
            }
        }
        [HttpPost]
        public IActionResult DeleteClaim(Claim claim)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId != null)
            {
                var oldclaim= dbc.Claims.FirstOrDefault(x=>x.ClaimId==claim.ClaimId);
                dbc.Claims.Remove(oldclaim); 
                dbc.SaveChanges();
                return RedirectToAction("ClaimList");
                
            }
            else
            {
                ViewBag.ErrorMessage = "Log in First";
                return RedirectToAction("LogIn");
            }
        }

        //Logout
        public IActionResult Logout()
        {
            // Sign out the user
            HttpContext.Session.Clear();

            // Redirect to the login page or another desired page after logout
            return RedirectToAction("LogIn");

        }

    }
}
