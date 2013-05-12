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



namespace eNotaryWebRole.Controllers
{
    public class SignDocsController : Controller
    {
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

        private eNotaryDBEFEntities _db = new eNotaryDBEFEntities();


        public ActionResult Index()
        {

            


            //Load the certificate

            FCLX509.X509Certificate FCLcer = WSEX509.X509Certificate.CreateFromCertFile(@"D:\Scoala\Licenta\Temp\eNotary\eNotaryWebRole\Certificate\certificatVeriSignTest.cer");

            // Construct the WSE 1.0 X509Certificate class
            WSEX509.X509Certificate cer = new WSEX509.X509Certificate(FCLcer.GetRawCertData());
            string expirationDate = cer.GetExpirationDateString();
            X509Certificate2 cert = new X509Certificate2(FCLcer);


            // verify if the certificate is revoked using CRLs
            string state = "valid";
            X509Chain chain = new X509Chain();
            chain.Build(cert);
            chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
            chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
            chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(100);
            chain.ChainPolicy.VerificationTime = DateTime.Now;
            foreach (X509ChainElement element in chain.ChainElements)
            {
                bool elementValid = element.Certificate.Verify();
                if (!elementValid)
                {
                    // if elementValid is false this means that the certificate is revoked 
                    // we save the status of certificate to display an error to user
                    state = "revoked";
                }

            }


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
            ViewBag.ActTypeList = new SelectList(actTypeList,"ID","Name",0);

            return View(model);
        }

