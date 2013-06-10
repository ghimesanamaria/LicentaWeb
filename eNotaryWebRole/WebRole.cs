using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.Web.Administration;


namespace eNotaryWebRole
{
    public class WebRole : RoleEntryPoint
    {
        public override bool OnStart()
        {
            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
            CloudConfigurationManager.GetSetting("eNotaryCloudStorage"));
            using(var server = new ServerManager()){
            var siteNameFromServiceModel = "Web";
            var siteName = "eNotaryWebRole";
            var config = server.GetApplicationHostConfiguration();
            var accessSection = config.GetSection("system.webServer/security/access",siteName);
            accessSection["sslFlags"] = @"Ssl, SslRequireCert";
            server.CommitChanges();
            }



          

            return base.OnStart();
        }
    }
}
