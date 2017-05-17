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

namespace WMICore
{
    public partial class WMICore
    {
        public Int32 CreateProcess(string strCommandLine)
        {


            using (var managementClass = new ManagementClass(this.Scope,
                                         new ManagementPath("Win32_Process"),
                                         new ObjectGetOptions()))
            {
                var inputParams = managementClass.GetMethodParameters("Create");

                inputParams["CommandLine"] = strCommandLine;

                var outParams = managementClass.InvokeMethod("Create",
                                                             inputParams,
                                                             new InvokeMethodOptions());

                var ret = Convert.ToInt32(outParams["ReturnValue"]);

                return Convert.ToInt32(outParams["ProcessId"]);
            }
        }

    }
}
