namespace Kohl.Framework.Security
{
    public class Credential : System.Net.NetworkCredential
    {
        // Wrapper
        public string DomainName
        {
            get
            {
                return Domain;
            }
            set
            {
                this.Domain = value;
            }
        }
        
        public string UserNameWithDomain
        {
        	get
        	{
        		return Domain + "\\" + UserName;
        	}
        }

    }
}
