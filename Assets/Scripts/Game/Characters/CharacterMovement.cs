namespace pdxpartyparrot.Game.Characters
{
    public interface ICharacterMovement
    {
        bool IsKinematic { get; set; }

        void Jump(float height);

        void EnableDynamicCollisionDetection(bool enable);
    }
}
