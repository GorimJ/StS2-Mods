using System;
using MegaCrit.Sts2.Core.Models;

public class TestClass {
    public void TestMethod() {
        Console.WriteLine(typeof(RelicRarity).FullName);
        Console.WriteLine(typeof(CharacterGender).FullName);
        Console.WriteLine(typeof(LocString).FullName);
        Console.WriteLine(typeof(PowerModel.StackType).FullName);
        Console.WriteLine(typeof(PowerModel.Type).FullName);
    }
}
