using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using System.Configuration;
using Microsoft.Web.Helpers;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Configuration;
using CloudStorageAccount = Microsoft.WindowsAzure.Storage.CloudStorageAccount;
using eNotaryWebRole.Models;
using System.Security;
using System.Security.Cryptography;
using System.Globalization;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;

using SBConstants;
using SBPKCS11Base;
using SBPKCS11Common;
using SBPKCS11CertStorage;
using SBX509;
using SBUtils;

namespace eNotaryWebRole.Controllers
{
    public class HomeController : Controller
    {

        private System.ComponentModel.Container components = null;

        private CardBase m_iCard = null;
       // private APDUPlayer m_apduPlayer = null;
        private APDUParam m_apduParam = null;
        const string DefaultReader = "Gemplus USB Key Smart Card Reader 0";
        private APDUPlayer m_apduPlayer = null;
       
        const string ApduListFile = "ApduList.xml";

        private void SelectICard()
        {
            try
            {
                if (m_iCard != null)
                    m_iCard.Disconnect(DISCONNECT.Unpower);

                m_iCard = new CardNative();
                

                m_iCard.OnCardInserted += new CardInsertedEventHandler(m_iCard_OnCardInserted);
                m_iCard.OnCardRemoved += new CardRemovedEventHandler(m_iCard_OnCardRemoved);

            }
            catch (Exception ex)
            {
               
            }
        }

        /// <summary>
        /// CardRemovedEventHandler
        /// </summary>
        private void m_iCard_OnCardRemoved(string reader)
        {
          
        }


        // CardInsertedEventHandler
        /// </summary>
        private void m_iCard_OnCardInserted(string reader)
        {
           
        }

        private void SetupReaderList()
        {
            try
            {
                string[] sListReaders = m_iCard.ListReaders();
                

                if (sListReaders != null)
                {
                  
                }
            }
            catch (Exception ex)
            {
               
            }
        }
        /// <summary>
        /// Loads the APDU list
        /// </summary>
        private void LoadApduList()
        {
            try
            {
                // Create the APDU player
              //  m_apduPlayer = new APDUPlayer(ApduListFile, m_iCard);

             
            }
            catch (Exception ex)
            {
                
            }
        }
        private APDUParam BuildParam()
        {
            byte bP1 = byte.Parse("20", NumberStyles.AllowHexSpecifier);
            byte bP2 = byte.Parse("30", NumberStyles.AllowHexSpecifier);
            byte bLe = byte.Parse("40");

            APDUParam apduParam = new APDUParam();
            apduParam.P1 = bP1;
            apduParam.P2 = bP2;
            apduParam.Le = bLe;

            // Update Current param
            m_apduParam = apduParam.Clone();

            return apduParam;
        }

        

        private eNotaryDBEFEntities _db = new eNotaryDBEFEntities();
        public ActionResult Index()
        {
            
         
           
            //SelectICard();
            //SetupReaderList();
            //LoadApduList();
            //try
            //{
            //    m_iCard.Connect(DefaultReader, SHARE.Shared, PROTOCOL.T0orT1);
            //    try
            //    {
            //        // Get the ATR of the card
            //        byte[] atrValue = m_iCard.GetAttribute(SCARD_ATTR_VALUE.ATR_STRING);              
                           
            //    }
            //    catch (Exception ex)
            //    {                   
            //    }          

            //}
            //catch (Exception ex)
            //{            
            //}
          
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";

            // to test if your service is running in the compute emulator or in a classic webserver
            // environment, you can check the value of the IsAvailable property
            // true measns the service is running a WA fabric, either local or live

            if(RoleEnvironment.IsAvailable)
            ViewBag.EmailAdmin = RoleEnvironment.GetConfigurationSettingValue("EmailAdmin");
            else
            {
                ViewBag.EmailAdmin = ConfigurationManager.AppSettings["EnailAdmin"];
            }

            LocalResource resouce = RoleEnvironment.GetLocalResource("eNotarySpace");
            ViewBag.LocalStorage =resouce.RootPath ;

          

            return View();
        }

        [HttpPost]
        public ActionResult Index(IEnumerable<HttpPostedFileBase> files)
        {
            /*foreach (var file in files)
            {
                var filename = Path.Combine(Server.MapPath("~/App_Data"), file.FileName);
                file.SaveAs(filename);
            }*/

            var temp = Request.Files.Count;
            return Json(temp);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

      [HttpPost]
        public ActionResult Upload()
      {
          var tmp = Request;

          if (Request.Files.Count > 0)
          {
             foreach (string  fileName in Request.Files)
             {

                 // get the file
                HttpPostedFileBase file = Request.Files[fileName];

                 // create a container

                 // First step
                 // If your creating an application with no reference to Microsoft.WindowsAzure.CloudConfigurationManager
                 // and your connection string is located in the web.config or app.config , the you can use ConfigurationManger to retrieve the connection string.
                 // 
                 CloudStorageAccount storageAccount = CloudStorageAccount.DevelopmentStorageAccount;



                 // Second step
                 CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

                 // Retrieve a reference to a container
               CloudBlobContainer container = blobClient.GetContainerReference("acte");


                 // Create the container if it doesn't already exist.
                 container.CreateIfNotExists();
                 // By default, the new container is private and you must specify your storage access key to download the blobs from this container.


                 CloudBlobDirectory subDirectory = container.GetDirectoryReference("actenesemnate");
                                

                 // upload a blob into a container
                 // Create or overwrite the "testContainer" blob with contents from an uploaded fike
                 var contentType = file.ContentType;
                 var streamContents = file.InputStream;
                 var blobName = file.FileName;
                 var blob = subDirectory.GetBlockBlobReference(blobName);
                 blob.Properties.ContentType = contentType;
                 blob.UploadFromStream(streamContents);

             }
          }

            

            return  RedirectToAction("Index");
        }
    }
}
