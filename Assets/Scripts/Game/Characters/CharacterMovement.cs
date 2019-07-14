namespace pdxpartyparrot.Game.Characters
{
    public interface ICharacterMovement
    {
        bool IsComponentControlling { get; set; }

        void Jump(float height);

        void EnableDynamicCollisionDetection(bool enable);
    }
}
