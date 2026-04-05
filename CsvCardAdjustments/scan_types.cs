using System;
using System.Reflection;
using System.Linq;

namespace TestNamespace {
    public class ReflectionTest {
        public static void Main() {
            try {
                var asm = Assembly.LoadFrom(@"A:\SteamLibrary\steamapps\common\Slay the Spire 2\data_sts2_windows_x86_64\sts2.dll");
                foreach (var type in asm.GetTypes()) {
                    if (type.Name.Contains("Feed", StringComparison.OrdinalIgnoreCase) ||
                        type.Name.Contains("Hunt", StringComparison.OrdinalIgnoreCase) ||
                        type.Name.Contains("Hand", StringComparison.OrdinalIgnoreCase)) 
                    {
                        Console.WriteLine($"Found: {type.FullName}");
                    }
                }
            } catch (Exception e) {
                Console.WriteLine(e);
            }
        }
    }
}
