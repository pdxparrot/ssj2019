using pdxpartyparrot.Core.Actors;

namespace pdxpartyparrot.Game.Actors
{
    public interface IDamagable
    {
        void Damage(Actor source, string type, int amount);
    }
}
