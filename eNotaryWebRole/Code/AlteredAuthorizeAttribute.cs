using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eNotaryWebRole.Code
{
	public class AlteredAuthorizeAttribute : AuthorizeAttribute
	{
		private bool useAuthentication = AuthConfigurationProvider.Instance.UseAuthentication;

		public override void OnAuthorization( AuthorizationContext filterContext )
		{
			if( useAuthentication )
				base.OnAuthorization( filterContext );
		}

		protected override HttpValidationStatus OnCacheAuthorization( HttpContextBase httpContext )
		{
			if( useAuthentication )
				return base.OnCacheAuthorization( httpContext );

			return HttpValidationStatus.Valid;
		}
	}
}