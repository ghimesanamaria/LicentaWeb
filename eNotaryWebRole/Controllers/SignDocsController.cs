using System;
using System.Collections.Generic;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Mvc;
using System.Runtime.InteropServices;
using System.Text;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using eNotaryWebRole.ViewModel;
using System.Security.Cryptography;
using FCLX509 = System.Security.Cryptography.X509Certificates;
using WSEX509 = Microsoft.Web.Services.Security.X509;
using System.IO;
using System.IdentityModel;
using PdfSharp;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using System.Configuration;
using Microsoft.Web.Helpers;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using eNotaryWebRole.Code;
using CloudStorageAccount = Microsoft.WindowsAzure.Storage.CloudStorageAccount;
using eNotaryWebRole.Models;

using TallComponents.PDF;
using TallComponents.PDF.Forms.Fields;
using TallComponents.PDF.Annotations.Widgets;
using TallComponents.PDF.DigitalSignatures;


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

using System.IO;



namespace eNotaryWebRole.Controllers
{
    
    public class SignDocsController : Controller
    {

        string username = "";
       

        //
        // GET: /SignDocs/
        // If the X.509 certificate is used to authenticate a client or service, Windows Communication Foundation (WCF) by default uses the Windows certificate store and Crypto API to validate the certificate and to ensure that it is trusted. Sometimes the built-in certificate validation functionality is not enough and must be changed. WCF provides an easy way to change the validation logic by allowing users to add a custom certificate validator. If a custom certificate validator is specified, WCF does not use the built-in certificate validation logic but relies on the custom validator instead.
        //public class CustomX509CertificateValidator : X509CertificateValidator
        ////{
        ////    //public override void Validate(X509Certificate2 certificate)
        ////    //{
        ////    //    var ch = new X509Chain();


        ////    //    // RevocationMode Enumeration
        ////    //    ch.ChainPolicy.RevocationMode = X509RevocationMode.Online;


        ////    //    ch.ChainPolicy.RevocationFlag=X509RevocationFlag.EntireChain;


        ////    //    //The time span that elapsed during online revocation verification or downloading the 
        ////    //    //certificate revocation list (CRL)
        ////    //    ch.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(1000);

        ////    //    //The time that the certificate was verified expressed in local time
        ////    //    ch.ChainPolicy.VerificationTime = DateTime.Now;

        ////    //    ch.Build(certificate);

        ////    //    //Check to see if the CA is a specific one
        ////    //    //if (ch.ChainElements[ch.ChainElements.Count - 1].Certificate.IssuerName.Name != "CN=Something, OU=PKI...,")
        ////    //    //{
        ////    //    //    throw new SecurityTokenValidationException("Certificate was not issued by a trusted issuer");
        ////    //    //}

        ////    //    foreach (X509ChainStatus s in ch.ChainStatus)
        ////    //    {
        ////    //        string str = s.Status.ToString();
        ////    //        Console.WriteLine("str: " + str);
        ////    //    }

        ////    //    //Check to see if the current certificate is revoked in the current system (does this not happen in the above?
        ////    //    X509Store store = new X509Store(StoreName.Disallowed, StoreLocation.LocalMachine);
        ////    //    store.Open(OpenFlags.ReadOnly);
        ////    //    bool isRevoked = store.Certificates.Contains(certificate);
        ////    //    store.Close();

        ////    //    if (isRevoked)
        ////    //    {
        ////    //        throw new SecurityTokenValidationException("Certificate is revoked");
        ////    //    }

        ////    //    //if (certificate.Verify() == false)
        ////    //    //{
        ////    //    //    throw new SecurityTokenValidationException("Certificate cannot be verified");
        ////    //    //}

        ////    //    //throw new NotImplementedException();
        ////    //}
        ////}

        private eNotaryDBEntities1 _db = new eNotaryDBEntities1();
        private IDataAccessRepository _reporsitory = new DataAccessRepository();


