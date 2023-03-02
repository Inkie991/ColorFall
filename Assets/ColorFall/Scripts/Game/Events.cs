using ColorFall.Core;
using UnityEngine;

namespace ColorFall.Game
{
    public class GameEvent
    {
    }
    
    public static class Events
    {
        public static PlayerFinishEvent PlayerFinishEvent = new();
        public static PlayerWinEvent PlayerWinEvent = new();
        public static PlayerLoseEvent PlayerLoseEvent = new();
        public static CollectDropEvent CollectDropEvent = new();
        public static ChargedModeOnEvent ChargedModeOnEvent = new();
        public static ChargedModeOffEvent ChargedModeOffEvent = new();
        public static GameStartedEvent GameStartedEvent = new();
        public static SecondChanceEvent SecondChanceEvent = new();
        public static CollectMoneyEvent CollectMoneyEvent = new();
        public static ComboEvent ComboEvent = new();
        public static CannonEvent CannonEvent = new();
        public static BoostEvent BoostEvent = new();
        public static SetBoostPowerEvent SetBoostPower = new();
    }

    public class SetBoostPowerEvent : GameEvent
    {
        public float power;
    }

    public class CollectMoneyEvent : GameEvent
    {
        public Vector3 screenPos;
    }

    public class PlayerFinishEvent : GameEvent
    {
        public float endSpeed;
    }

    public class PlayerWinEvent : GameEvent
    {
        public float multiplier;
    }

    public class CannonEvent : GameEvent
    {
    }

    public class BoostEvent : GameEvent
    {
        public float power;
    }

    public class PlayerLoseEvent : GameEvent
    {
    }

    public class SecondChanceEvent : GameEvent
    {
    }

    public class BouncedOffPlatformEvent : GameEvent
    {
        public float posY;
    }
    
    public class CollectDropEvent : GameEvent
    {
        public CollectedDrop collectedDrop;
        public Vector3 screenPos; 
    }

    public class ChargedModeOnEvent : GameEvent
    {
    }

    public class ChargedModeOffEvent : GameEvent
    {
    }
    
    public class GameStartedEvent : GameEvent
    {
    }

    public class ComboEvent : GameEvent
    {
        public int messageModifier;
    }
}
