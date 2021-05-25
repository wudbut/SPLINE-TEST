// ZlibStream.cs
// ------------------------------------------------------------------
//
// Copyright (c) 2009 Dino Chiesa and Microsoft Corporation.  
// All rights reserved.
//
// This code module is part of DotNetZip, a zipfile class library.
//
// ------------------------------------------------------------------
//
// This code is licensed under the Microsoft Public License. 
// See the file License.txt for the license details.
// More info on: http://dotnetzip.codeplex.com
//
// ------------------------------------------------------------------
//
// last saved (in emacs): 
// Time-stamp: <2009-August-19 18:39:59>
//
// ------------------------------------------------------------------
//
// This module defines the ZlibStream class, which is similar in idea to
// the System.IO.Compression.DeflateStream and
// System.IO.Compression.GZipStream classes in the .NET BCL.
//
// ------------------------------------------------------------------

#if WINDOWS_PHONE

using System;
using System.IO;

namespace RestSharp.Compression.ZLib
{

	/// <summary>
	/// Represents a Zlib stream for compression or decompression.
	/// </summary>
	/// <remarks>
	///
	/// <para>
	/// The ZlibStream is a <see
	/// href="http://en.wikipedia.org/wiki/Decorator_pattern">Decorator</see> on a <see
	/// cref="System.IO.Stream"/>.  It adds ZLIB compression or decompression to any
	/// stream.
	/// </para>
	///
	/// <para> Using this stream, applications can compress or decompress data via
	/// stream <c>Read</c> and <c>Write</c> operations.  Either compresssion or
	/// decompression can occur through either reading or writing. The compression
	/// format used is ZLIB, which is documented in <see
	/// href="http://www.ietf.org/rfc/rfc1950.txt">IETF RFC 1950</see>, "ZLIB Compressed
	/// Data Format Specification version 3.3". This implementation of ZLIB always uses
	/// DEFLATE as the compression method.  (see <see
	/// href="http://www.ietf.org/rfc/rfc1951.txt">IETF RFC 1951</see>, "DEFLATE
	/// Compressed Data Format Specification version 1.3.") </para>
	///
	/// <para>
	/// The ZLIB format allows for varying compression methods, window sizes, and dictionaries.
	/// This implementation always uses the DEFLATE compression method, a preset dictionary,
	/// and 15 window bits by default.  
	/// </para>
	///
	/// <para>
	/// This class is similar to <see cref="DeflateStream"/>, except that it adds the
	/// RFC1950 header and trailer bytes to a compressed stream when compressing, or expects
	/// the RFC1950 header and trailer bytes when decompressing.  It is also similar to the
	/// <see cref="GZipStream"/>.
	/// </para>
	/// </remarks>
	/// <seealso cref="DeflateStream" />
	/// <seealso cref="GZipStream" />
	internal class ZlibStream : System.IO.Stream
	{
		internal ZlibBaseStream _baseStream;
		bool _disposed;

		public ZlibStream(System.IO.Stream stream)
		{
			_baseStream = new ZlibBaseStream(stream, ZlibStreamFlavor.ZLIB, false);
		}

		#region Zlib properties

		/// <summary>
		/// This property sets the flush behavior on the stream.  
		/// Sorry, though, not sure exactly how to describe all the various settings.
		/// </summary>
		virtual public FlushType FlushMode
		{
			get { return (this._baseStream._flushMode); }
			set
			{
				if (_disposed) throw new ObjectDisposedException("ZlibStream");
				this._baseStream._flushMode = value;
			}
		}

		/// <summary>
		/// The size of the working buffer for the compression codec. 
		/// </summary>
		///
		/// <remarks>
		/// <para>
		/// The working buffer is used for all stream operations.  The default size is 1024 bytes.
		/// The minimum size is 128 bytes. You may get better performance with a larger buffer.
		/// Then again, you might not.  You would have to test it.
		/// </para>
		///
		/// <para>
		/// Set this before the first call to Read()  or Write() on the stream. If you try to set it 
		/// afterwards, it will throw