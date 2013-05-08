using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Web;

namespace eNotaryWebRole.Code
{
	public class AuthConfigurationProvider
	{
		private NameValueCollection _section;

		private static AuthConfigurationProvider _instance = null;
		public static AuthConfigurationProvider Instance
		{
			get
			{
				if( _instance == null )
					_instance = new AuthConfigurationProvider();

				return _instance;
			}
		}

		public AuthConfigurationProvider()
		{
			_section = (NameValueCollection) ConfigurationManager.GetSection( "AuthConfig" );
		}

		public bool UseAuthentication
		{
			get
			{
				return bool.Parse(_section["use_authentication"]);
			}
		}
        public string LDAP_DirIdentifier
        {
            get
            {
                return _section["LDAP_DirIdentifier"];
            }
        }
        public string LDAP_Credential
        {
            get
            {
                return _section["LDAP_DirCredential"];
            }
        }

	}
}