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


// secure BlackBox

using SBPDF;
using SBPDFCore;
using SBPAdES;
using SBX509;
using SBCustomCertStorage;
using SBCRL;
using SBCRLStorage;
using SBPDFSecurity;
using SBHTTPSClient;
using SBHTTPTSPClient;
using SBOCSPClient;
using SBWinCertStorage;

namespace eNotaryWebRole.Controllers
{
    public class HomeController : Controller
    {

        private System.ComponentModel.Container components = null;


        // variables to verify if pdfs are signed 

        private TElPDFDocument m_CurrDoc = null;
        private string m_CurrOrigFile = "";
        private string m_CurrTempFile = "";
        private FileStream m_CurrStream = null;

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
          init_function();

          if (Request.Files.Count > 0)
          {
             foreach (string  fileName in Request.Files)
             {
                 
                 // get the file
                HttpPostedFileBase file = Request.Files[fileName];


                var streamContents = file.InputStream;

                 // now we create a temporary file with the stream to verify if it is signed or not

                string TempPath = Path.GetTempFileName();
                
                m_CurrStream = new FileStream(TempPath, FileMode.Open, FileAccess.ReadWrite);
                CopyStream(streamContents, m_CurrStream);
                try
                {
                    m_CurrDoc = new TElPDFDocument();

                    try
                    {
                        m_CurrDoc.OwnActivatedSecurityHandlers = true;
                        m_CurrDoc.Open(m_CurrStream);
                       
                    }
                    catch (Exception ex)
                    {
                        m_CurrDoc = null;
                        throw;
                    }

                }
                catch (Exception ex)
                {
                }

                 // verify the signature
                for (int i = 0; i < m_CurrDoc.SignatureCount; i++)
                {
                  VerifyIfIsSigned( m_CurrDoc.get_Signatures(i));
                  
                }



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
                 
                 var blobName = file.FileName;
                 var blob = subDirectory.GetBlockBlobReference(blobName);
                 blob.Properties.ContentType = contentType;
                 blob.UploadFromStream(streamContents);

             }
          }

            

            return  RedirectToAction("Index");
        }

      TElPDFAdvancedPublicKeySecurityHandler m_Handler = null;

      public void init_function()
      {
          SBUtils.Unit.SetLicenseKey("AF479A648A42969644F109C690E12B1402F11DBD9EB213B43821FD62B787AE111989D1C38A5E2278F9D19F3D1D6AD85D87B7DA6DBAEDC72150960800413FB48E6067B17B03A5AB32A4417F35B4A17DA29FF2C9512DBC2D7AAE5CE117889C2FDC64CB65C6F6A9F1891D0CEEE134994DFF0DC19B95ABFDC55161B144E9482299618BE29FA9C8EFB89EB666049899C11907610B664CDAA723A1E18820A18A671B68C88C661854CC1B4DC48BA8806ED30AF02DAB7B25A63DE63258CE2F616F93D040DA6BC54212072542DBD41F7A343485A23C9AEF476404980F00B0125997FC7A4869186411F543FB4ED74A897E46B75351983715EEF95E6E443B25D156D010A57A");
          SBPDF.Unit.Initialize();
          SBPAdES.Unit.Initialize();
          SBPDFSecurity.Unit.Initialize();

          // The following lines are required for HTTP retrieval of CRLs and OCSP in TElX509CertificateValidator to work
          SBHTTPCRL.Unit.RegisterHTTPCRLRetrieverFactory();
          SBHTTPOCSPClient.Unit.RegisterHTTPOCSPClientFactory();
          SBHTTPCertRetriever.Unit.RegisterHTTPCertificateRetrieverFactory();

          m_Handler = new TElPDFAdvancedPublicKeySecurityHandler();
      }


      //// <summary>
      /// Copies the contents of input to output. Doesn't close either stream.
      /// </summary>
      public static void CopyStream(Stream input, Stream output)
      {
          byte[] buffer = new byte[8 * 1024];
          int len;
          while ((len = input.Read(buffer, 0, buffer.Length)) > 0)
          {
              output.Write(buffer, 0, len);
          }
      }


      void handler_OnCertValidatorPrepared(object Sender, ref SBCertValidator.TElX509CertificateValidator CertValidator, TElX509Certificate Cert)
      {
          
      }
      private void PrepareValidation(TElPDFAdvancedPublicKeySecurityHandler handler)
      {
         
          handler.OnCertValidatorPrepared += new TSBPDFCertValidatorPreparedEvent(handler_OnCertValidatorPrepared);
      }
     