        public ActionResult Index()
        {

            
          

            //Load the certificate
          

            //FCLX509.X509Certificate FCLcer = WSEX509.X509Certificate.CreateFromCertFile(@"D:\Scoala\Licenta\Temp\eNotary\eNotaryWebRole\Certificate\certificatVeriSignTest.cer");

            //// Construct the WSE 1.0 X509Certificate class
            //WSEX509.X509Certificate cer = new WSEX509.X509Certificate(FCLcer.GetRawCertData());
            //string expirationDate = cer.GetExpirationDateString();
            //X509Certificate2 cert = new X509Certificate2(FCLcer);


            //// verify if the certificate is revoked using CRLs
            //string state = "valid";
            //X509Chain chain = new X509Chain();
            //chain.Build(cert);
            //chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
            //chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
            //chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(100);
            //chain.ChainPolicy.VerificationTime = DateTime.Now;
            //foreach (X509ChainElement element in chain.ChainElements)
            //{
            //    bool elementValid = element.Certificate.Verify();
            //    if (!elementValid)
            //    {
            //        // if elementValid is false this means that the certificate is revoked 
            //        // we save the status of certificate to display an error to user
            //        state = "revoked";
            //    }

            //}
            try
            {

                UnsignedDocumentsViewModel model = new UnsignedDocumentsViewModel();
                List<string> nimic = new List<string>();
                model.List = nimic;


                // signPDF();
                // signAdvancedPDF();

                var actTypeList = (from at in _db.ActTypes
                                   select new
                                   {
                                       ID = at.ID,
                                       Name = at.ActTypeName
                                   }).ToList();
                ViewBag.ActTypeList = new SelectList(actTypeList, "ID", "Name", 0);
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
                            on s.ID equals rs.SecurityPointID
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

                if (doc == 0)
                {
                    return RedirectToAction("Index", "Home");
                }
                ViewBag.ViewDocuments = doc;


                long em = (from s in _db.SecurityPoints
                           join rs in _db.RoleSecurityPoints
                           on s.ID equals rs.SecurityPointID
                           join u in _db.Users.Where(u => u.Username == username)
                           on rs.RoleID equals u.RoleID
                           where s.Name == "trimite email"
                           select rs.State).FirstOrDefault();
                long em_em = (from u in _db.Users.Where(u => u.Username == username)
                              join r in _db.RoleSecurityPoints
                              on u.ID equals r.UserID
                              join s in _db.SecurityPoints
                              on r.SecurityPointID equals s.ID
                              where s.Name == "trimite email"
                              select r.State).FirstOrDefault();
                if (em_em == 1)
                {
                    em = em_em;
                }
                ViewBag.SendMail = em;


                long sg = (from s in _db.SecurityPoints
                           join rs in _db.RoleSecurityPoints
                           on s.ID equals rs.SecurityPointID
                           join u in _db.Users.Where(u => u.Username == username)
                           on rs.RoleID equals u.RoleID
                           where s.Name == "semanare documente"
                           select rs.State).FirstOrDefault();
                long sg_sg = (from u in _db.Users.Where(u => u.Username == username)
                              join r in _db.RoleSecurityPoints
                              on u.ID equals r.UserID
                              join s in _db.SecurityPoints
                              on r.SecurityPointID equals s.ID
                              where s.Name == "semanare documente"
                              select r.State).FirstOrDefault();
                if (sg_sg == 1)
                {
                    sg = sg_sg;
                }
                ViewBag.Sign = sg;
                return View(model);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Home");
            }
            






            
        }

