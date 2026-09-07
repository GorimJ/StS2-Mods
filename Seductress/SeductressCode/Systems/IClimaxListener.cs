using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace Seductress.SeductressCode.Systems;

/// <summary>
/// Implemented by powers that trigger "whenever you Climax" (Afterglow, Rapture).
///
/// There is no vanilla hook for this — Climax is our own event — so
/// <see cref="ClimaxModel"/> walks the owner's powers and notifies anything implementing this.
/// </summary>
public interface IClimaxListener
{
    Task OnClimax(Creature owner);
}
