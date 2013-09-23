using System;
using System.Reflection;
using System.Resources;
using System.Windows;

namespace WindowLock {
    /// <summary>
    /// Jeffrey Richter: Excerpt #2 from CLR via C#, Third Edition:
    /// <list type="">
    /// <item>add DLLs to project as Embedded Resources (Properties / Advanced / Build Action = Embedded Resource)</item>
    /// <item>to find the dependent DLL assemblies, when your application initializes, register a callback method with the AppDomain’s ResolveAssembly event</item>
    /// <item>the first time a thread calls a method that references a type in a dependent DLL file, the AssemblyResolve event will be raised and
    /// the callback code shown above will find the embedded DLL resource desired and load it by calling an overload of Assembly’s Load method
    /// that takes a Byte[] as an argument.</item>
    /// </list>
    /// </summary>
    /// <seealso cref="http://blogs.msdn.com/b/microsoft_press/archive/2010/02/03/jeffrey-richter-excerpt-2-from-clr-via-c-third-edition.aspx"/>
    public class EmbeddedAssemblyResolver {

        /// <summary>
        /// Registers the embedded assembly resolver in Release mode only.<br/>
        /// Must be called from application's constructor.
        /// </summary>
        public static void RegisterResolver() {
#if (!DEBUG)
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(ResolveEmbeddedAssembly);
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        /// <seealso cref="http://blogs.msdn.com/b/microsoft_press/archive/2010/02/03/jeffrey-richter-excerpt-2-from-clr-via-c-third-edition.aspx"/>
        private static Assembly ResolveEmbeddedAssembly(object sender, ResolveEventArgs args) {
            String resourceName = Application.Current.GetType().Namespace + "." + new AssemblyName(args.Name).Name + ".dll";
            using(var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName)) {
                byte[] assemblyData = new byte[stream.Length];
                stream.Read(assemblyData, 0, assemblyData.Length);
                return Assembly.Load(assemblyData);
            }
        }

        /// TODO check this 2nd option
        /// <seealso cref="http://codeblog.larsholm.net/2011/06/embed-dlls-easily-in-a-net-assembly/"/>
        private static Assembly ResolveEmbeddedAssembly2(object sender, ResolveEventArgs args) {
            string dllName = args.Name.Contains(",") ? args.Name.Substring(0, args.Name.IndexOf(',')) : args.Name.Replace(".dll", "");
            dllName = dllName.Replace('.', '_');
            if(dllName.EndsWith("_resources")) {
                return null;
            }
            ResourceManager rm = new ResourceManager(Application.Current.GetType().Namespace + ".Properties.Resources", Assembly.GetExecutingAssembly());
            byte[] assemblyData = (byte[])rm.GetObject(dllName);
            return Assembly.Load(assemblyData);
        }
    }
}
