using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Cryptography.X509Certificates;
namespace eNotaryWebRole.Code
{
    public class AuthorizationFilter: IAuthorizationFilter
    {
        //private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(typeof(AuthorizationFilter));

        public void OnAuthorization(AuthorizationContext filterContext)
        {
           // if (_logger.IsInfoEnabled) _logger.Info("OnAuthorization");

            HttpContext context = HttpContext.Current;

            if (context.Request.ClientCertificate.IsPresent)
            {
                X509Certificate2 userCert = new X509Certificate2(context.Request.ClientCertificate.Certificate);
                X500DistinguishedName dn = userCert.SubjectName;
                string userDn = dn.Name.Trim().ToUpper();

              //  if (_logger.IsInfoEnabled) _logger.Info(string.Format("OnAuthorization : UserDn = {0}", userDn));

                if (userDn != "CN=JOHN DOE")
                {
                    throw new HttpException(401, "Invalid Client Certificate");
                }
            }
            else
            {
                throw new HttpException(401, "Client Certificate Missing");
            }
        }
    }
}