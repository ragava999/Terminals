/*
 * Created by SharpDevelop.
 * User: Oliver Kohl D.Sc.
 * Date: 01.02.2016
 * Time: 08:54
 */
using System;

namespace Terminals.Configuration.Files.Credentials
{
	/// <summary>
	/// Description of ExtensionMethod.
	/// </summary>
	public static class ExtensionMethod
	{
		public static Credential ToCredential (this CredentialSet credential)
		{
			if (credential == null)
				return null;
			
			return new Credential() {
				Domain = credential.Domain,
				Password = credential.SecretKey,
				UserName = credential.Username,
			};
		}
	}
}
