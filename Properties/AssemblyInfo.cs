using System.Reflection;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Azavea.Open.DAO")]
[assembly: AssemblyDescription("The core of FastDAO, an object-relational mapper for the .NET framework")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyProduct("FastDAO")]

// Configure log4net using the .config file
[assembly: log4net.Config.XmlConfigurator(Watch = true)]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("38e3bb65-9bb6-4aea-a562-e448ec3afaab")]