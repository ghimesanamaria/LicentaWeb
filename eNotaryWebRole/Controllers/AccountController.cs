using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;
using eNotaryWebRole.Filters;
using eNotaryWebRole.Models;
using SBClient;
using System.Security.Cryptography;
using System.Text;
using eNotaryWebRole.Code;

namespace eNotaryWebRole.Controllers
{
    //[Authorize]
    //[InitializeSimpleMembership]
    public class AccountController : Controller
    {
        //
        // GET: /Account/Login

        private eNotaryDBEntities1 _db = new eNotaryDBEntities1();

        [AllowAnonymous]
        [RequireHttps]
       
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            

            HttpClientCertificate cert = Request.ClientCertificate;

            return View();
        }

        //
        // POST: /Account/Login

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
       
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            //if (ModelState.IsValid && WebSecurity.Login(model.UserName, model.Password, persistCookie: model.RememberMe))
            //{
            //    return RedirectToLocal(returnUrl);
            //}


            if (ModelState.IsValid)
            {

                string userNameLdap = model.UserName;
                if (Membership.ValidateUser(model.UserName, model.Password))
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                    if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Utilizatorul sau parola sunt incorecte!");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);


            HttpClientCertificate ret = Request.ClientCertificate;

            //// If we got this far, something failed, redisplay form
            //ModelState.AddModelError("", "The user name or password provided is incorrect.");
            return View(model);
        }

        //
        // POST: /Account/LogOff

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
           // WebSecurity.Logout();
            FormsAuthentication.SignOut();

