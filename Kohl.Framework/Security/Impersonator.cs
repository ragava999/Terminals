namespace Kohl.Framework.Security
{
    using PInvoke;
    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using System.Security.Principal;

    public enum LogonType
    {
        LOGON32_LOGON_INTERACTIVE = 2,
        LOGON32_LOGON_NETWORK = 3,
        LOGON32_LOGON_BATCH = 4,
        LOGON32_LOGON_SERVICE = 5,
        LOGON32_LOGON_UNLOCK = 7,
        LOGON32_LOGON_NETWORK_CLEARTEXT = 8, // Win2K or higher
        LOGON32_LOGON_NEW_CREDENTIALS = 9 // Win2K or higher
    };

    public enum LogonProvider
    {
        LOGON32_PROVIDER_DEFAULT = 0,
        LOGON32_PROVIDER_WINNT35 = 1,
        LOGON32_PROVIDER_WINNT40 = 2,
        LOGON32_PROVIDER_WINNT50 = 3
    };

    public enum ImpersonationLevel
    {
        SecurityAnonymous = 0,
        SecurityIdentification = 1,
        SecurityImpersonation = 2,
        SecurityDelegation = 3
    }

    /// <summary>
    /// Allows code to be executed under the security context of a specified user account.
    /// </summary>
    /// <remarks> 
    ///
    /// Implements IDispose, so can be used via a using-directive or method calls;
    ///  ...
    ///
    ///  var imp = new Impersonator( "myUsername", "myDomainname", "myPassword" );
    ///  imp.UndoImpersonation();
    ///
    ///  ...
    ///
    ///   var imp = new Impersonator();
    ///  imp.Impersonate("myUsername", "myDomainname", "myPassword");
    ///  imp.UndoImpersonation();
    ///
    ///  ...
    ///
    ///  using ( new Impersonator( "myUsername", "myDomainname", "myPassword" ) )
    ///  {
    ///   ...
    ///   1
    ///   ...
    ///  }
    ///
    ///  ...
    /// </remarks>
    public sealed class Impersonator : IDisposable
    {
        private WindowsImpersonationContext _wic;

        /// <summary>
        /// Begins impersonation with the given credentials, Logon type and Logon provider.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="domainName">Name of the domain.</param>
        /// <param name="password">The password. <see cref="System.String"/></param>
        /// <param name="logonType">Type of the logon.</param>
        /// <param name="logonProvider">The logon provider. <see cref="Mit.Sharepoint.WebParts.EventLogQuery.Network.LogonProvider"/></param>
        public Impersonator(Credential credential, LogonType logonType, LogonProvider logonProvider)
        {
            Impersonate(credential, logonType, logonProvider);
        }

        /// <summary>
        /// Begins impersonation with the given credentials.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="domainName">Name of the domain.</param>
        /// <param name="password">The password. <see cref="System.String"/></param>
        public Impersonator(Credential credential)
        {
            Impersonate(credential, LogonType.LOGON32_LOGON_INTERACTIVE, LogonProvider.LOGON32_PROVIDER_DEFAULT);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Impersonator"/> class.
        /// </summary>
        public Impersonator()
        { }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            UndoImpersonation();
        }

        /// <summary>
        /// Impersonates the specified user account.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="domainName">Name of the domain.</param>
        /// <param name="password">The password. <see cref="System.String"/></param>
        public void Impersonate(Credential credential)
        {
            Impersonate(credential, LogonType.LOGON32_LOGON_INTERACTIVE, LogonProvider.LOGON32_PROVIDER_DEFAULT);
        }

        /// <summary>
        /// Impersonates the specified user account.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="domainName">Name of the domain.</param>
        /// <param name="password">The password. <see cref="System.String"/></param>
        /// <param name="logonType">Type of the logon.</param>
        /// <param name="logonProvider">The logon provider. <see cref="Mit.Sharepoint.WebParts.EventLogQuery.Network.LogonProvider"/></param>
        public void Impersonate(Credential credential, LogonType logonType, LogonProvider logonProvider)
        {
            UndoImpersonation();

            IntPtr logonToken = IntPtr.Zero;
            IntPtr logonTokenDuplicate = IntPtr.Zero;
            try
            {
                // revert to the application pool identity, saving the identity of the current requestor
                _wic = WindowsIdentity.Impersonate(IntPtr.Zero);

                // 1. STEP: do logon
                bool success = (NativeMethods.LogonUser(credential.UserName, credential.Domain, credential.Password, (int)logonType, (int)logonProvider, ref logonToken) != 0);

                int lastError = Marshal.GetLastWin32Error();

                if (lastError != 0)
                    throw new Win32Exception(lastError);

                if (success)
                {
                    success = (NativeMethods.DuplicateToken(logonToken, (int)ImpersonationLevel.SecurityImpersonation, ref logonTokenDuplicate) != 0);

                    lastError = Marshal.GetLastWin32Error();

                    if (lastError != 0)
                        throw new Win32Exception(lastError);

                    // 2. STEP: impersonate
                    if (success)
                    {
                        var wi = new WindowsIdentity(logonTokenDuplicate);
                        // discard the returned identity context (which is the context of the application pool)
                        wi.Impersonate();
                    }
                }
            }
            finally
            {
                if (logonToken != IntPtr.Zero)
                    NativeMethods.CloseHandle(logonToken);

                if (logonTokenDuplicate != IntPtr.Zero)
                    NativeMethods.CloseHandle(logonTokenDuplicate);
            }
        }

        /// <summary>
        /// Stops impersonation.
        /// </summary>
        private void UndoImpersonation()
        {
            // restore saved requestor identity
            if (_wic != null)
                _wic.Undo();
            _wic = null;
        }
    }
}
