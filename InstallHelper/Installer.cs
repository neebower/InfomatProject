using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;

namespace InstallHelper
{
    [RunInstaller(true)]
    public partial class Installer : System.Configuration.Install.Installer
    {
        public Installer()
        {
            InitializeComponent();
        }
        private static void GrantAccess(string file)
        {
            bool exists = Directory.Exists(file);
            if (!exists)
            {
                Directory.CreateDirectory(file);
                //Console.WriteLine("The Folder is created Sucessfully");
            }
            //else
            //{
            //    Console.WriteLine("The Folder already exists");
            //}
            DirectoryInfo dInfo = new DirectoryInfo(file);
            DirectorySecurity dSecurity = dInfo.GetAccessControl();
            dSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
            dInfo.SetAccessControl(dSecurity);

        }
        public override void Install(IDictionary stateSaver)
        {
            // This gets the named parameters passed in from your custom action
            string folder = Context.Parameters["folder"];
            //CustomDataProperty  /folder="[CommonAppDataFolder][ProductName]"

            //// This gets the "Authenticated Users" group, no matter what it's called
            //SecurityIdentifier sid = new SecurityIdentifier(WellKnownSidType.AuthenticatedUserSid, null);

            //// Create the rules
            //FileSystemAccessRule writerule = new FileSystemAccessRule(sid, FileSystemRights.Write, AccessControlType.Allow);

            //if (!string.IsNullOrEmpty(folder) && Directory.Exists(folder))
            //{
            //    // Get your file's ACL
            //    DirectorySecurity fsecurity = Directory.GetAccessControl(folder);

            //    // Add the new rule to the ACL
            //    fsecurity.AddAccessRule(writerule);

            //    // Set the ACL back to the file
            //    Directory.SetAccessControl(folder, fsecurity);
            //}

            //Permissions.GrantAccess(Context.Parameters["folder"]);
            // Explicitly call the overriden method to properly return control to the installer
            
            GrantAccess(folder);
            base.Install(stateSaver);
        }
    }
}