            return RedirectToAction("Login", "Account");
        }

        //
        // GET: /Account/Register

       [AllowAnonymous]
        public ActionResult Register(long id = 0) // id =0 when you create a new account
        {
            PersonDetail model;
            if (id != 0)
            {
                model = _db.PersonDetails.Where(p => p.ID == id && p.Disabled == false).FirstOrDefault();
                ViewBag.Button = "Salveaza";
                ViewBag.Action = "Edit";
            }
            else
            {
                model = new PersonDetail();
                ViewBag.Button = "Inregistrati-va acum";
                ViewBag.Action = "Register";
            }
           

            List<string> jobTypeList = new List<string>();
           jobTypeList = ( from j in _db.JobTypes
                               select j.JobName).ToList();

           ViewBag.JobType = new SelectList(jobTypeList);
           List<string> educationLevelList = new List<string>();
           educationLevelList = (from e in _db.EducationLevels
                                 select e.EducationLevel1).ToList();
           ViewBag.EducationLevel = new SelectList(educationLevelList);

            return View(model);
        }


       [AllowAnonymous]
       public ActionResult Username(long id )
       {
           User user;

           if (id == 0)
           {
               // register
               user = new User();
           }
           else
           {
               user = (from p in  _db.PersonDetails
                       join u in _db.Users
                       on p.ID equals u.PersonID
                       select u).FirstOrDefault() ;
           }
           return PartialView("Username",user);
       }


        // GET
         [AllowAnonymous]
        //partial view for address module
       public ActionResult Address(long? id )
       {
           // change this 

          // string username = "admin";
           string username = User.Identity.Name;
           string message = "";

           // verify more careful if the address is for that specific user

           Address model ;
           if (id != 0)
           {
               model = (
                        from p in _db.PersonDetails                        
                        join a in _db.Addresses
                        on p.AddressID equals a.ID
                        where a.ID == id
                        select a).SingleOrDefault();
           }
           else
           {
               model = new Address();
           }

           return PartialView(model);
       }

      
        //
        // POST: /Account/Register

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(FormCollection collection)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                try
                {
                    string message = "";
                    long user_id = 0;
                    Address addressPerson;
                       PersonDetail personDetail;

                // create address object for this person and get the id 
                    if (collection["iPersonID"]!="0") {

                        long person_id = long.Parse(collection["iPersonID"]);
                        var q = (from p in _db.PersonDetails
                                join a in _db.Addresses
                                on p.AddressID equals a.ID
                                where p.ID == person_id
                                select new
                        { 
                            address = a,
                            person = p
                        }).FirstOrDefault();

                        addressPerson = q.address;
                        personDetail = q.person;
                        addressPerson.Address1 = collection["Address1"];
                        addressPerson.Street_1 = collection["Street_1"];
                        addressPerson.Street_2 = collection["Street_2"];
                        addressPerson.Street_3 = collection["Street_3"];
                        addressPerson.ZIP = long.Parse(collection["ZIP"]);
                        addressPerson.City = collection["City"];
                        addressPerson.Country = collection["Country"];
                        addressPerson.Disabled = false;



                          personDetail.FirstName = collection["FirstName"];
                             personDetail.LastName = collection["LastName"];
                             personDetail.MiddleName = collection["LastName"];
                             personDetail.Birthday = DateTime.Parse(collection["Birthday"]);
                             personDetail.Gender = collection["genderList"];
                             personDetail.Nationality = collection["Nationality"];
                             personDetail.JobPlace = collection["JobPlace"];
                             personDetail.MobilePhoneNumber = collection["MobilePhoneNumber"];
                             personDetail.HomePhoneNumber = collection["HomePhoneNumber"];
                             personDetail.Email = collection["Email"];
                             personDetail.FacebookID = collection["FacebookID"];
                            
                             personDetail.Disabled = false;
                             personDetail.EditContactID = user_id;
                             personDetail.EditDate = DateTime.Now;
                        
                    }
                    else
                    {

                       addressPerson = new Address()
                        {
                            Address1 = collection["Address1"],
                            Street_1 = collection["Street_1"],
                            Street_2 = collection["Street_2"],
                            Street_3 = collection["Street_3"],
                            ZIP = long.Parse(collection["ZIP"]),
                            City = collection["City"],
                            Country = collection["Country"],
                            Disabled = false

                        };

                        _db.Addresses.Add(addressPerson);


                        _db.SaveChanges();
                        ///.Entry(addressPerson).GetDatabaseValues();

                        long addressID = addressPerson.ID;

                        // create new object for person details
                       personDetail = new PersonDetail()
                        {
                            FirstName = collection["FirstName"],
                            LastName = collection["LastName"],
                            MiddleName = collection["LastName"],
                            Birthday = DateTime.Parse(collection["Birthday"]),
                            Gender = collection["genderList"],
                            Nationality = collection["Nationality"],
                            JobPlace = collection["JobPlace"],
                            MobilePhoneNumber = collection["MobilePhoneNumber"],
                            HomePhoneNumber = collection["HomePhoneNumber"],
                            Email = collection["Email"],
                            FacebookID = collection["FacebookID"],
                            AddressID = addressID,
                            Disabled = false,
                            CreateContactID = 1,
                            CreateDate = DateTime.Now


                        };
                        _db.PersonDetails.Add(personDetail);
                        _db.SaveChanges();


                        // create new user for this person
                        MD5 md5 = new MD5CryptoServiceProvider();
                        Byte[] originalBytes = ASCIIEncoding.Default.GetBytes(collection["IPassword"]);
                        Byte[] encodedBytes = md5.ComputeHash(originalBytes);

                        long role_ID = 0; // by default roleID would be "utilizator"
                        role_ID = (from r in _db.UserRoles
                                   where r.RoleName == "utilizator"
                                   select r.ID).FirstOrDefault();

                        User user = new User()
                        {
                            Username = collection["iUsername"],
                            PsswdEncrypt = BitConverter.ToString(encodedBytes),
                            CreateContactID = 1,
                            CreationDate = DateTime.Now,
                            PersonID = personDetail.ID,
                            RoleID = role_ID,
                            Disabled = false
                        };


                        _db.Users.Add(user);
                    }

                    _db.SaveChanges();

                    // create user object 

                    message = "Modificarile au fost efectuate cu succes!";
                   
                   
                }
                catch (MembershipCreateUserException e)
                {
                    ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
                    return Json("Modificarile nu au fost salvate in baza de date!");

                }
            }

            // If we got this far, something failed, redisplay form
            PersonDetail model = new PersonDetail();
            return Json("Modificarile au fost efectuate cu succes!");
        }

        //
        // POST: /Account/Disassociate

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Disassociate(string provider, string providerUserId)
        {
            string ownerAccount = OAuthWebSecurity.GetUserName(provider, providerUserId);
            ManageMessageId? message = null;

            // Only disassociate the account if the currently logged in user is the owner
            if (ownerAccount == User.Identity.Name)
            {
                // Use a transaction to prevent the user from deleting their last login credential
                using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.Serializable }))
                {
                    bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
                    if (hasLocalAccount || OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name).Count > 1)
                    {
                        OAuthWebSecurity.DeleteAccount(provider, providerUserId);
                        scope.Complete();
                        message = ManageMessageId.RemoveLoginSuccess;
                    }
                }
            }

            return RedirectToAction("Manage", new { Message = message });
        }

        //
        // GET: /Account/Manage

        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : "";
            ViewBag.HasLocalPassword = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        //
        // POST: /Account/Manage

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Manage(LocalPasswordModel model)
        {
            bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.HasLocalPassword = hasLocalAccount;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasLocalAccount)
            {
                if (ModelState.IsValid)
                {
                    // ChangePassword will throw an exception rather than return false in certain failure scenarios.
                    bool changePasswordSucceeded;
                    try
                    {
                        changePasswordSucceeded = WebSecurity.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
                    }
                    catch (Exception)
                    {
                        changePasswordSucceeded = false;
                    }

                    if (changePasswordSucceeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                    }
                    else
                    {
                        ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                    }
                }
            }
            else
            {
                // User does not have a local password so remove any validation errors caused by a missing
                // OldPassword field
                ModelState state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        WebSecurity.CreateAccount(User.Identity.Name, model.NewPassword);
                        return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                    }
                    catch (Exception e)
                    {
                        ModelState.AddModelError("", e);
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/ExternalLogin

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            return new ExternalLoginResult(provider, Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/ExternalLoginCallback

        [AllowAnonymous]
        public ActionResult ExternalLoginCallback(string returnUrl)
        {
            AuthenticationResult result = OAuthWebSecurity.VerifyAuthentication(Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
            if (!result.IsSuccessful)
            {
                return RedirectToAction("ExternalLoginFailure");
            }

            if (OAuthWebSecurity.Login(result.Provider, result.ProviderUserId, createPersistentCookie: false))
            {
                return RedirectToLocal(returnUrl);
            }

            if (User.Identity.IsAuthenticated)
            {
                // If the current user is logged in add the new account
                OAuthWebSecurity.CreateOrUpdateAccount(result.Provider, result.ProviderUserId, User.Identity.Name);
                return RedirectToLocal(returnUrl);
            }
            else
            {
                // User is new, ask for their desired membership name
                string loginData = OAuthWebSecurity.SerializeProviderUserId(result.Provider, result.ProviderUserId);
                ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(result.Provider).DisplayName;
                ViewBag.ReturnUrl = returnUrl;
                return View("ExternalLoginConfirmation", new RegisterExternalLoginModel { UserName = result.UserName, ExternalLoginData = loginData });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLoginConfirmation(RegisterExternalLoginModel model, string returnUrl)
        {
            string provider = null;
            string providerUserId = null;

            if (User.Identity.IsAuthenticated || !OAuthWebSecurity.TryDeserializeProviderUserId(model.ExternalLoginData, out provider, out providerUserId))
            {
                return RedirectToAction("Manage");
            }

            if (ModelState.IsValid)
            {
                // Insert a new user into the database
                using (UsersContext db = new UsersContext())
                {
                    UserProfile user = db.UserProfiles.FirstOrDefault(u => u.UserName.ToLower() == model.UserName.ToLower());
                    // Check if user already exists
                    if (user == null)
                    {
                        // Insert name into the profile table
                        db.UserProfiles.Add(new UserProfile { UserName = model.UserName });
                        db.SaveChanges();

                        OAuthWebSecurity.CreateOrUpdateAccount(provider, providerUserId, model.UserName);
                        OAuthWebSecurity.Login(provider, providerUserId, createPersistentCookie: false);

                        return RedirectToLocal(returnUrl);
                    }
                    else
                    {
                        ModelState.AddModelError("UserName", "User name already exists. Please enter a different user name.");
                    }
                }
            }

            ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(provider).DisplayName;
            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // GET: /Account/ExternalLoginFailure

        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [AllowAnonymous]
        [ChildActionOnly]
        public ActionResult ExternalLoginsList(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return PartialView("_ExternalLoginsListPartial", OAuthWebSecurity.RegisteredClientData);
        }

        [ChildActionOnly]
        public ActionResult RemoveExternalLogins()
        {
            ICollection<OAuthAccount> accounts = OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name);
            List<ExternalLogin> externalLogins = new List<ExternalLogin>();
            foreach (OAuthAccount account in accounts)
            {
                AuthenticationClientData clientData = OAuthWebSecurity.GetOAuthClientData(account.Provider);

                externalLogins.Add(new ExternalLogin
                {
                    Provider = account.Provider,
                    ProviderDisplayName = clientData.DisplayName,
                    ProviderUserId = account.ProviderUserId,
                });
            }

            ViewBag.ShowRemoveButton = externalLogins.Count > 1 || OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            return PartialView("_RemoveExternalLoginsPartial", externalLogins);
        }

        #region Helpers
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
        }

        internal class ExternalLoginResult : ActionResult
        {
            public ExternalLoginResult(string provider, string returnUrl)
            {
                Provider = provider;
                ReturnUrl = returnUrl;
            }

            public string Provider { get; private set; }
            public string ReturnUrl { get; private set; }

            public override void ExecuteResult(ControllerContext context)
            {
                OAuthWebSecurity.RequestAuthentication(Provider, ReturnUrl);
            }
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion
    }
}
