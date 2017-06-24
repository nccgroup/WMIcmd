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
using System.Security;
using CommandLine;
using CommandLine.Text;

namespace WMIcmd
{
    class Options
    {
        // Generic
        [Option('h', "host", Required = false,
            HelpText = "Host (IP address or hostname - default: localhost)")]
        public string Host { get; set; }

        [Option('u', "username", Required = false,
            HelpText = "Username to authenticate with")]
        public string Username { get; set; }

        [Option('p', "password", Required = false,
            HelpText = "Password to authenticate with")]
        public string Password { get; set; }

        [Option('a', "promptpass", Required = false,
            HelpText = "Prompt for password")]
        public bool PromptPassword { get; set; }

        [Option('d', "domain", Required = false,
            HelpText = "Domain to authenticate with")]
        public string Domain { get; set; }

        [Option('v', "Verbose", DefaultValue = false,
            HelpText = "Prints all messages to standard output.")]
        public bool Debug { get; set; }

        // Used by CDOHostRun
        [Option('c', "Command", Required = false, DefaultValue = null,
            HelpText = "Command to run e.g. \"nestat-ano\" ")]
        public string Command { get; set; }

        [Option('s', "CommandSleep", Required = false, DefaultValue = 10000,
        HelpText = "Command sleep in milliseconds - increase if getting truncated output")]
        public int CommandDelay { get; set; }

        [ParserState]
        public IParserState LastParserState { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this,
              (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }

        /// <summary>
        /// Get a password via prompting
        /// </summary>
        /// <returns>the supplied password</returns>
        public SecureString GetPassword()
        {
            var pwd = new SecureString();
            while (true)
            {
                ConsoleKeyInfo i = Console.ReadKey(true);
                if (i.Key == ConsoleKey.Enter)
                {
                    break;
                }
                else if (i.Key == ConsoleKey.Backspace)
                {
                    if (pwd.Length > 0)
                    {
                        pwd.RemoveAt(pwd.Length - 1);
                        Console.Write("\b \b");
                    }
                }
                else
                {
                    pwd.AppendChar(i.KeyChar);
                    Console.Write("*");
                }
            }
            return pwd;
        }

        /// <summary>
        /// Universal argument parsing the supplier Options object contains the parsed variables
        /// </summary>
        /// <param name="args">String array from main</param>
        /// <param name="options">Options object</param>
        public void ParseArgs(string[] args, Options options)
        {
            // parse the arguments
            if (CommandLine.Parser.Default.ParseArguments(args, options))
            {

                // if the user didn't supply a hostname to connect to be default to the local machine
                if (options.Host == null) options.Host = System.Environment.GetEnvironmentVariable("COMPUTERNAME");

                if(PromptPassword == true)
                {
                    Console.Write("Password: ");
                    options.Password = GetPassword().ToString();
                    Console.WriteLine();
                }
            }
            else
            {
                System.Environment.Exit(2);
            }


        }
    }
}
