/*
 * Created by SharpDevelop.
 * User: Oliver Kohl D.Sc.
 * Date: 08.02.2016
 * Time: 13:29
 */
using System;

namespace Terminals.Configuration
{
	/// <summary>
	/// Description of Url.
	/// </summary>
	public static class Url
	{
		public const string GitHubRepositry = "https://github.com/OliverKohlDSc/Terminals";
		
		public const string GitHubLatestRelease_Link = "https://github.com/OliverKohlDSc/Terminals/releases/latest";
		
		// https://github.com/OliverKohlDSc/Terminals/commits/master.atom
		// https://github.com/OliverKohlDSc/Terminals/releases.atom
		public const string GitHubReleasesFeed = "https://github.com/OliverKohlDSc/Terminals/releases.atom";
		
		// https://github.com/OliverKohlDSc/Terminals/releases/tag/4.8.0.0
	    // https://github.com/OliverKohlDSc/Terminals/releases/download/4.8.0.0/Terminals_4.8.0.0.zip
		public const string GitHubLatestRelease_Binary = "https://github.com/OliverKohlDSc/Terminals/releases/download/{0}/Terminals_{1}.zip";
	}
}
