using Unity.Mathematics;
using static Unity.Mathematics.math;

namespace Nebukam.Cluster
{
    public static class Sockets
    {

        // Sides

        public const int SIDES = 6;

        public const int UP = 0;
        public const int DOWN = 1;
        public const int LEFT = 2;
        public const int RIGHT = 3;
        public const int FRONT = 4;
        public const int BACK = 5;

        // Corners

        public const int CORNERS = 14;

        public const int FRONT_UP_LEFT = 6;
        public const int FRONT_UP_RIGHT = 7;
        public const int FRONT_DOWN_LEFT = 8;
        public const int FRONT_DOWN_RIGHT = 9;
        public const int BACK_UP_LEFT = 10;
        public const int BACK_UP_RIGHT = 11;
        public const int BACK_DOWN_LEFT = 12;
        public const int BACK_DOWN_RIGHT = 13;

        public static int[] MIRROR = new int[] {
            DOWN,
            UP,
            RIGHT,
            LEFT,
            BACK,
            FRONT,
            //TODO : opposites...
            FRONT_UP_LEFT,
            FRONT_UP_RIGHT,
            FRONT_DOWN_LEFT,
            FRONT_DOWN_RIGHT,
            BACK_UP_LEFT,
            BACK_UP_RIGHT,
            BACK_DOWN_LEFT,
            BACK_DOWN_RIGHT
        };

        // Coords in cluster space

        public static int3 O_UP = int3(0,0,1);
        public static int3 O_DOWN = int3(0, 0, -1);
        public static int3 O_LEFT = int3(-1, 0, 0);
        public static int3 O_RIGHT = int3(1, 0, 0);
        public static int3 O_FRONT = int3(0, -1, 0);
        public static int3 O_BACK = int3(0, 1, 0);

        public static int3 O_FRONT_UP_LEFT = O_FRONT + O_UP + O_LEFT;
        public static int3 O_FRONT_UP_RIGHT = O_FRONT + O_UP + O_RIGHT;
        public static int3 O_FRONT_DOWN_LEFT = O_FRONT + O_DOWN + O_LEFT;
        public static int3 O_FRONT_DOWN_RIGHT = O_FRONT + O_DOWN + O_RIGHT;
        public static int3 O_BACK_UP_LEFT = O_BACK + O_UP + O_LEFT;
        public static int3 O_BACK_UP_RIGHT = O_BACK + O_UP + O_RIGHT;
        public static int3 O_BACK_DOWN_LEFT = O_BACK + O_DOWN + O_LEFT;
        public static int3 O_BACK_DOWN_RIGHT = O_BACK + O_DOWN + O_RIGHT;

        public static int3[] OFFSETS = new int3[] {
            O_UP,
            O_DOWN,
            O_LEFT,
            O_RIGHT,
            O_FRONT,
            O_BACK,
            O_FRONT_UP_LEFT,
            O_FRONT_UP_RIGHT,
            O_FRONT_DOWN_LEFT,
            O_FRONT_DOWN_RIGHT,
            O_BACK_UP_LEFT,
            O_BACK_UP_RIGHT,
            O_BACK_DOWN_LEFT,
            O_BACK_DOWN_RIGHT
        };

        public static int3[] OFFSET_MIRROR = new int3[] {
            O_UP * -1,
            O_DOWN* -1,
            O_LEFT* -1,
            O_RIGHT* -1,
            O_FRONT* -1,
            O_BACK* -1,
            O_FRONT_UP_LEFT* -1,
            O_FRONT_UP_RIGHT* -1,
            O_FRONT_DOWN_LEFT* -1,
            O_FRONT_DOWN_RIGHT* -1,
            O_BACK_UP_LEFT* -1,
            O_BACK_UP_RIGHT* -1,
            O_BACK_DOWN_LEFT* -1,
            O_BACK_DOWN_RIGHT* -1
        };


    }
}
