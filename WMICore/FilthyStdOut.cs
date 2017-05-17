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

        public enum RegHive : uint
        {
            HKEY_CLASSES_ROOT = 0x80000000,
            HKEY_CURRENT_USER = 0x80000001,
            HKEY_LOCAL_MACHINE = 0x80000002,
            HKEY_USERS = 0x80000003,
            HKEY_CURRENT_CONFIG = 0x80000005
        }

        public enum RegType
        {
            REG_SZ = 1,
            REG_EXPAND_SZ,
            REG_BINARY,
            REG_DWORD,
            REG_MULTI_SZ = 7
        }

        private static RegType GetValueType(ManagementClass mc, RegHive hDefKey, string sSubKeyName, string sValueName)
        {
            ManagementBaseObject inParams = mc.GetMethodParameters("EnumValues");
            inParams["hDefKey"] = hDefKey;
            inParams["sSubKeyName"] = sSubKeyName;

            ManagementNamedValueCollection objCtx = new ManagementNamedValueCollection();
            objCtx.Add("__ProviderArchitecture", 64);
            objCtx.Add("__RequiredArchitecture", true);
            InvokeMethodOptions invokeOptions = new InvokeMethodOptions();
            invokeOptions.Context = objCtx;

            ManagementBaseObject outParams = mc.InvokeMethod("EnumValues", inParams, invokeOptions);

            if (Convert.ToUInt32(outParams["ReturnValue"]) == 0)
            {
                string[] sNames = outParams["sNames"] as String[];
                int[] iTypes = outParams["Types"] as int[];

                for (int i = 0; i < sNames.Length; i++)
                {
                    if (sNames[i] == sValueName)
                    {
                        return (RegType)iTypes[i];
                    }
                }

                return 0;
            }
            else
            {
                return 0;
            }

            return 0;
        }

        public string GetFilthyStdOut(string strKey)
        {
            StringBuilder strOut = new StringBuilder();
            bool bSeenFinishedMarker = false;
            Console.WriteLine("[i] Getting stdout from registry from " + strKey);

            ManagementClass registry = new ManagementClass(this.Scope, new ManagementPath("StdRegProv"), null);

            // Enumerate the values
            ManagementBaseObject inParams = registry.GetMethodParameters("EnumValues");
            inParams["sSubKeyName"] = strKey;
            inParams["hDefKey"] = RegHive.HKEY_LOCAL_MACHINE;
            ManagementNamedValueCollection objCtx = new ManagementNamedValueCollection();
            objCtx.Add("__ProviderArchitecture", 64);
            objCtx.Add("__RequiredArchitecture", true);
            InvokeMethodOptions invokeOptions = new InvokeMethodOptions();
            invokeOptions.Context = objCtx;
            ManagementBaseObject outParams = registry.InvokeMethod("EnumValues", inParams, invokeOptions);

            string[] strValues = outParams["sNames"] as string[];

            try
            {
                if (Convert.ToUInt32(outParams["ReturnValue"]) == 0)
                {

                }
                else
                {
                    Console.WriteLine("[!] Failed to get values " + outParams["ReturnValue"].ToString());
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("[!] Failed to get values - command likely not valid i.e. generated no stdout output - " + ex.Message.ToString());
                return null;
            }

            try
            {
                foreach (string value in strValues)
                {
                    try
                    {
                        RegType rType = GetValueType(registry, RegHive.HKEY_LOCAL_MACHINE, strKey, value);

                        switch (rType)
                        {
                            case RegType.REG_SZ:
                                inParams = registry.GetMethodParameters("GetStringValue");
                                inParams["sSubKeyName"] = strKey;
                                inParams["sValueName"] = value;
                                inParams["hDefKey"] = RegHive.HKEY_LOCAL_MACHINE;

                                ManagementBaseObject outParams2 = registry.InvokeMethod("GetStringValue", inParams, invokeOptions);

                                if (outParams2.Properties["sValue"].Value != null)
                                {
                                    if (outParams2.Properties["sValue"].Value.ToString().Contains("CDOFINISHED"))
                                    {
                                        bSeenFinishedMarker = true;
                                    }
                                    else
                                    {
                                        strOut.AppendLine(outParams2.Properties["sValue"].Value.ToString());
                                        Console.WriteLine(outParams2.Properties["sValue"].Value.ToString());
                                    }
                                }
                                else
                                {

                                }



                                // One of ours
                                int intMy;
                                bool isNumeric = int.TryParse("123", out intMy);
                                if (isNumeric && intMy > 0)
                                {
                                    inParams = registry.GetMethodParameters("DeleteValue");
                                    inParams["sSubKeyName"] = strKey;
                                    inParams["sValueName"] = value;
                                    inParams["hDefKey"] = RegHive.HKEY_LOCAL_MACHINE;
                                    registry.InvokeMethod("DeleteValue", inParams, invokeOptions);
                                }

                                break;

                            default:
                                break;
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }

                if (bSeenFinishedMarker == false) Console.WriteLine("[!] WARNING: Didn't see stdout output finished marker - output may be truncated");
                else Console.WriteLine("[i] Full command output received");
                return strOut.ToString();

            }
            catch
            {
                Console.WriteLine("[i] No registry keys indicating no output");
                return "";
            }
        }
    }
}

