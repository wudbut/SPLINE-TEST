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
		public virtual string ClientPassword { get; set; }

		/// <seealso cref="http://oauth.net/core/1.0#request_urls"/>
		public virtual string RequestTokenUrl { get; set; }

		/// <seealso cref="http://oauth.net/core/1.0#request_urls"/>
		public virtual string AccessTokenUrl { get; set; }

		/// <seealso cref="http://oauth.net/core/1.0#request_urls"/>
		public virtual string AuthorizationUrl { get; set; }

		/// <summary>
		/// Generates a <see cref="OAuthWebQueryInfo"/> instance to pass to an
		/// <see cref="IAuthenticator" /> for the purpose of requesting an
		/// unauthorized request token.
		/// </summary>
		/// <param name="method">The HTTP method for the intended request</param>
		/// <seealso cref="http://oauth.net/core/1.0#anchor9"/>
		/// <returns></returns>
		public OAuthWebQueryInfo BuildRequestTokenInfo(string method)
		{
			return BuildRequestTokenInfo(method, null);
		}

		/// <summary>
		/// Generates a <see cref="OAuthWebQueryInfo"/> instance to pass to an
		/// <see cref="IAuthenticator" /> for the purpose of requesting an
		/// unauthorized request token.
		/// </summary>
		/// <param name="method">The HTTP method for the intended request</param>
		/// <param name="parameters">Any existing, non-OAuth query parameters desired in the request</param>
		/// <seealso cref="http://oauth.net/core/1.0#anchor9"/>
		/// <returns></returns>
		public virtual OAuthWebQueryInfo BuildRequestTokenInfo(string method, WebParameterCollection parameters)
		{
			ValidateTokenRequestState();

			if (parameters == null)
			{
				parameters = new WebParameterCollection();
			}

			var timestamp = OAuthTools.GetTimestamp();
			var nonce = OAuthTools.GetNonce();

			AddAuthParameters(parameters, timestamp, nonce);

			var signatureBase = OAuthTools.ConcatenateRequestElements(method, RequestTokenUrl, parameters);
			var signature = OAuthTools.GetSignature(SignatureMethod, SignatureTreatment, signatureBase, ConsumerSecret);

			var info = new OAuthWebQueryInfo
			{
				WebMethod = method,
				ParameterHandling = ParameterHandling,
				ConsumerKey = ConsumerKey,
				SignatureMethod = SignatureMethod.ToRequestValue(),
				SignatureTreatment = SignatureTreatment,
				Signature = signature,
				Timestamp = timestamp,
				Nonce = nonce,
				Version = Version ?? "1.0",
				Callback = OAuthTools.UrlEncodeRelaxed(CallbackUrl ?? ""),
				TokenSecret = TokenSecret,
				ConsumerSecret = ConsumerSecret
			};

			return info;
		}

		/// <summary>
		/// Generates a <see cref="OAuthWebQueryInfo"/> instance to pass to an
		/// <see cref="IAuthenticator" /> for the purpose of exchanging a request token
		/// for an access token authorized by the user at the Service Provider site.
		/// </summary>
		/// <param name="method">The HTTP method for the intended request</param>
		/// <seealso cref="http://oauth.net/core/1.0#anchor9"/>
		public virtual OAuthWebQueryInfo BuildAccessTokenInfo(string method)
		{
			return BuildAccessTokenInfo(method, null);
		}

		/// <summary>
		/// Generates a <see cref="OAuthWebQueryInfo"/> instance to pass to an
		/// <see cref="IAuthenticator" /> for the purpose of exchanging a request token
		/// for an access token authorized by the user at the Service Provider site.
		/// </summary>
		/// <param name="method">The HTTP method for the intended request</param>
		/// <seealso cref="http://oauth.net/core/1.0#anchor9"/>
		/// <param name="parameters">Any existing, non-OAuth query parameters desired in the request</param>
		public virtual OAuthWebQueryInfo BuildAccessTokenInfo(string method, WebParameterCollection parameters)
		{
			ValidateAccessRequestState();

			if (parameters == null)
			{
				parameters = new WebParameterCollection();
			}

			var uri = new Uri(AccessTokenUrl);
			var timestamp = OAuthTools.GetTimestamp();
			var nonce = OAuthTools.GetNonce();

			AddAuthParameters(parameters, timestamp, nonce);

			var signatureBase = OAuthTools.ConcatenateRequestElements(method, uri.ToString(), parameters);
			var signature = OAuthTools.GetSignature(SignatureMethod, SignatureTreatment, signatureBase, ConsumerSecret, TokenSecret);

			var info = new OAuthWebQueryInfo
			{
				WebMethod = method,
				ParameterHandling = ParameterHandling,
				ConsumerKey = ConsumerKey,
				Token = Token,
				SignatureMethod = SignatureMethod.ToRequestValue(),
				SignatureTreatment = SignatureTreatment,
				Signature = signature,
				Timestamp = timestamp,
				Nonce = nonce,
				Version = Version ?? "1.0",
				Verifier = Verifier,
				Callback = CallbackUrl,
				TokenSecret = TokenSecret,
				ConsumerSecret = ConsumerSecret,