        [HttpPost]
        public ActionResult Index(FormCollection collection)
        {
            // verify which action was triggered sign document or save details about act
            long idAct = 0;
            if (!string.IsNullOrEmpty(collection["sgSignDocument"]))
            {
                foreach (string coll in collection.AllKeys)
                {
                    if (coll.Contains("check_act"))
                    {
                         idAct = long.Parse(collection[coll]);
                    }
                }

                string externalUniqueRef = _db.Acts.Where(o => o.ID == idAct).FirstOrDefault().ExternalUniqueReference;

                signAdvancedPDF(externalUniqueRef);



            }
            else
                if(!string.IsNullOrEmpty(collection["sgSave"]))
                {
                   
                }
                else
                    if (!string.IsNullOrEmpty(collection["sgSendToClient"]))
                    {

                        MailProvider.SendEmailToUser("", "", "", "ghimes.ana@gmail.com");
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

            var url = HttpContext.Request.PhysicalApplicationPath;
            using (FileStream inFile = new FileStream(url+"\\Fisiere\\test.pdf", FileMode.Open, FileAccess.Read))
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
                var ks = new X509Certificate2(url+"\\Certificate\\certificat12.p12", "jerrycora", X509KeyStorageFlags.Exportable);
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
                using (System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(url+"\\Content\\logo_pdfkit.gif"))
                {
                    signedAppearance.Bitmap = bmp;
                }
                widget.SignedAppearance = signedAppearance;
                widget.BackgroundColor = System.Drawing.Color.LightPink;

                //add widget and field to document.
                field.Widgets.Add(widget);
                page.Widgets.Add(widget);
                document.Fields.Add(field);

                // write the modified document to disk, note: signing requires read-write file access
                using (FileStream outFile = new FileStream(url+"\\Fisiere\\test_signed.pdf", FileMode.Create, FileAccess.ReadWrite))
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

        public void function_init_document()
        {
            var url = HttpContext.Request.PhysicalApplicationPath;
            PrepareTemporaryFile(url + "\\Fisiere\\test.pdf");
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


            int r = m_Cert.LoadFromFileAuto(@"D:\Scoala\Licenta\Temp\eNotary\eNotaryWebRole\Certificate\certificat12.p12", "jerrycora");
                if (r != 0)
                {
                    //MessageBox.Show("Failed to load certificate, error " + r.ToString());
                    //return;
                }
            return m_Cert;

        }
           
        
       
        public void signAdvancedPDF(string extUniqueRefAct)
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
            var url = HttpContext.Request.PhysicalApplicationPath;
            try
            {
                using (FileStream fileStream = new FileStream(url + "\\Fisiere\\test.jpg", FileMode.Create))
                {
                    blockBlob.DownloadToStream(fileStream);

                }
            }
            catch (Exception ex)
            {
            }
            

            // Step 2. Create a new PdfDocument

            PdfSharp.Pdf.PdfDocument doc = new PdfSharp.Pdf.PdfDocument();
            doc.Pages.Add(new PdfSharp.Pdf.PdfPage());
            XGraphics xgr = XGraphics.FromPdfPage(doc.Pages[0]);

            //get the image            

            XImage img = XImage.FromFile(url + "\\Fisiere\\test.jpg");
            xgr.DrawImage(img, 0, 0);
            //save the image in format pdf
            doc.Save(url + "\\Fisiere\\test.pdf");
            doc.Close();


            m_TspClient = new TElHTTPTSPClient();
            init_function();
            function_init_document();
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
                      
                     
                        
                       
                            m_Handler.CustomRevocationInfo.Assign(m_LocalRevInfo, true);
                        
                        m_Handler.AutoCollectRevocationInfo = true;
                        m_Handler.IgnoreChainValidationErrors = true;
                        sig.SigningTime = DateTime.UtcNow;
                      
                         CloseCurrentDocument(true);
                       
                    }
                    catch (Exception ex)
                    {
                      
                    }
                }
                finally
                {
                // save the signed document to blob

                    FileStream file = new FileStream(url+"\\Fisiere\\test.pdf", FileMode.Open, FileAccess.ReadWrite); ;
                  
                        

                    CloudBlobDirectory subDirectory2 = container.GetDirectoryReference("actesemnate");
                    var contentType = "pdf";
                    var streamContents = file;
                
                // subdirectory name as username to identify from the blob hierarchy who uploaded the file
                    // get a unique name 
                        var blobName = "test.pdf";

                    var blob = subDirectory2.GetBlockBlobReference(blobName);
                    blob.Properties.ContentType = contentType;
                    blob.UploadFromStream(streamContents);


                // save the signed act to db 

                    SignedAct act = new SignedAct()
                    {
                        
                        CreatePersonID = 2,
                        Name = "Certificat nastere semnat",
                        CreationDate =DateTime.Now,
                        ActID = 1,
                        ExternalUniqueReference = "test.pdf",
                        SentToClient = true,
                        Signed = true,
                        
                        
                    };
                    _db.SignedActs.Add(act);
                    _db.SaveChanges();
                   
                }
            }

       
        [HttpPost]
        public ActionResult DisplayImage(string id, long parentID)
        {
            long idAct = long.Parse(id.Split('_')[0]);

            object model = new object();

            if (parentID == -1 || parentID == -3)
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
                             a.ExternalUniqueReference
                             

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
                             sa.SentToClient
                             

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
            if (parentID == -1 || parentID == -3)
            {
                subDirectory = container.GetDirectoryReference("actenesemnate");
               blockBlob = subDirectory.GetBlockBlobReference(_db.Acts.Where(o => o.ID == idAct).FirstOrDefault().ExternalUniqueReference);
            }
            else
            {
                subDirectory = container.GetDirectoryReference("actesemnate");
                blockBlob = subDirectory.GetBlockBlobReference(_db.SignedActs.Where(o => o.ID == idAct).FirstOrDefault().ExternalUniqueReference);
            }
            
            var url = HttpContext.Request.PhysicalApplicationPath;
            string type;
            // must extract the type of file 
            if (parentID == -1 || parentID == -3)
            {
                type = "jpg";
            }
            else
            {
                type = "pdf";
            }

           
            try
            {
                using (var stream = System.IO.File.OpenWrite(url+"\\Fisiere\\temp_jpeg."+type))
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
             nameFile = "temp_jpeg."+type
            
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
            
        }

    }




