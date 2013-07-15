using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

//Windows Azure
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using System.Configuration;
using Microsoft.Web.Helpers;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Configuration;
using CloudStorageAccount = Microsoft.WindowsAzure.Storage.CloudStorageAccount;

// Models and ViewModels
using eNotaryWebRole.Models;
using eNotaryWebRole.ViewModel;

// Security
using System.Security;
using System.Security.Cryptography;
using System.Globalization;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;

// secure BlackBox

using SBConstants;
using SBPKCS11Base;
using SBPKCS11Common;
using SBPKCS11CertStorage;
using SBX509;
using SBUtils;
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

using System.Net.Http.Headers;


using System.Web.Script.Serialization;

namespace eNotaryWebRole.Controllers
{
    using eNotaryWebRole.Code;
    using eNotaryWebRole.ViewModel;

    
 
   
    public class HomeController : Controller
    {

        private System.ComponentModel.Container components = null;
        string username = "";
        private IPDFProvider _rep_pdf = new PDFProvider();
        private IDataAccessRepository _repository = new DataAccessRepository();


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

        

        private eNotaryDBEntities1 _db = new eNotaryDBEntities1();

        [Authorize]
       [RequireHttps]
        
        public ActionResult Index()
        {

            var cert = HttpContext.Request.ClientCertificate;
           
           

           var act_type_list = from at in _db.ActTypes
                                    select new
                                    {
                                        ID= at.ID,
                                        Name = at.ActTypeName
                                    };


                     ViewBag.ActType = new SelectList(act_type_list, "ID", "Name", 0);
                     // verify security points 

                     username = User.Identity.Name;
                     long us = (from s in _db.SecurityPoints
                                join rs in _db.RoleSecurityPoints
                                on s.ID equals rs.SecurityPointID
                                join u in _db.Users.Where(u => u.Username == username)
                                on rs.RoleID equals u.RoleID
                                where s.Name == "vizualizare utilizatori"
                                select rs.State).FirstOrDefault();
                     // verify if per user is set this security point
                     long us_us = (from u in _db.Users.Where(u => u.Username == username)
                                   join r in _db.RoleSecurityPoints
                                   on u.ID equals r.UserID
                                   join s in _db.SecurityPoints
                                   on r.SecurityPointID equals s.ID
                                   where s.Name == "vizualizare utilizatori"
                                   select r.State).FirstOrDefault();

                     if (us_us == 1)
                     {
                         us = us_us;
                     }
                     ViewBag.ViewUsers = us;

                     long doc = (from s in _db.SecurityPoints
                                 join rs in _db.RoleSecurityPoints
                                 on s.ID equals rs.RoleID
                                 join u in _db.Users.Where(u => u.Username == username)
                                 on rs.RoleID equals u.RoleID
                                 where s.Name == "vizualizare documente"
                                 select rs.State).FirstOrDefault();
                     long doc_doc = (from u in _db.Users.Where(u => u.Username == username)
                                     join r in _db.RoleSecurityPoints
                                     on u.ID equals r.UserID
                                     join s in _db.SecurityPoints
                                     on r.SecurityPointID equals s.ID
                                     where s.Name == "vizualizare documente"
                                     select r.State).FirstOrDefault();
                     if (doc_doc == 1)
                     {
                         doc = doc_doc;
                     }
                     ViewBag.ViewDocuments = doc;
                         


           

            //if (Request.IsAuthenticated)
            //{
                return View();
            //}

            
            //return RedirectToAction("Login", "Account");
        }

        public ActionResult UploadAndDetails()
        {
            return PartialView("UploadAndDetails");
        }