        [HttpPost]
        public ActionResult Index(FormCollection collection)
        {
            // verify which action was triggered sign document or save details about act
            long idAct = 0;
            string type = "";
            if (!string.IsNullOrEmpty(collection["sgSignDocument"]))
            {
                foreach (string coll in collection.AllKeys)
                {
                    if (coll.Contains("check_act"))
                    {
                         idAct = long.Parse(collection[coll].Split('_')[0]);
                        type=collection[coll].Split('_')[1];
                    }
                }

                string externalUniqueRef ="";
                if(type!="signedAct"){
                
                externalUniqueRef= _db.Acts.Where(o => o.ID == idAct).FirstOrDefault().ExternalUniqueReference;
                }
                else{
                    externalUniqueRef = _db.SignedActs.Where(o=>o.ID == idAct).FirstOrDefault().ExternalUniqueReference;
                }
                long id_Max = _db.SignedActs.Select(x => x.ID).Max();
                string ext_unique_signed = externalUniqueRef.Split('.')[0]+id_Max+".pdf";

                bool is_Signed = signAdvancedPDF(externalUniqueRef,ext_unique_signed);
                bool state = false;
                if (collection["sdSentToClient"] == "True")
                {
                    state = true;
                }

                bool signed = false;
                if (collection["sdState"] == "semnat")
                {
                    signed = true;
                }

                if (is_Signed)
                {
                    SignedAct new_act = _reporsitory.create_SignedAct(idAct, long.Parse(collection["ActTypeList"]), collection["sdActName"], collection["sdReasonState"], state, signed, collection["sdExtraDetails"], collection["sdReason"],ext_unique_signed);
                    _db.SignedActs.Add(new_act);
                    _db.SaveChanges();
                }



            }
            else
                if(!string.IsNullOrEmpty(collection["sgSave"]))
                {

                   /// must save the details
                   /// 

                    // 1. Identify the type of act 
                    foreach (string coll in collection.AllKeys)
                    {
                        if (coll.Contains("check_act"))
                        {
                            idAct = long.Parse(collection[coll].Split('_')[0]);
                            type = collection[coll].Split('_')[1];
                        }
                    }

                    // retain the details for specified act
                    if (type != "signedAct")
                    {
                       // DateTime creation_Date = DateTime.ParseExact(collection["sdCreationDate"],"yyyy-MM-dd HH:mm:ss",System.Globalization.CultureInfo.InvariantCulture);

                        _reporsitory.update_Act(idAct, long.Parse(collection["ActTypeList"]), collection["sdActName"], collection["sdReason"], collection["sdReasonState"], collection["sdState"], collection["sdExtraDetails"]);
                        

                    }
                    else
                    {
                        bool state = false;
                        if(collection["sdSentToClient"] == "True"){
                            state = true;
                        }

                        bool signed = false;
                        if(collection["sdState"] == "semnat"){
                            signed = true;
                        }
                        _reporsitory.update_SignedAct(idAct, long.Parse(collection["ActTypeList"]), collection["sdActName"], collection["sdReasonState"], state, signed, collection["sdExtraDetails"], collection["sdReason"]);
                    }
                }
                else
                    if (!string.IsNullOrEmpty(collection["sgSendToClient"]))
                    {

                       
                    }

            var actTypeList = (from at in _db.ActTypes
                               select new
                               {
                                   ID = at.ID,
                                   Name = at.ActTypeName
                               }).ToList();

            long tempActType = 0;
            if (long.TryParse(collection["ActTypeList"], out tempActType))
            {
            }
            ViewBag.ActTypeList = new SelectList(actTypeList, "ID", "Name", tempActType);
            return View();
        }
        //sign pdf
        public void signPDF()
        {

            var url = Server.MapPath("~/Fisiere/test.pdf"); ;
            using (FileStream inFile = new FileStream(url, FileMode.Open, FileAccess.Read))
            {
                //Existing document
                Document document = new Document(inFile);
                Page page = document.Pages[0];


                //Retrieve existing SignatureField
                SignatureField field = new SignatureField("SignHere");
                //foreach (Field fl in document.Fields)
                //{
                //    if( fl.FullName == "SignHere" )
                //    {
                //        field = fl as SignatureField;
                //        break;
                //    }
                //}





                //open certicate store.
                //Pkcs12Store ks = null;
                //using (StreamReader file = new StreamReader(@"D:\Scoala\Licenta\Temp\eNotary\eNotaryWebRole\Certificate\certificat12.p12"))
                //{
                //    string psswd = "jerrycora";
                //    ks = new Pkcs12Store(file.BaseStream, psswd);
                //}

                url = Server.MapPath("~/Certificate/certificat12.p12");
                var ks = new X509Certificate2(url, "jerrycora", X509KeyStorageFlags.Exportable);
                Pkcs12Store k = new Pkcs12Store(ks);

                //let the Create factory decide which type should be used.
                SignatureHandler handler = StandardSignatureHandler.Create(k);

                field.SignatureHandler = handler;

                //set some optional info.
                field.ContactInfo = "+31 (0)77 4748677";
                field.Location = "The Netherlands";
                field.Reason = "I fully agree!";

                //optional code to set image:
                //enumerate widgets 


                //create widget, which specifies the location and size.
                SignatureWidget widget = new SignatureWidget(45, page.Height - 200, 150, 40);

                //optional code to set image;
                SignatureAppearance signedAppearance = new SignatureAppearance();
                signedAppearance.Style = SignatureAppearanceStyle.ImageAndText;
                url = Server.MapPath("~/Content/logo_pdfkit.gif");
                using (System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(url))
                {
                    signedAppearance.Bitmap = bmp;
                }
                widget.SignedAppearance = signedAppearance;
                widget.BackgroundColor = System.Drawing.Color.LightPink;

                //add widget and field to document.
                field.Widgets.Add(widget);
                page.Widgets.Add(widget);
                document.Fields.Add(field);
                url = Server.MapPath("~/Fisiere/test_signed.pdf");

                // write the modified document to disk, note: signing requires read-write file access
                using (FileStream outFile = new FileStream(url, FileMode.Create, FileAccess.ReadWrite))
                {
                    document.Write(outFile);
                }
            }
        }


