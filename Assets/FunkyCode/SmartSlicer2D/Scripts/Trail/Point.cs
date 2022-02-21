using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities2D;

namespace Slicer2D.Trail {

    public class Point {
        public Vector2D position;
        public float time = 1;
        public TimerHelper timer;

        public Point(Vector2D vector, float _time) {
            position = vector;
            time = _time;
            timer = TimerHelper.Create();
        }

        public bool Update() {
            if (timer.Get() >= time) {
                return(false);
            } else {
                return(true);
            }
        }
    }
}