
﻿#region License
//   Copyright 2010 John Sheehan
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License. 
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using RestSharp.Extensions;
using RestSharp.Serializers;

namespace RestSharp
{
	/// <summary>
	/// Container for data used to make requests
	/// </summary>
	public class RestRequest : IRestRequest
	{
		/// <summary>
		/// Always send a multipart/form-data request - even when no Files are present.
		/// </summary>
		public bool AlwaysMultipartFormData { get; set; }
		
		/// <summary>
		/// Serializer to use when writing JSON request bodies. Used if RequestFormat is Json.
		/// By default the included JsonSerializer is used (currently using JSON.NET default serialization).
		/// </summary>
		public ISerializer JsonSerializer { get; set; }

		/// <summary>
		/// Serializer to use when writing XML request bodies. Used if RequestFormat is Xml.
		/// By default the included XmlSerializer is used.
		/// </summary>
		public ISerializer XmlSerializer { get; set; }

		/// <summary>
		/// Set this to write response to Stream rather than reading into memory.
		/// </summary>
		public Action<Stream> ResponseWriter { get; set; }

		/// <summary>
		/// Determine whether or not the "default credentials" (e.g. the user account under which the current process is running)
		/// will be sent along to the server. The default is false.
		/// </summary>
		public bool UseDefaultCredentials { get; set; }

		/// <summary>
		/// Default constructor
		/// </summary>
		public RestRequest()
		{
			Parameters = new List<Parameter>();
			Files = new List<FileParameter>();
			XmlSerializer = new XmlSerializer();
			JsonSerializer = new JsonSerializer();

			OnBeforeDeserialization = r => { };
		}

		/// <summary>
		/// Sets Method property to value of method
		/// </summary>
		/// <param name="method">Method to use for this request</param>
		public RestRequest(Method method)
			: this()
		{
			Method = method;
		}

		/// <summary>
		/// Sets Resource property
		/// </summary>
		/// <param name="resource">Resource to use for this request</param>
		public RestRequest(string resource)
			: this(resource, Method.GET)
		{
		}

		/// <summary>
		/// Sets Resource and Method properties
		/// </summary>
		/// <param name="resource">Resource to use for this request</param>
		/// <param name="method">Method to use for this request</param>
		public RestRequest(string resource, Method method)
			: this()
		{
			Resource = resource;
			Method = method;
		}


		/// <summary>
		/// Sets Resource property
		/// </summary>
		/// <param name="resource">Resource to use for this request</param>
		public RestRequest(Uri resource)
			: this(resource, Method.GET)
		{

		}

		/// <summary>
		/// Sets Resource and Method properties
		/// </summary>
		/// <param name="resource">Resource to use for this request</param>
		/// <param name="method">Method to use for this request</param>
		public RestRequest(Uri resource, Method method)
			: this(resource.IsAbsoluteUri ? resource.AbsolutePath + resource.Query : resource.OriginalString, method)
		{
			//resource.PathAndQuery not supported by Silverlight :(
		}

		/// <summary>
		/// Adds a file to the Files collection to be included with a POST or PUT request 
		/// (other methods do not support file uploads).
		/// </summary>
		/// <param name="name">The parameter name to use in the request</param>
		/// <param name="path">Full path to file to upload</param>
		/// <returns>This request</returns>
		public IRestRequest AddFile (string name, string path)
		{
			FileInfo f = new FileInfo (path);
			long fileLength = f.Length;
			return AddFile(new FileParameter
			{
				Name = name,
				FileName = Path.GetFileName(path),
				ContentLength = fileLength,
				Writer = s =>
				{
					using(var file = new StreamReader(path))
					{
						file.BaseStream.CopyTo(s);
					}
				}
			});
		}

		/// <summary>
		/// Adds the bytes to the Files collection with the specified file name
		/// </summary>
		/// <param name="name">The parameter name to use in the request</param>
		/// <param name="bytes">The file data</param>
		/// <param name="fileName">The file name to use for the uploaded file</param>
		/// <returns>This request</returns>
		public IRestRequest AddFile (string name, byte[] bytes, string fileName)
		{
			return AddFile(FileParameter.Create(name, bytes, fileName));
		}

		/// <summary>
		/// Adds the bytes to the Files collection with the specified file name and content type
		/// </summary>
		/// <param name="name">The parameter name to use in the request</param>
		/// <param name="bytes">The file data</param>
		/// <param name="fileName">The file name to use for the uploaded file</param>
		/// <param name="contentType">The MIME type of the file to upload</param>
		/// <returns>This request</returns>
		public IRestRequest AddFile (string name, byte[] bytes, string fileName, string contentType)
		{
			return AddFile(FileParameter.Create(name, bytes, fileName, contentType));
		}

