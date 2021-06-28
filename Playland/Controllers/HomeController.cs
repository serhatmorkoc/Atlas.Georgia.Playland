using Atlantis.Core.Crypto;
using Atlantis.Core.Web;
using Atlantis.Data.Common.Base;
using Newtonsoft.Json;
using Playland.Database;
using Playland.Database.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Playland.Controllers
{
    public class HomeController : Controller
    {

        public DashboardFactory factory = new DashboardFactory();

        public string CookieId
        {
            get
            {
                return EncryptionHelper.Encrypt("PlaylandUserCookie2018");
            }
        }

        public string UsernameCookieId
        {
            get
            {
                return EncryptionHelper.Encrypt("PlaylandUserCookie2017");
            }
        }

        public User SessionUser
        {
            get
            {
                string cookieKey = CookieHelper.GetCookie(CookieId);
                string decryptedResult = EncryptionHelper.Decrypt(cookieKey);
                return JsonConvert.DeserializeObject<User>(decryptedResult);
            }
        }


        public ActionResult Dashboard()
        {
            if (SessionUser == null)
            {
                return RedirectToAction("Login", "Home");
            }
            ViewBag.SessionUser = SessionUser;
            return View();
        }


        [HttpGet]
        [AcceptVerbs(HttpVerbs.Get)]
        [ActionName("Login")]
        public ActionResult Login()
        {
            if (SessionUser != null)
            {
                return RedirectToAction("Dashboard", "Home");
            }

            User user = new User();

            string usernameCookie = CookieHelper.GetCookie(UsernameCookieId);
            if (!string.IsNullOrEmpty(usernameCookie))
            {
                user.Username = usernameCookie;
            }
            return View(user);
        }

        [HttpGet]
        [AcceptVerbs(HttpVerbs.Get)]
        [ActionName("Logout")]
        public ActionResult Logout()
        {
            CookieHelper.CreateCookie(CookieId, string.Empty);
            return RedirectToAction("Login", "Home");
        }


        [HttpPost]
        public JsonResult PostLogin(User user)
        {
            Func<GenericResponse<User>> action = () =>
            {
                return factory.Authenticate(user);
            };
            GenericResponse<User> genericResponse = ExecuteAction<User>(action);

            if (genericResponse.IsSucceed)
            {
                user = genericResponse.Result;
                string serializedUser = JsonConvert.SerializeObject(user);
                string encryptedResult = EncryptionHelper.Encrypt(serializedUser);
                CookieHelper.CreateCookie(CookieId, encryptedResult, DateTime.Now.AddDays(1));
                CookieHelper.CreateCookie(UsernameCookieId, user.Username);
            }

            return Json(genericResponse);
        }


        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        [ActionName("GetSummaries")]
        public JsonResult GetSummaries(string startDate, string endDate, int typeId)
        {

            Func<GenericResponse<Summary>> action = () =>
            {
                return factory.GetSummaries(startDate, endDate, typeId);
            };
            GenericResponse<Summary> genericResponse = ExecuteAction<Summary>(action);
            return Json(genericResponse);
        }


        public GenericResponse<T> ExecuteAction<T>(Func<GenericResponse<T>> actionExecutor)
        {
            GenericResponse<T> response = new GenericResponse<T>();

            try
            {
                response = actionExecutor.Invoke();
            }
            catch (Exception ex)
            {
                response.IsSucceed = false;
                response.Error = new Error
                {
                    ErrorMessage = ex.Message,
                };
            }

            return response;
        }

        [ActionName("_UploadDetail")]
        public ActionResult GetUploadDetail(string startDate, string endDate, int typeId)
        {
            Func<GenericResponse<List<CardUpload>>> action = () =>
            {
                return factory.GetCardUploads(startDate, endDate, typeId);
            };

            GenericResponse<List<CardUpload>> genericResponse = ExecuteAction<List<CardUpload>>(action);
            List<CardUpload> cardUploads = genericResponse.Result;
            return PartialView(cardUploads);
        }
    }
}
