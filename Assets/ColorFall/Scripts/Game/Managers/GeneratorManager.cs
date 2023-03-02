using ColorFall.Core;
using UnityEngine;

namespace ColorFall.Game
{
    public class GeneratorManager : MonoBehaviour, IGameManager
    {
        public ManagerStatus Status { get; private set; }

        public void Startup()
        {
            Debug.Log("Generator manager starting...");

            Status = ManagerStatus.Started;
        }
    }
}
