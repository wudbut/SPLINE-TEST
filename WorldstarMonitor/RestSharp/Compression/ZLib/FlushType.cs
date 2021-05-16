#if WINDOWS_PHONE

using System;
using System.Net;

namespace RestSharp.Compression.ZLib
{

	/// <summary>
	/// Describes how to flush the current deflate operation. 
	/// </summary>
	/// <remarks>
	/// The different FlushType values are useful when using a Deflate in a streaming application.
	/// </remarks>
	internal enum FlushType
	{
		/// <summary>No flush at all.</summary>
		None = 0,

		/// <summary>Closes the current block, but doesn't flush it to
		/// the output. Used internally only in hypothetical
		/// scenarios.  This was supposed to be removed by Zlib, but it is
		/// still in use in some edge cases. 
		/// </summary>
		Partial,

		/// <summary>
		/// Use this during compression to specify that all pending output should be
		/// flushed to the output buffer and the output should be aligned on a byte
		/// boundary.  You might use this in a streaming communication scenario, so that
		/// the decompressor can get all input data available so far.  When using this
		/// with a ZlibCodec, <c>AvailableBytesIn</c> will be zero after the call if
		/// enough output space has been provided before the call.  Flushing will
		/// degrade compression and so it should be used only whe