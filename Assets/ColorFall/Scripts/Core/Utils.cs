using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ColorFall.Core
{
    public static class Utils
    {
        // THIS STUPID METHOD CREATED BY LAZY DEV FOR LAZY DEVS
        public static Vector3 SetCoordinate(Vector3 position, float value, string key = "y")
        {
            switch (key)
            {
                case "x":
                    position.x = value;
                    return position;
                case "z":
                    position.z = value;
                    return position;
                default:
                    position.y = value;
                    return position;
            }
        }

        public static Quaternion SetCoordinate(Quaternion rotation, float value, string key = "y")
        {
            switch (key)
            {
                case "x":
                    rotation.x = value;
                    return rotation;
                case "z":
                    rotation.z = value;
                    return rotation;
                default:
                    rotation.y = value;
                    return rotation;
            }
        }

        public static T GetRandomItem<T>(List<T> list)
        {
            int i = (int) Random.Range(0, list.Count);
            return list[i];
        }
        
        public static void Invoke(this MonoBehaviour mb, Action f, float delay)
        {
            mb.StartCoroutine(InvokeRoutine(f, delay));
        }
 
        private static IEnumerator InvokeRoutine(System.Action f, float delay)
        {
            yield return new WaitForSeconds(delay);
            f();
        }
    }
}