        //sign pdf with extended signature 
        // and add timestamp


        // variables 
        private TElPDFDocument m_CurrDoc = null;
        private string m_CurrOrigFile = "";
        private string m_CurrTempFile = "";
        private FileStream m_CurrStream = null;
        TElPDFPublicKeyRevocationInfo m_DocRevInfo = new TElPDFPublicKeyRevocationInfo();
        TElPDFPublicKeyRevocationInfo m_LocalRevInfo = new TElPDFPublicKeyRevocationInfo();
        TElMemoryCertStorage m_TrustedCerts = new TElMemoryCertStorage();
        TElMemoryCRLStorage m_KnownCRLs = new TElMemoryCRLStorage();
        TElPDFAdvancedPublicKeySecurityHandler m_Handler = null;
        TElMemoryCertStorage m_CertStorage = new TElMemoryCertStorage();
        TElHTTPTSPClient m_TspClient;
        TElHTTPSClient m_HttpClient = new TElHTTPSClient();

        // function needed to use secure blacbox

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

        // create a temporary file 
        private void PrepareTemporaryFile(string srcFile)
        {
            string TempPath = Path.GetTempFileName();
            System.IO.File.Copy(srcFile, TempPath, true);
            m_CurrStream = new FileStream(TempPath, FileMode.Open, FileAccess.ReadWrite);
            m_CurrOrigFile = srcFile;
            m_CurrTempFile = TempPath;
        }

        private void DeleteTemporaryFile(bool saveChanges)
        {
            if (m_CurrStream != null)
            {
                m_CurrStream.Close();
                m_CurrStream = null;
            }
            if (saveChanges)
            {
                System.IO.File.Copy(m_CurrTempFile, m_CurrOrigFile, true);
            }
            System.IO.File.Delete(m_CurrTempFile);
            m_CurrTempFile = "";
            m_CurrOrigFile = "";


            
        }

