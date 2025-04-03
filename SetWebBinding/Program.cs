using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Web.Administration;
using System.Security.Cryptography.X509Certificates;

namespace SetWebBinding
{
    internal class Program
    {
        static int Main(string[] args)
        {
            var argString = String.Join(" ", args);

            string siteName = "";  // Change to your IIS site name
            string certThumbprint = "";  // Replace with actual certificate thumbprint
            string certStoreName = "";  // Usually "MY" (Personal Store)
            int sslPort = 443;  // Change if necessary
            string hostheader = "";

            var split = argString.Split('-');

            foreach (var arg in split)
            {
                if (arg.Contains(" "))
                {
                    var prop = arg.Substring(0, arg.IndexOf(' ')).Trim();
                    var val = arg.Substring(arg.IndexOf(" ") + 1).Trim(' ', '"');
                    switch (prop.ToUpper())
                    {
                        case "THUMB":
                            certThumbprint = val;
                            break;
                        case "STORE":
                            certStoreName = val;
                            break;
                        case "SITE":
                            siteName = val;
                            break;
                        case "PORT":
                            Int32.TryParse(val, out sslPort);
                            break;
                        case "HOST":
                            hostheader = val;
                            break;
                    }
                }
            }

            Boolean isok = true;

            if (siteName.Trim() == "")
            {
                isok = false;
            }
            if (certThumbprint.Trim() == "")
            {
                isok = false;
            }
            if (certStoreName.Trim() == "")
            {
                isok = false;
            }
            if (hostheader.Trim() == "")
            {
                isok = false;
            }

            if (!isok)
            {
                Console.WriteLine("A required parameter is missing.");
                Console.WriteLine(
@"
Example usage:
sslbinding.exe -thumb ""xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx"" -store ""My"" -site ""Doogle"" -port ""443"" -host ""www.doogle.com""

    -thumb ""somethumb""
    This is the certificate thumbprint. Certificates should typically be located in LocalMachine\My.

    -store ""somestore""
    This is the location store of the certificate. Usually ""My"" is where you'd place the certificate.

    -site ""somesite""
    This is the name of the site.

    -port ""someport""
    This is the port number used for binding.

    -host ""somehost""
    This is the IIS hostheader to use. You would normally use a
    FQDN that exists as a subject alternate name in the certificate.
");
                return 1;
            }

            X509Certificate2 cert;
            X509Store store = new X509Store(StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadOnly);
            var certcoll = store.Certificates.Find(X509FindType.FindByThumbprint, certThumbprint, false);

            cert = certcoll[0];

            using (ServerManager serverManager = new ServerManager())
            {
                Site site = serverManager.Sites[siteName];
                if (site == null)
                {
                    Console.WriteLine("Site not found!");
                    return 1;
                }

                // Get existing SSL binding if it exists
                var existingBinding = site.Bindings.FirstOrDefault(b => b.Protocol == "https" && b.EndPoint.Port == sslPort);
                if (existingBinding != null)
                {
                    site.Bindings.Remove(existingBinding);
                }

                // Add a new HTTPS binding
                string bindingInformation = String.Format("*:{0}:{1}", sslPort, hostheader);
                Binding newBinding = site.Bindings.CreateElement();
                newBinding.Protocol = "https";
                newBinding.CertificateStoreName = certStoreName;
                newBinding.CertificateHash = cert.GetCertHash();
                newBinding.BindingInformation = bindingInformation;
                site.Bindings.Add(newBinding);

                try
                {
                    serverManager.CommitChanges();
                    Console.WriteLine("SSL certificate has been set successfully.");
                }
                catch (Exception)
                {

                    return 1;
                }

                return 0;
            }
        }
    }
}
