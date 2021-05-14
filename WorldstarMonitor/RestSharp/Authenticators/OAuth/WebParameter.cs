#if !Smartphone
using System;
using System.Diagnostics;

#endif

namespace RestSharp.Authenticators.OAuth
{
#if !Smartphone && !PocketPC
	[DebuggerDisplay("{Name}:{Value}