        private void CloseCurrentDocument(bool saveChanges)
        {
            try
            {
                if (m_CurrDoc != null)
                {
                    try
                    {
                        m_CurrDoc.Close(saveChanges);
                    }
                    finally
                    {
                        m_CurrDoc = null;
                    }
                }
            }
            catch (Exception ex)
            {
               // MessageBox.Show(ex.ToString(), "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            if (m_CurrStream != null)
            {
                DeleteTemporaryFile(saveChanges);
            }
        }

       
        private void PrepareValidation(TElPDFAdvancedPublicKeySecurityHandler handler)
        {
            m_TrustedCerts.Clear();
            //for (int i = 0; i < lvRevInfo.Items.Count; i++)
            //{
            //    if ((lvRevInfo.Items[i].Tag is TElX509Certificate) && (lvRevInfo.Items[i].Checked))
            //    {
            //        m_TrustedCerts.Add((TElX509Certificate)lvRevInfo.Items[i].Tag, false);
            //    }
            //    else if (lvRevInfo.Items[i].Tag is TElCertificateRevocationList)
            //    {
            //        m_KnownCRLs.Add((TElCertificateRevocationList)lvRevInfo.Items[i].Tag);
            //    }
            //}
            handler.OnCertValidatorPrepared += new TSBPDFCertValidatorPreparedEvent(handler_OnCertValidatorPrepared);
        }

        private void ExtractRevInfo()
        {
            if (m_CurrDoc == null)
            {
                return;
            }
            m_DocRevInfo.Clear();
            for (int i = 0; i < m_CurrDoc.SignatureCount; i++)
            {
                if (m_CurrDoc.get_Signatures(i).Handler is TElPDFAdvancedPublicKeySecurityHandler)
                {
                    TElPDFAdvancedPublicKeySecurityHandler handler = (TElPDFAdvancedPublicKeySecurityHandler)m_CurrDoc.get_Signatures(i).Handler;
                    handler.CMS.ExtractAllCertificates(m_DocRevInfo.Certificates);
                    for (int k = 0; k < handler.CMS.CRLs.Count; k++)
                    {
                        int idx = m_DocRevInfo.AddCRL();
                        m_DocRevInfo.get_CRLs(idx).Assign(handler.CMS.CRLs.get_CRLs(k));
                    }
                    m_DocRevInfo.Assign(handler.DSSRevocationInfo, false);
                    m_DocRevInfo.Assign(handler.RevocationInfo, false);
                }
            }
        }

        void handler_OnCertValidatorPrepared(object Sender, ref SBCertValidator.TElX509CertificateValidator CertValidator, TElX509Certificate Cert)
        {
            CertValidator.AddTrustedCertificates(m_TrustedCerts);
            CertValidator.AddKnownCertificates(m_LocalRevInfo.Certificates);
            CertValidator.AddKnownCRLs(m_KnownCRLs);
            for (int i = 0; i < m_LocalRevInfo.OCSPResponseCount; i++)
            {
                TElOCSPResponse resp = new TElOCSPResponse();
                try
                {
                    byte[] encodedResp = m_LocalRevInfo.get_OCSPResponses(i);
                    resp.Load(encodedResp, 0, encodedResp.Length);
                    CertValidator.AddKnownOCSPResponses(resp);
                }
                catch (Exception)
                {
                }
            }
            CertValidator.ForceCompleteChainValidationForTrusted = false;
        }

        public void function_init_document(string filename)
        {
            var url = Server.MapPath("~/Fisiere/"+filename);
            PrepareTemporaryFile(url);
                try
                {
                    m_CurrDoc = new TElPDFDocument();
                    try
                    {
                        m_CurrDoc.OwnActivatedSecurityHandlers = true;
                        m_CurrDoc.Open(m_CurrStream);
                      
                    }
                    catch (Exception)
                    {
                        m_CurrDoc = null;
                        throw;
                    }
                } catch(Exception) 
                {
                    DeleteTemporaryFile(false);
                    throw;
                }
        }


        public TElX509Certificate add_sig()
        {
             TElX509Certificate m_Cert = null;
             TElWinCertStorage m_CertStorage = null;

            m_Cert = new TElX509Certificate();
            m_CertStorage = new TElWinCertStorage();
            m_CertStorage.SystemStores.BeginUpdate();
            try
            {
                m_CertStorage.SystemStores.Add("MY");
            }
            finally
            {
                m_CertStorage.SystemStores.EndUpdate();
            }
            for (int i = 0; i < m_CertStorage.Count; i++)
            {
               // cbSystemCerts.Items.Add(m_CertStorage.get_Certificates(i).SubjectRDN.SaveToDNString());
            }



            var url = Server.MapPath("~/Certificate/certificat12.p12");
            int r = m_Cert.LoadFromFileAuto(url, "jerrycora");
                if (r != 0)
                {
                    //MessageBox.Show("Failed to load certificate, error " + r.ToString());
                    //return;
                }
            return m_Cert;

        }
           
        
       
        public bool signAdvancedPDF(string extUniqueRefAct, string signed_ext)
        {

            // get the specified document 
            // verify its extension, because the signed document will have the pdf extension
            // if the extension is different convert document in pdf, then sign

            // Step 1. Get the document wished

            CloudStorageAccount storageAccount = CloudStorageAccount.DevelopmentStorageAccount;



            // Second step
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve a reference to a container
            CloudBlobContainer container = blobClient.GetContainerReference("acte");
            CloudBlobDirectory subDirectory = container.GetDirectoryReference("actenesemnate");

            CloudBlockBlob blockBlob = subDirectory.GetBlockBlobReference(extUniqueRefAct);
            var url = Server.MapPath("~/Fisiere/");
            try
            {
                using (FileStream fileStream = new FileStream(url +extUniqueRefAct, FileMode.Create))
                {
                    blockBlob.DownloadToStream(fileStream);

                }
            }
            catch (Exception ex)
            {
            }
            

            // Step 2. Create a new PdfDocument

            if (extUniqueRefAct.Split('.')[1] != "pdf")
            {
                PdfSharp.Pdf.PdfDocument doc = new PdfSharp.Pdf.PdfDocument();
                doc.Pages.Add(new PdfSharp.Pdf.PdfPage());
                XGraphics xgr = XGraphics.FromPdfPage(doc.Pages[0]);

                //get the image            

                XImage img = XImage.FromFile(url  + extUniqueRefAct);
                xgr.DrawImage(img, 0, 0);
                //save the image in format pdf
                doc.Save(url  + extUniqueRefAct.Split('.')[0] + ".pdf");
                doc.Close();
            }
           

            m_TspClient = new TElHTTPTSPClient();
            init_function();
            function_init_document(extUniqueRefAct.Split('.')[0] + ".pdf");
            try
            {
                try
                {
                    int idx = m_CurrDoc.AddSignature();
                    TElPDFSignature sig = m_CurrDoc.get_Signatures(idx);
                    sig.Handler = m_Handler;
                    sig.Invisible = false;
                    m_CertStorage.Clear();
                    m_CertStorage.Add(add_sig(), true);
                    m_Handler.CertStorage = m_CertStorage;

                    m_Handler.PAdESSignatureType = TSBPAdESSignatureType.pastEnhanced;


                    m_Handler.CustomName = "Adobe.PPKMS"; // article: http://www.eldos.com/security/articles/2992.php
                    PrepareValidation(m_Handler);

                    m_TspClient.HTTPClient = m_HttpClient;
                    m_TspClient.URL = "http://inoa.net/ca/tsa";
                    m_TspClient.HashAlgorithm = SBConstants.Unit.SB_ALGORITHM_DGST_SHA1;
                    m_Handler.TSPClient = m_TspClient;

                    sig.WidgetProps.StretchX = 200;
                    sig.WidgetProps.StretchY = 200;




                    m_Handler.CustomRevocationInfo.Assign(m_LocalRevInfo, true);

                    m_Handler.AutoCollectRevocationInfo = true;
                    m_Handler.IgnoreChainValidationErrors = true;
                    sig.SigningTime = DateTime.UtcNow;

                    CloseCurrentDocument(true);

                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
            }
               
                // save the signed document to blob

                    FileStream file = new FileStream(url+extUniqueRefAct.Split('.')[0]+".pdf", FileMode.Open, FileAccess.ReadWrite); ;
                  
                        

                    CloudBlobDirectory subDirectory2 = container.GetDirectoryReference("actesemnate");
                    var contentType = "pdf";
                    var streamContents = file;
                
                // subdirectory name as username to identify from the blob hierarchy who uploaded the file
                    // get a unique name 
                    var blobName = signed_ext ;

                    var blob = subDirectory2.GetBlockBlobReference(blobName);
                    blob.Properties.ContentType = contentType;
                    blob.UploadFromStream(streamContents);



                    return true;
                //// save the signed act to db 

                //    SignedAct act = new SignedAct()
                //    {
                        
                //        CreatePersonID = 2,
                //        Name = "Certificat nastere semnat",
                //        CreationDate =DateTime.Now,
                //        ActID = 1,
                //        ExternalUniqueReference = extUniqueRefAct.Split('.')[0] + ".pdf",
                //        SentToClient = true,
                //        Signed = true,
                        
                        
                //    };
                //    _db.SignedActs.Add(act);
                //    _db.SaveChanges();
                   
               
            }

       
        [HttpPost]
        public ActionResult DisplayImage(string id, long parentID)
        {
            

           
            

            long idAct = long.Parse(id.Split('_')[0]);
            string type = "signed";
            if (parentID == 0)
            {
                type = id.Split('_')[1];
                if (type.Substring(0, type.Length).Contains("unsigned"))
                {
                    type = "unsigned";
                }
                

            }
            

            object model = new object();

            if (parentID == -1 || parentID == -3 || type=="unsigned")
            {
                model = (from a in _db.Acts.Where(o => o.ID == idAct)
                         select new
                         {
                             
                             a.ActTypeID,
                             a.Name,
                             a.CreationDate,
                             a.Reason,
                             a.State,
                             a.ReasonState,
                             a.ExternalUniqueReference,
                             a.ExtraDetails
                             

                         }).FirstOrDefault();
            }
            else
            {
                model = (from sa in _db.SignedActs.Where(o => o.ID == idAct)
                         join a in _db.Acts
                         on sa.ActID equals a.ID
                         select new
                         {
                             a.ActTypeID,
                             sa.Name,
                             sa.CreationDate,
                             a.Reason,
                             State = "semnat",
                             sa.ReasonSigned,
                             sa.SentToClient,
                             sa.ExtraDetails
                             

                         }).FirstOrDefault();
            }


            object personDetail = new object();
            if (parentID == -1 || parentID == -3)
            {
                personDetail = (from p in _db.PersonDetails
                                join a in _db.Acts.Where(o => o.ID == idAct)
                                on p.ID equals a.PersonDetailsID
                                select new
                                {
                                    p.FirstName,
                                    p.MiddleName,
                                    p.LastName,
                                    p.Gender,
                                    p.Birthday

                                }).FirstOrDefault();
            }
            else
                if (parentID == 0)
                {
                    personDetail =( from p in _db.PersonDetails
                                   join u in _db.Users.Where(u => u.Username == username)
                                   on p.ID equals u.PersonID
                                    select new
                                {
                                    p.FirstName,
                                    p.MiddleName,
                                    p.LastName,
                                    p.Gender,
                                    p.Birthday

                                }).FirstOrDefault();
                }
                else
            {


                personDetail = (from p in _db.PersonDetails
                                join a in _db.SignedActs.Where(o => o.ID == idAct)
                                on p.ID equals a.CreatePersonID
                                select new
                                {
                                    p.FirstName,
                                    p.MiddleName,
                                    p.LastName,
                                    p.Gender,
                                    p.Birthday

                                }).FirstOrDefault();
            }


            // get the specified document 
            // verify its extension, because the signed document will have the pdf extension
            // if the extension is different convert document in pdf, then sign

            // Step 1. Get the document wished

            CloudStorageAccount storageAccount = CloudStorageAccount.DevelopmentStorageAccount;



            // Second step
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve a reference to a container
            CloudBlobContainer container = blobClient.GetContainerReference("acte");
            CloudBlockBlob blockBlob;
            CloudBlobDirectory subDirectory;
            string extUnique = "";
            if (parentID == -1 || parentID == -3 || type == "unsigned" )
            {
                subDirectory = container.GetDirectoryReference("actenesemnate");
                extUnique = _db.Acts.Where(o => o.ID == idAct).FirstOrDefault().ExternalUniqueReference;
               blockBlob = subDirectory.GetBlockBlobReference(extUnique);
            }
            else
            {
                subDirectory = container.GetDirectoryReference("actesemnate");
                extUnique = _db.SignedActs.Where(o => o.ID == idAct).FirstOrDefault().ExternalUniqueReference;
                blockBlob = subDirectory.GetBlockBlobReference(extUnique);
            }
            
           
            
       

           
            try
            {
                var url = Server.MapPath("~/Fisiere/");
                using (var stream = System.IO.File.OpenWrite(url+extUnique))
                {
                    blockBlob.BeginDownloadToStream(stream, null, null);
                    blockBlob.DownloadToStream(stream);
                    blockBlob.EndDownloadToStream(blockBlob.BeginDownloadToStream(stream, null /* callback */, null /* state */));
                    
                   
                }
                
            }
            catch (Exception ex)
            {
            }




            return Json(new  {
             act = model, 
             person = personDetail,
             nameFile = extUnique
            
            });

        }




        [HttpPost]
        public ActionResult PersonDetails(long parentID, long id)
        {

            
             var q = parentID ==-2 ? _db.SignedActs.Where(o => o.ID == id).Select(o=>o.CreatePersonID): _db.Acts.Where(o=> o.ID == id).Select(o=> o.PersonDetailsID);
            


            var model = from a in q
                        join p in _db.PersonDetails
                        on a equals p.ID
                        join ad in _db.Addresses
                        on p.AddressID equals ad.ID
                        select new
                        {
                            p.Birthday,
                            p.CommunicationMode,
                            p.EducationLevel.EducationLevel1,
                            p.Email,
                            p.FacebookID,
                            p.FirstName,
                            p.Gender,
                            p.HomePhoneNumber,
                            p.JobPlace,
                            p.JobType.JobName,
                            p.LastName,
                            p.MiddleName,
                            p.MobilePhoneNumber,
                            p.Nationality,
                            ad.Address1,
                            ad.City,
                            ad.Country,
                            ad.Street_1,
                            ad.Street_2,
                            ad.Street_3,
                            ad.ZIP
                            
                            
                        };
            var actTypeList = (from at in _db.ActTypes
                               select new
                               {
                                   ID = at.ID,
                                   Name = at.ActTypeName
                               }).ToList();
            ViewBag.ActTypeList = new SelectList(actTypeList, "ID", "Name", 0);

            return Json(model);
        }

        [HttpPost]
        public ActionResult SendMailToUser(long id, string subject, string body, string attachment)
        {
            string message = "Emai-ul a fost transmis cu succes";

           // string username = "admin";
            




            username= User.Identity.Name;

            string toMail = _db.PersonDetails.Where(p => p.ID == id).Select(x => x.Email).FirstOrDefault();
           
            try
            {

                if (username == "admin")
                {
                    MailProvider.SendEmailToUser(subject, body, toMail, "ghimes.ana@compu-cons.ro", 0,attachment);
                }
                else
                {

                    string fromMail = (from u in _db.Users.Where(o => o.Username == username)
                                      join p in _db.PersonDetails
                                      on u.PersonID equals p.ID
                                      select p.Email).FirstOrDefault();

                    MailProvider.SendEmailToUser(subject, body, "ghimes.ana@compu-cons.ro", fromMail, 1,attachment);
                }
            }
            catch (Exception ex)
            {
                message = "Email-ul nu a fost trimis ";
            }



            return Json(message);
        }
            
        }

    }




