using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Slicer2D {

    public class Debug {
        static public bool enabled = true;

        public static void Log(string message) {
            UnityEngine.Debug.Log(message);
        }

        public static void LogWarning(string message) {
            UnityEngine.Debug.LogWarning(message);
        }

    }
}