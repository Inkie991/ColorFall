using ColorFall.Core;

namespace ColorFall.Game
{
    public interface IGameManager
    {
        public ManagerStatus Status { get; }

        void Startup();
    }
}