using System;
using System.Reflection;
using System.Linq;

class Program { 
    static void Main() { 
        var sts2Core = Assembly.LoadFrom(@"A:\SteamLibrary\steamapps\common\Slay the Spire 2\data_sts2_windows_x86_64\sts2.dll");
        
        var dvsType = sts2Core.GetTypes().FirstOrDefault(t => t.Name == "DynamicVarSet");
        if (dvsType != null) {
            foreach(var m in dvsType.GetMethods(BindingFlags.Public | BindingFlags.Instance)) {
                Console.WriteLine("DVS M: " + m.ReturnType.Name + " " + m.Name + " " + string.Join(",", m.GetParameters().Select(p => p.ParameterType.Name)));
            }
        }
        
        var ivType = sts2Core.GetTypes().FirstOrDefault(t => t.Name == "IntVar");
        if (ivType != null) {
            foreach(var m in ivType.GetMethods(BindingFlags.Public | BindingFlags.Instance)) {
                if (m.Name.Contains("Add") || m.Name.Contains("Diff") || m.Name.Contains("Set"))
                Console.WriteLine("IV M: " + m.ReturnType.Name + " " + m.Name + " " + string.Join(",", m.GetParameters().Select(p => p.ParameterType.Name)));
            }
            foreach(var p in ivType.GetProperties(BindingFlags.Public | BindingFlags.Instance)) {
                Console.WriteLine("IV P: " + p.PropertyType.Name + " " + p.Name);
            }
        }
    } 
}
