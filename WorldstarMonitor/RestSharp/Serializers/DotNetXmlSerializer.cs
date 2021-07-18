using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace RestSharp.Serializers
{
	/// <summary>
	/// Wrapper for System.Xml.Serialization.XmlSerializer.
	/// </summary>
	public class DotNetXmlSerializer : ISerializer
	{
		/// <summary>
		/// Default constructor, does not specify namespace
		/// </summary>
		public DotNetXmlSerializer()
		{
			ContentType = "application/xml";
			Encoding = Encoding.UTF8;
		}

		/// <summary>
		/// Specify the namespaced to be used when serializing
		/// </summary>
		/// <param name="namespace">XML namespace</param>
		public DotNetXmlSerializer(string @namespace) : this()
		{
			Namespace = @namespace;
		}

		/// <summary>
		/// Serialize the object as XML
		/// </summary>
		/// <param na