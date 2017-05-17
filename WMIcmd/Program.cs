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
using System.Threading;
using System.Reflection;
using System.IO;

namespace WMIcmd
{
    class Program
    {
        static void Main(string[] args)
        {
            // This is how all the programs in this project can parse arguments
            var Options = new Options();
            Options.ParseArgs(args, Options);
            WMICore.WMICore myWMICore = new WMICore.WMICore(Options.Host);

            try
            {
                myWMICore.WMIConnect(Options.Domain, Options.Username, Options.Password);
            }
            catch (Exception ex)
            {
                Console.WriteLine("[!] Failed to connect " + ex.Message.ToString());
                System.Environment.Exit(1);
            }

            Console.WriteLine("[i] Connected");

            Assembly _assembly;
            StreamReader _textStreamReader;
            string strLine;

            _assembly = Assembly.GetExecutingAssembly();

            _textStreamReader = new StreamReader(_assembly.GetManifestResourceStream("WMIcmd.CommandLine.txt"));

            if (Options.Command != null) Console.WriteLine("[i] Command: " + Options.Command);
            else
            {
                Console.WriteLine("[!] You need to specify a command with -c or --command see --help");
                System.Environment.Exit(1);
            }

            
            // Read the template in
            while ((strLine = _textStreamReader.ReadLine()) != null)
            {
                Console.WriteLine("[i] Running command...");
                string strEscapedUserCommand = Options.Command.Replace("=", "^=");
                string strActualCommand = strLine.Replace("PLACEHOLDER", strEscapedUserCommand);
                myWMICore.CreateProcess(strActualCommand);
                Thread.Sleep(Options.CommandDelay);
            }

            // Output the commands stdout
            Console.WriteLine(myWMICore.GetFilthyStdOut("SOFTWARE\\"));
        }
    }
}
