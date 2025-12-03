namespace _Project.Scripts.Infrastructure.Animation
{
    public interface IAnimationStateReader
    {
        void EnteredState(int stateHash);
        void ExitedState(int stateHash);
        void UpdateState(int stateHash);
    }
}