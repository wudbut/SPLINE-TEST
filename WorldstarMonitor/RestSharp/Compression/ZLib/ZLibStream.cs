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