        public ActionResult DivorceApplication()
        {
            DivorceViewModel dv = new DivorceViewModel();
            dv.husband = new DivorcePersonDetailViewModel();
            dv.wife = new DivorcePersonDetailViewModel();
            dv.common = new DivorceCommonDetailsViewModel();
            return PartialView("DivorceApplication",dv);
        }
        [HttpPost]
        public ActionResult DivorceApplication(FormCollection collection)
        {

            Dictionary<string, string> formatPDF = new Dictionary<string, string>();
            foreach (string q in collection.AllKeys)
            {
                formatPDF.Add("["+q+"]", collection[q]);
            }

            // create divorce view model object
            DivorceViewModel dv = new DivorceViewModel();
            dv.husband = _repository.create_person(collection["daNameHusband/"], collection["daNameFatherHusband/"], collection["daNameMotherHusband/"], collection["daCityHusband"], collection["daCountyHusband"], collection["daSerieActHusband"], collection["daNoActHusband"], collection["daCNPHusband/"], collection["daAddressHusband"], DateTime.Parse(collection["daBirthdayHusband"]));
            dv.wife = _repository.create_person(collection["daNameWife/"], collection["daNameFatherWife/"], collection["daNameMotherWife/"], collection["daCityHusband"], collection["daCountyWife"], collection["daSerieActWife"], collection["daNoActWife"], collection["daCNPWife/"], collection["daAddressWife"], DateTime.Parse(collection["daBirthdayWife"]));
            dv.common = new DivorceCommonDetailsViewModel()
            {
                downhall_city = collection["daDownHallCity"],
                marriage_date = DateTime.Parse(collection["daMarriageDate"]),
                downhall_county = collection["daDownHallCounty"],
                marriage_certificate_serie = collection["daMarriageCertificateSeries"],
                marriage_certificate_no = collection["daMarriageCertificateNo"],
                common_city = collection["daCommonCity"],
                common_street = collection["daCommonStreet"],
                common_street_no = collection["daCommonStreetNo"],
                common_street_bl = collection["daCommonStreetBl"],
                common_et = collection["daCommonEt"],
                common_ap = collection["daCommonAp"],
                common_ex_husband_name = collection["daCommonExHusbandName"],
                common_ex_wife_name=collection["daCommonExWifeName"]
            };
            string url = Server.MapPath("~/PDFApplications/");
            string url_config = Server.MapPath("~/ConfigFiles/");
            string filename = "out.pdf";
            _rep_pdf.create_divorce_pdf(filename, url, formatPDF,url_config);
            return View(dv);
        }


        [HttpGet]
        public JsonResult GetActType()
        {
            var act_type = from at in _db.ActTypes
                           select new
                           {
                               id=at.ID,
                               name = at.ActTypeName
                           };
            return Json(act_type, JsonRequestBehavior.AllowGet);
        }


         [HttpPost]
        public ActionResult SaveDataFile(string ID, string Name, string ActTypeID, string ExtraDetails, string Reason)
        {

            string message = "Datele au fost salvate cu succes pentru fisierul "+Name+"!";
             long id=long.Parse(ID);


            try
            {
                Act uploaded_Act = _db.Acts.Where(x => x.ID == id).FirstOrDefault();
                uploaded_Act.Name = Name;
                uploaded_Act.ActTypeID = long.Parse(ActTypeID);
                uploaded_Act.ExtraDetails = ExtraDetails;
                uploaded_Act.Reason = Reason;
                uploaded_Act.Disabled = false;
                _db.SaveChanges();



            }
            catch (Exception ex)
            {
                message = "In timpul salvarii datelor au avut loc niste probleme. Va rugam repetati completarea datelor si salvati din nou!";
            }

            return Json(message);
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
          username = User.Identity.Name;
          var tmp = Request;
          string messages = "Fiserele au fost  incarcat cu succes!"; 
          InitBlackBox();
          Dictionary<string, List<SignatureDetailsViewModel> > docsSigs = new Dictionary<string, List<SignatureDetailsViewModel>>();
          Dictionary<string,string> png_preview_List = new Dictionary<string,string>();
       
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
                    messages = messages + "In timpul verificarii semnaturii au intervenit erori . Va rugam reluati actiunea de upload!";
                }

                 // verify the signatures
                List<SignatureDetailsViewModel> list_Signatures = new List<SignatureDetailsViewModel>();
                for (int i = 0; i < m_CurrDoc.SignatureCount; i++)
                {
                  list_Signatures.Add( VerifyIfIsSigned( m_CurrDoc.get_Signatures(i)));
                  
                }

