using System;
using System.Management;
using System.Security.Cryptography;
using System.Text;

namespace Infomat.InfomatMachineId
{
    public class MachineIdGenerator
    {
        private static MachineIdGenerator _instance;

        public string MachineId { get; private set; } = string.Empty;

        private void GenerateMachineId() 
        {
            if (string.IsNullOrEmpty(MachineId))
            {
                MachineId = GetHash("CPU >> " + CpuId() + "\nBIOS >> " +
            BiosId() + "\nBASE >> " + BaseId()
                            +"\nDISK >> "+ DiskId() + "\nVIDEO >> " + 
            VideoId() 
                                     );
            }
        }


        private MachineIdGenerator()
        {
            GenerateMachineId();
        }

        public static MachineIdGenerator Instance => _instance ?? (_instance = new MachineIdGenerator());

        private static string GetHash(string s)
        {
            MD5 sec = new MD5CryptoServiceProvider();
            ASCIIEncoding enc = new ASCIIEncoding();
            byte[] bt = enc.GetBytes(s);
            return GetHexString(sec.ComputeHash(bt));
        }
        private static string GetHexString(byte[] bt)
        {
            string s = string.Empty;
            for (var i = 0; i < bt.Length; i++)
            {
                byte b = bt[i];
                int n = b;
                int n1 = n & 15;
                int n2 = (n >> 4) & 15;
                if (n2 > 9)
                    s += ((char)(n2 - 10 + 'A')).ToString();
                else
                    s += n2.ToString();
                if (n1 > 9)
                    s += ((char)(n1 - 10 + 'A')).ToString();
                else
                    s += n1.ToString();
                if ((i + 1) != bt.Length && (i + 1) % 2 == 0) s += "-";
            }
            return s;
        }
        #region Original Device ID Getting Code
        //Return a hardware identifier
        private static string Identifier(string wmiClass, string wmiProperty)
        {
            string result = "";
            ManagementClass mc =
        new ManagementClass(wmiClass);
            ManagementObjectCollection moc = mc.GetInstances();
            foreach (var o in moc)
            {
                var mo = (ManagementObject) o;
                //Only get the first one
                if (result == "")
                {
                    try
                    {
                        result = mo[wmiProperty].ToString();
                        break;
                    }
                    catch
                    {
                        // ignored
                    }
                }
            }
            return result;
        }
        private static string CpuId()
        {
            //Uses first CPU identifier available in order of preference
            //Don't get all identifiers, as it is very time consuming
            string retVal = Identifier("Win32_Processor", "UniqueId");
            if (retVal == "") //If no UniqueID, use ProcessorID
            {
                retVal = Identifier("Win32_Processor", "ProcessorId");
                if (retVal == "") //If no ProcessorId, use Name
                {
                    retVal = Identifier("Win32_Processor", "Name");
                    if (retVal == "") //If no Name, use Manufacturer
                    {
                        retVal = Identifier("Win32_Processor", "Manufacturer");
                    }
                    //Add clock speed for extra security
                    retVal += Identifier("Win32_Processor", "MaxClockSpeed");
                }
            }
            return retVal;
        }
        //BIOS Identifier
        private static string BiosId()
        {
            return Identifier("Win32_BIOS", "Manufacturer")
            + Identifier("Win32_BIOS", "SMBIOSBIOSVersion")
            + Identifier("Win32_BIOS", "IdentificationCode")
            + Identifier("Win32_BIOS", "SerialNumber")
            + Identifier("Win32_BIOS", "ReleaseDate")
            + Identifier("Win32_BIOS", "Version");
        }
        //Main physical hard drive ID
        private static string DiskId()
        {
            string serialNumber = string.Empty;


            string systemLogicalDiskDeviceId = Environment.GetFolderPath(Environment.SpecialFolder.System).Substring(0, 2);


            using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_LogicalDisk WHERE DeviceID='" + systemLogicalDiskDeviceId + "'"))
            {
                // ReSharper disable once PossibleInvalidCastExceptionInForeachLoop
                foreach (ManagementObject logicalDisk in searcher.Get())
                    // ReSharper disable once PossibleInvalidCastExceptionInForeachLoop
                    foreach (ManagementObject partition in logicalDisk.GetRelated("Win32_DiskPartition"))
                        // ReSharper disable once PossibleInvalidCastExceptionInForeachLoop
                        foreach (ManagementObject diskDrive in partition.GetRelated("Win32_DiskDrive"))
                            // ReSharper disable once PossibleInvalidCastExceptionInForeachLoop
                            foreach (ManagementObject diskMedia in diskDrive.GetRelated("Win32_PhysicalMedia"))
                                serialNumber = diskMedia["SerialNumber"].ToString();
            }

            return serialNumber.Trim();
        }
        //Motherboard ID
        private static string BaseId()
        {
            return Identifier("Win32_BaseBoard", "Model")
            + Identifier("Win32_BaseBoard", "Manufacturer")
            + Identifier("Win32_BaseBoard", "Name")
            + Identifier("Win32_BaseBoard", "SerialNumber");
        }
        //Primary video controller ID
        private static string VideoId()
        {
            return Identifier("Win32_VideoController", "DriverVersion")
            + Identifier("Win32_VideoController", "Name");
        }

        #endregion
    }
}