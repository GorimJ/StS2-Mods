using System;
using System.IO;
using System.Linq;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Characters;
using MegaCrit.Sts2.Core.Entities.Relics;

class Program
{
    static void Main()
    {
        MegaCrit.Sts2.Core.Helpers.OneTimeInitialization.ExecuteEssential();
        // Force model DB initialization (if not done by ExecuteEssential)
        
        using (StreamWriter writer = new StreamWriter("class_relic_dump.txt"))
        {
            var characters = new CharacterModel[] {
                new IroncladModel(),
                new SilentModel(),
                new DefectModel(),
                new NecrobinderModel()
            };

            foreach (var character in characters)
            {
                writer.WriteLine($"=== {character.Title} ===");
                var pool = character.RelicPool.AllRelics;
                
                var commons = pool.Where(r => r.Rarity == RelicRarity.Common).Select(r => r.Id).ToList();
                writer.WriteLine("Commons:");
                foreach(var c in commons) writer.WriteLine(" - " + c);

                var uncommons = pool.Where(r => r.Rarity == RelicRarity.Uncommon).Select(r => r.Id).ToList();
                writer.WriteLine("Uncommons:");
                foreach(var u in uncommons) writer.WriteLine(" - " + u);

                var rares = pool.Where(r => r.Rarity == RelicRarity.Rare).Select(r => r.Id).ToList();
                writer.WriteLine("Rares:");
                foreach(var r in rares) writer.WriteLine(" - " + r);
                writer.WriteLine();
            }
        }
    }
}
