using System;
using System.Collections.Generic;
using RestSharp.Authenticators.OAuth.Extensions;
#if !WINDOWS_PHONE && !SILVERLIGHT && !PocketPC
using RestSharp.Contrib;
#endif

namespace RestSharp.Authenticators.OAuth
{
	/// <summary>
	/// A class to encapsulate OAuth authentication flow.
	/// <seealso cref="http://oauth.net/core/1.0#anchor9"/>
	/// </summary>
	internal class OAuthWorkflow
	{
		public virtual string Version { get; set; }
		public virtual string ConsumerKey { get; set; }
		public virtual string ConsumerSecret { get; set; }
		public virtual string Token { get; set; }
		public virtual string TokenSecret { get; set; }
		public virtual string CallbackUrl { get; set; }
		public virtual string Verifier { get; set; }
		public virtual string SessionHandle { get; set; }

		public virtual OAuthSignatureMethod SignatureMethod { get; set; }
		public virtual OAuthSignatureTreatment SignatureTreatment { get; set; }
		public virtual OAuthParameterHandling ParameterHandling { get; set; }

		public virtual string ClientUsername { get; set; }
		public virtual strin