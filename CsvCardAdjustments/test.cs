using System;
using System.Reflection;
class TestClass {
    public bool IsMutable { get; private set; }
    public TestClass() { IsMutable = false; }
}
class Program {
    static void Main() {
        var obj = new TestClass();
        var prop = typeof(TestClass).GetProperty("IsMutable", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (prop == null) {
            Console.WriteLine("Prop is null");
            return;
        }
        try {
            prop.SetValue(obj, true);
            Console.WriteLine("SetValue success: " + obj.IsMutable);
        } catch (Exception e) {
            Console.WriteLine("SetValue failed: " + e.Message);
        }
    }
}