      public bool VerifyIfIsSigned(TElPDFSignature sig)
      {
          string typeOfSignature = "";
          TElPDFAdvancedPublicKeySecurityHandler handler = null;
          if (sig.Handler is TElPDFAdvancedPublicKeySecurityHandler)
          {
              handler = (TElPDFAdvancedPublicKeySecurityHandler)sig.Handler;
              if (handler.PAdESSignatureType == TSBPAdESSignatureType.pastBasic)
              {
                  typeOfSignature = "Basic";
              }
              else if (handler.PAdESSignatureType == TSBPAdESSignatureType.pastEnhanced)
              {
                  typeOfSignature = "Enhanced (long-term)";
              }
              else if (handler.PAdESSignatureType == TSBPAdESSignatureType.pastDocumentTimestamp)
              {
                  typeOfSignature = "Document timestamp";
              }
              else
              {
                  typeOfSignature = "Unknown";
              }
          }
          else if (sig.Handler is TElPDFPublicKeySecurityHandler)
          {
              typeOfSignature = "Standard";
          }
          else
          {
              typeOfSignature = "Unknown";
          }

          string issuerName = "";
          string timeStampDetails = "";
          string signatureDetails = "";
          string t = "";
          if (handler != null)
            {
                issuerName = "Issuer: " + handler.CMS.get_Signatures(0).Signer.Issuer.SaveToDNString() + ", S/N: " +
                    SBUtils.Unit.BinaryToString(handler.CMS.get_Signatures(0).Signer.SerialNumber);
                
                if (handler.TimestampCount > 0)
                {
                    timeStampDetails= handler.get_Timestamps(0).Time.ToShortDateString() + " " + handler.get_Timestamps(0).Time.ToShortTimeString()+" yes";
                    
                }
                else if (handler.PAdESSignatureType == TSBPAdESSignatureType.pastDocumentTimestamp)
                {
                    timeStampDetails= handler.DocumentTimestamp.Time.ToShortDateString() + " " + handler.DocumentTimestamp.Time.ToShortTimeString() +" yes";
                    
                }
                else
                {
                    timeStampDetails= sig.SigningTime.ToShortDateString() + " " + sig.SigningTime.ToShortTimeString()+ " no";
                   
                }
               
                if ((handler.ValidationDetails == SBPKICommon.TSBCMSAdvancedSignatureValidity.casvUnknown) )
                {
                    PrepareValidation(handler);
                    try
                    {
                        sig.Validate();

                        if (!sig.IsDocumentSigned())
                            t = "Signature does not cover the entire document";
                    }
                    catch (Exception e)
                    {
                        t = e.Message;
                    }
                }
                switch (handler.ValidationDetails)
                {
                    case SBPKICommon.TSBCMSAdvancedSignatureValidity.casvValid :
                        signatureDetails = "Valid";
                        break;
                    case SBPKICommon.TSBCMSAdvancedSignatureValidity.casvSignatureCorrupted :
                        signatureDetails = "Signature corrupted";
                        break;
                    case SBPKICommon.TSBCMSAdvancedSignatureValidity.casvSignerNotFound:
                        signatureDetails = "Signer not found";
                        break;
                    case SBPKICommon.TSBCMSAdvancedSignatureValidity.casvIncompleteChain:
                        signatureDetails = "Incomplete chain";
                        break;
                    case SBPKICommon.TSBCMSAdvancedSignatureValidity.casvBadCountersignature:
                        signatureDetails = "Bad countersignature";
                        break;
                    case SBPKICommon.TSBCMSAdvancedSignatureValidity.casvBadTimestamp:
                        signatureDetails = "Bad timestamp";
                        break;
                    case SBPKICommon.TSBCMSAdvancedSignatureValidity.casvCertificateExpired:
                        signatureDetails = "Certificate expired";
                        break;
                    case SBPKICommon.TSBCMSAdvancedSignatureValidity.casvCertificateRevoked:
                        signatureDetails = "Certificate revoked";
                        break;
                    case SBPKICommon.TSBCMSAdvancedSignatureValidity.casvCertificateCorrupted:
                        signatureDetails = "Certificate corrupted";
                        break;
                    case SBPKICommon.TSBCMSAdvancedSignatureValidity.casvUntrustedCA:
                        signatureDetails = "Untrusted CA";
                        break;
                    case SBPKICommon.TSBCMSAdvancedSignatureValidity.casvRevInfoNotFound:
                        signatureDetails = "Revocation information not found";
                        break;
                    case SBPKICommon.TSBCMSAdvancedSignatureValidity.casvTimestampInfoNotFound:
                        signatureDetails = "Timestamp information not found";
                        break;
                    case SBPKICommon.TSBCMSAdvancedSignatureValidity.casvFailure:
                        signatureDetails = "General failure";
                        break;
                    case SBPKICommon.TSBCMSAdvancedSignatureValidity.casvCertificateMalformed:
                        signatureDetails = "Certificate malformed";
                        break;
                    case SBPKICommon.TSBCMSAdvancedSignatureValidity.casvUnknown:
                        signatureDetails = "Unknown";
                        break;
                    case SBPKICommon.TSBCMSAdvancedSignatureValidity.casvChainValidationFailed:
                        signatureDetails = "Chain validation failed";
                        break;
                    default:
                        signatureDetails = "";
                        break;
                }

              
            }
            else
            {
                //item.SubItems.Add(sig.AuthorName);
                //item.SubItems.Add(sig.SigningTime.ToShortDateString() + " " + sig.SigningTime.ToShortTimeString());
                //item.SubItems.Add("N/A");
                //item.SubItems.Add("N/A");
            }
           


          return true;
      }
    }
}
