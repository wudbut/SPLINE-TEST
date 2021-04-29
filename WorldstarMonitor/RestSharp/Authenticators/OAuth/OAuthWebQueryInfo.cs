using System;

namespace RestSharp.Authenticators.OAuth
{
#if !SILVERLIGHT && !WINDOWS_PHONE && !PocketPC
	[Serializable]
#endif
	public class OAuthWebQueryInfo
	{
		public virtual string ConsumerKey { get; set; }
		public virtual string Token { get; set; }
		public virtual string Nonce { get; set; }
		public virtual string Timestamp { get; set; }
		public virtual string SignatureMethod { get; set; }
		public virtual string Signature { get; set; }
		public virtual string Version { get; set; }
		public virtual string Callback { get; set; }
		public virtual string Verifier { get; set; }
		