		/// <summary>
		/// Adds the bytes to the Files collection with the specified file name and content type
		/// </summary>
		/// <param name="name">The parameter name to use in the request</param>
		/// <param name="writer">A function that writes directly to the stream.  Should NOT close the stream.</param>
		/// <param name="fileName">The file name to use for the uploaded file</param>
		/// <returns>This request</returns>
		public IRestRequest AddFile (string name, Action<Stream> writer, string fileName)
		{
			return AddFile(name, writer, fileName, null);
		}

		/// <summary>
		/// Adds the bytes to the Files collection with the specified file name and content type
		/// </summary>
		/// <param name="name">The parameter name to use in the request</param>
		/// <param name="writer">A function that writes directly to the stream.  Should NOT close the stream.</param>
		/// <param name="fileName">The file name to use for the uploaded file</param>
		/// <param name="contentType">The MIME type of the file to upload</param>
		/// <returns>This request</returns>
		public IRestRequest AddFile (string name, Action<Stream> writer, string fileName, string contentType)
		{
			return AddFile(new FileParameter { Name = name, Writer = writer, FileName = fileName, ContentType = contentType });
		}

		private IRestRequest AddFile (FileParameter file)
		{
			Files.Add(file);
			return this;
		}

		/// <summary>
		/// Serializes obj to format specified by RequestFormat, but passes xmlNamespace if using the default XmlSerializer
		/// </summary>
		/// <param name="obj">The object to serialize</param>
		/// <param name="xmlNamespace">The XML namespace to use when serializing</param>
		/// <returns>This request</returns>
		public IRestRequest AddBody (object obj, string xmlNamespace)
		{
			string serialized;
			string contentType;

			switch (RequestFormat)
			{
				case DataFormat.Json:
					serialized = JsonSerializer.Serialize(obj);
					contentType = JsonSerializer.ContentType;
					break;

				case DataFormat.Xml:
					XmlSerializer.Namespace = xmlNamespace;
					serialized = XmlSerializer.Serialize(obj);
					contentType = XmlSerializer.ContentType;
					break;

				default:
					serialized = "";
					contentType = "";
					break;
			}

			// passing the content type as the parameter name because there can only be
			// one parameter with ParameterType.RequestBody so name isn't used otherwise
			// it's a hack, but it works :)
			return AddParameter(contentType, serialized, ParameterType.RequestBody);
		}

		/// <summary>
		/// Serializes obj to data format specified by RequestFormat and adds it to the request body.
		/// </summary>
		/// <param name="obj">The object to serialize</param>
		/// <returns>This request</returns>
		public IRestRequest AddBody (object obj)
		{
			return AddBody(obj, "");
		}

		/// <summary>
		/// Calls AddParameter() for all public, readable properties specified in the white list
		/// </summary>
		/// <example>
		/// request.AddObject(product, "ProductId", "Price", ...);
		/// </example>
		/// <param name="obj">The object with properties to add as parameters</param>
		/// <param name="whitelist">The names of the properties to include</param>
		/// <returns>This request</returns>
		public IRestRequest AddObject (object obj, params string[] whitelist)
		{
			// automatically create parameters from object props
			var type = obj.GetType();
			var props = type.GetProperties();

			foreach (var prop in props)
			{
				bool isAllowed = whitelist.Length == 0 || (whitelist.Length > 0 && whitelist.Contains(prop.Name));

				if (isAllowed)
				{
					var propType = prop.PropertyType;
					var val = prop.GetValue(obj, null);

					if (val != null)
					{
						if (propType.IsArray)
						{
							var elementType = propType.GetElementType();

							if (((Array)val).Length > 0 && (elementType.IsPrimitive || elementType.IsValueType || elementType == typeof(string))) {
								// convert the array to an array of strings
								var values = (from object item in ((Array)val) select item.ToString()).ToArray<string>();
								val = string.Join(",", values);
							} else {
								// try to cast it
								val = string.Join(",", (string[])val);
							}
						}

						AddParameter(prop.Name, val);
					}
				}
			}

			return this;
		}

		/// <summary>
		/// Calls AddParameter() for all public, readable properties of obj
		/// </summary>
		/// <param name="obj">The object with properties to add as parameters</param>
		/// <returns>This request</returns>
		public IRestRequest AddObject (object obj)
		{
			AddObject(obj, new string[] { });
			return this;
		}

		/// <summary>
		/// Add the parameter to the request
		/// </summary>
		/// <param name="p">Parameter to add</param>
		/// <returns></returns>
		public IRestRequest AddParameter (Parameter p)
		{
			Parameters.Add(p);
			return this;
		}

		/// <summary>
		/// Adds a HTTP parameter to the request (QueryString for GET, DELETE, OPTIONS and HEAD; Encoded form for POST and PUT)
		/// </summary>
		/// <param name="name">Name of the parameter</param>