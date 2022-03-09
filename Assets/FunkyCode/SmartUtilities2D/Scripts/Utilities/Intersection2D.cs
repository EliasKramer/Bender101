namespace Utilities2D {

    public struct Intersection2D {
        public double x, y;
        public bool state;

        public Intersection2D(bool astate, double ax, double ay) {
            x = ax;
            y = ay;
            state = astate;
        }
    }

}