                docsSigs.Add(file.FileName, list_Signatures);
                if (list_Signatures.Count > 0)
                {

                    try
                    {

                       
                        // create a container

                        // First step
                        // If your creating an application with no reference to Microsoft.WindowsAzure.CloudConfigurationManager
                        // and your connection string is located in the web.config or app.config , the you can use ConfigurationManger to retrieve the connection string.
                        // 

                        // local storage
                        //CloudStorageAccount storageAccount = CloudStorageAccount.DevelopmentStorageAccount;


                        CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=portalvhdsb0gp4f2vb4fdk;AccountKey=H7CNOHAFQBVZ5KLRqGUFLYblBJAfSrdBcmyMXbj3tP/YE0HR1oS2PYWbELqQN6wnBuWEJ1nUV39SsarZHfbVcw==");
                        

                       

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

                        // create unique name for file adding the last record id from table
                        long rand = 0;
                        if( _db.Acts.Count()>0){
                        rand = (from a in _db.Acts
                               select a.ID).Max();
                        }
                        else
                        {
                            rand = 0;
                        }

                        var contentType = file.ContentType;
                        var blobName = file.FileName.Split('.')[0] + "_" + rand + "." + file.FileName.Split('.')[1];
                        var blob = subDirectory.GetBlockBlobReference(blobName);
                        blob.Properties.ContentType = contentType;
                        streamContents.Position = 0;
                        blob.UploadFromStream(streamContents);

                        User user = _db.Users.Where(x => x.Username == username).FirstOrDefault();

                        string sig_users = "";
                        foreach( var sig in list_Signatures)
                        {
                            sig_users = sig_users + sig.issuerName + ";";

                        }
                        //save the file in database

                        Act new_act = new Act()
                        {
                            PersonDetailsID = user.PersonID,
                            ExternalUniqueReference = blobName,
                            CreationDate = DateTime.Now,
                            Signed = false,
                            State="nevizualizat",
                            Reason="",//must be edited after the upload is reloaded
                            Disabled = true,
                            ActTypeID = 1,// this will be changed
                            ExtraDetails = sig_users
                        };
                        _db.Acts.Add(new_act);
                        _db.SaveChanges();

                        var url = Server.MapPath("~/Content/pdf_preview/"); 
                        //create png image from first page to use it for preview
                        PdfFunctions pdf_to_bitmap = new PdfFunctions();


                        ///must find an unique name
                        ///


                        pdf_to_bitmap.create_bitmap(streamContents, file.FileName.Split('.')[0]+"_" + rand + ".png", url);

                        png_preview_List.Add(new_act.ID.ToString(),file.FileName.Split('.')[0]+"_" + rand + ".png");
                      

                    }
                    catch (Exception ex)
                    {
                        messages = messages+ "In timpul incarcarii fisierului " + file.FileName + " pe server a intervenit o problema va rugam reluati actiunea de upload" +" Exceptia este: "+ ex.ToString();
                    }
                   
                }
             }
             JavaScriptSerializer jsonserializer = new JavaScriptSerializer();

              string png_list= jsonserializer.Serialize(png_preview_List);
             
