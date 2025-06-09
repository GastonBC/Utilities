using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Utilities
{
    public static partial class Utils
    {
        /// <summary>
        /// Invoke a dll to memmory and then close it. Used to execute the addin. Pass the
        /// first 3 arguments from the calling class
        /// </summary>
        /// <param name="commandData">ExternalCommandData</param>
        /// <param name="message">string</param>
        /// <param name="elements">ElementSet</param>
        /// <param name="AssemblyPath">Path to dll</param>
        /// <param name="InvokePosition">INVOKE XX</param>
        /// <param name="CommandName">Optional. Should be ThisApplication always</param>
        /// <returns></returns>
        public static Result InvokeCmd(ExternalCommandData commandData, ref string message, ElementSet elements,
                                         string AssemblyPath, string InvokePosition, string CommandName = "ThisApplication")
        {
            try
            {
                string assemblyPath = AssemblyPath; // Addin dll path

                byte[] assemblyBytes = File.ReadAllBytes(assemblyPath); // Read bytes to memory (from docstring: "[...] then closes the file")
                Assembly objAssembly01 = Assembly.Load(assemblyBytes); // Load assembly

                string strCommandName = CommandName; // Class where the command or app reside

                IEnumerable<Type> myIEnumerableType = GetTypesSafely(objAssembly01);
                foreach (Type objType in myIEnumerableType)
                {
                    if (objType.IsClass)
                    {
                        if (objType.Name.ToLower() == strCommandName.ToLower())
                        {
                            object ibaseObject = Activator.CreateInstance(objType);
                            object[] arguments = new object[] { commandData, message, elements };
                            object result = null;

                            result = objType.InvokeMember("Execute", BindingFlags.Default | BindingFlags.InvokeMethod, null, ibaseObject, arguments);

                            break;
                        }
                    }
                }
                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                Utils.CatchDialog(ex, InvokePosition);
                return Result.Failed;
            }
        }

        public static IEnumerable<Type> GetTypesSafely(Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                return ex.Types.Where(x => x != null);
            }
        }
    }
}
