/*
Released as open source by NCC Group Plc - http://www.nccgroup.trust/
 
Developed by Ollie Whitehouse, ollie dot whitehouse at nccgroup dot trust
https://github.com/nccgroup/WMIcmd
 
Released under AGPL see LICENSE for more information
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using System.Security;

namespace WMICore
{
    /// <summary>
    /// Core CDO WMI class - note partial class to split out over multiple files
    /// </summary>
    public partial class WMICore
    {

        // The scope we use for the connection
        private ManagementScope scope;
        /// <summary>
        /// The getter / setter methods for the private scope variable above
        /// </summary>
        public ManagementScope Scope
        {
            get
            {
                return scope;
            }
            set
            {
                scope = value;

            }
        }

        // The host for this object
        private string host;


        /// <summary>
        /// Host for this object
        /// </summary>
        public string Host
        {
            get
            {
                return host;
            }
            set
            {
                host = value;

            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public WMICore(string strHost)
        {
            this.host = strHost;
        }


        /// <summary>
        /// Connect to a WMI server - if no domain, user and password supplied then defaults to local authentication
        /// </summary>
        /// <param name="strHost">Hostname or IP address</param>
        /// <param name="strDomain">Autentication domain</param>
        /// <param name="strUser">Username</param>
        /// <param name="strPass">Password</param>
        /// <returns></returns>
        public void WMIConnect(string strDomain, string strUser, string strPass)
        {
            bool bLocal = false;

            Console.WriteLine("[!] Connecting with " + strUser);

            // Are we a local connection?
            if (string.Compare(Host, System.Environment.GetEnvironmentVariable("COMPUTERNAME"), true) == 0)
            {
                bLocal = true;
            }
            else if ((strDomain ?? strUser ?? strPass) == null) // else have we been supplied what we need?
            {
                Console.WriteLine("[!] Need to specify a username, domain and password for non local connections");
                throw new System.ArgumentException("Need to specify a username, domain and password for non local connections");
            }

            Console.WriteLine("[i] Connecting to " + Host);

            ConnectionOptions connOps = new ConnectionOptions();
            try
            {
                if (strDomain.Length > 0 && bLocal != true)
                {
                    connOps.Username = strUser;
                    connOps.Authority = "NTLMDOMAIN:" + strDomain;
                    connOps.Password = strPass;
                }
                else if (bLocal != true)
                {
                    connOps.Username = strUser;
                    connOps.Password = strPass;
                }
                connOps.Impersonation = ImpersonationLevel.Impersonate;
                connOps.Authentication = AuthenticationLevel.PacketPrivacy;
                connOps.EnablePrivileges = true;
            }
            catch (System.NullReferenceException ex)
            {

            }

            if (bLocal == false)
            {
                this.scope = new ManagementScope("\\\\" + Host + "\\root\\cimv2", connOps);
            }
            else
            {
                this.scope = new ManagementScope("\\\\" + Host + "\\root\\cimv2");
            }

            // Do the connection
            try
            {
                scope.Connect();
            }
            catch (System.Management.ManagementException ex)
            {
                if (ex.Message.ToString().Contains(" 0x800706BA"))
                {
                    Console.WriteLine("[i] Check:");
                    Console.WriteLine("           - host is reachable");
                    Console.WriteLine("           - firewall (if enabled) allows connections");
                    Console.WriteLine("           - WS-Management is running on the remote host");
                }
                throw;
            }
        }

    }
}