             return Json(
                 new
                 {
                     list = docsSigs,
                     png_list,
                     messages = messages
                 }
                 );
          }

            

            return  RedirectToAction("Index");
        }

      TElPDFAdvancedPublicKeySecurityHandler m_Handler = null;

      public void InitBlackBox()
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
     
      public SignatureDetailsViewModel VerifyIfIsSigned(TElPDFSignature sig)
      {

          SignatureDetailsViewModel model = new SignatureDetailsViewModel();
          model.typeOfSignature = "";
          TElPDFAdvancedPublicKeySecurityHandler handler = null;
          if (sig.Handler is TElPDFAdvancedPublicKeySecurityHandler)
          {
              handler = (TElPDFAdvancedPublicKeySecurityHandler)sig.Handler;
              if (handler.PAdESSignatureType == TSBPAdESSignatureType.pastBasic)
              {
                  model.typeOfSignature = "Basic";
              }
              else if (handler.PAdESSignatureType == TSBPAdESSignatureType.pastEnhanced)
              {
                  model.typeOfSignature = "Enhanced (long-term)";
              }
              else if (handler.PAdESSignatureType == TSBPAdESSignatureType.pastDocumentTimestamp)
              {
                  model.typeOfSignature = "Document timestamp";
              }
              else
              {
                  model.typeOfSignature = "Unknown";
              }
          }
          else if (sig.Handler is TElPDFPublicKeySecurityHandler)
          {
              model.typeOfSignature = "Standard";
          }
          else
          {
              model.typeOfSignature = "Unknown";
          }

          model.issuerName = "";
         model.timeStampDetails = "";
          model.signatureDetails = "";
          model.errorDoc = "";
          if (handler != null)
            {
                model.issuerName = "Issuer: " + handler.CMS.get_Signatures(0).Signer.Issuer.SaveToDNString() + ", S/N: " +
                    SBUtils.Unit.BinaryToString(handler.CMS.get_Signatures(0).Signer.SerialNumber);
                
                if (handler.TimestampCount > 0)
                {
                    model.timeStampDetails = handler.get_Timestamps(0).Time.ToShortDateString() + " " + handler.get_Timestamps(0).Time.ToShortTimeString() + " yes";
                    
                }
                else if (handler.PAdESSignatureType == TSBPAdESSignatureType.pastDocumentTimestamp)
                {
                    model.timeStampDetails = handler.DocumentTimestamp.Time.ToShortDateString() + " " + handler.DocumentTimestamp.Time.ToShortTimeString() + " yes";
                    
                }
                else
                {
                    model.timeStampDetails = sig.SigningTime.ToShortDateString() + " " + sig.SigningTime.ToShortTimeString() + " no";
                   
                }
               
                if ((handler.ValidationDetails == SBPKICommon.TSBCMSAdvancedSignatureValidity.casvUnknown) )
                {
                    PrepareValidation(handler);
                    try
                    {
                        sig.Validate();

                        if (!sig.IsDocumentSigned())
                            model.errorDoc = "Signature does not cover the entire document";
                    }
                    catch (Exception e)
                    {
                        model.errorDoc = e.Message;
                    }
                }
                switch (handler.ValidationDetails)
                {
                    case SBPKICommon.TSBCMSAdvancedSignatureValidity.casvValid :
                        model.signatureDetails = "Valid";
                        break;
                    case SBPKICommon.TSBCMSAdvancedSignatureValidity.casvSignatureCorrupted :
                        model.signatureDetails = "Signature corrupted";
                        break;
                    case SBPKICommon.TSBCMSAdvancedSignatureValidity.casvSignerNotFound:
                        model.signatureDetails = "Signer not found";
                        break;
                    case SBPKICommon.TSBCMSAdvancedSignatureValidity.casvIncompleteChain:
                        model.signatureDetails = "Incomplete chain";
                        break;
                    case SBPKICommon.TSBCMSAdvancedSignatureValidity.casvBadCountersignature:
                        model.signatureDetails = "Bad countersignature";
                        break;
                    case SBPKICommon.TSBCMSAdvancedSignatureValidity.casvBadTimestamp:
                        model.signatureDetails = "Bad timestamp";
                        break;
                    case SBPKICommon.TSBCMSAdvancedSignatureValidity.casvCertificateExpired:
                        model.signatureDetails = "Certificate expired";
                        break;
                    case SBPKICommon.TSBCMSAdvancedSignatureValidity.casvCertificateRevoked:
                        model.signatureDetails = "Certificate revoked";
                        break;
                    case SBPKICommon.TSBCMSAdvancedSignatureValidity.casvCertificateCorrupted:
                        model.signatureDetails = "Certificate corrupted";
                        break;
                    case SBPKICommon.TSBCMSAdvancedSignatureValidity.casvUntrustedCA:
                        model.signatureDetails = "Untrusted CA";
                        break;
                    case SBPKICommon.TSBCMSAdvancedSignatureValidity.casvRevInfoNotFound:
                        model.signatureDetails = "Revocation information not found";
                        break;
                    case SBPKICommon.TSBCMSAdvancedSignatureValidity.casvTimestampInfoNotFound:
                        model.signatureDetails = "Timestamp information not found";
                        break;
                    case SBPKICommon.TSBCMSAdvancedSignatureValidity.casvFailure:
                        model.signatureDetails = "General failure";
                        break;
                    case SBPKICommon.TSBCMSAdvancedSignatureValidity.casvCertificateMalformed:
                        model.signatureDetails = "Certificate malformed";
                        break;
                    case SBPKICommon.TSBCMSAdvancedSignatureValidity.casvUnknown:
                        model.signatureDetails = "Unknown";
                        break;
                    case SBPKICommon.TSBCMSAdvancedSignatureValidity.casvChainValidationFailed:
                        model.signatureDetails = "Chain validation failed";
                        break;
                    default:
                        model.signatureDetails = "";
                        break;
                }

              
            }
            else
            {
                model.issuerName = sig.AuthorName;
                model.signatureDetails = "Semnat la data de :"+sig.SigningTime.ToShortDateString() + " " + sig.SigningTime.ToShortTimeString();
                model.timeStampDetails = "N/A";
                model.typeOfSignature = "N/A";
                model.errorDoc = "N/A";
                model.reason = sig.Reason;
            }
           


          return model;
      }
       

     
    }
}
