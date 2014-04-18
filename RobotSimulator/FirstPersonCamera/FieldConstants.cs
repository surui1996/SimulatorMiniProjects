using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RobotSimulator
{
    class FieldConstants
    {
        public static float CAMERA_RATIO = 640f / 480f;

        //in feet
        public static float WIDTH = 24f + (8f / 12f); 
        public static float HEIGHT = 54f;
        public static float RATIOZX = HEIGHT / WIDTH;
        public static float PICTURE_RATIO = 961f / 351f;
        public static float HIGHGOAL_HEIGHT = 3f + (1f / 12f);
        private static float high_goal_bottom_above_carpet = 6f + (10.75f / 12f);
        public static float HEIGHT_ABOVE_CARPET = high_goal_bottom_above_carpet + HIGHGOAL_HEIGHT;
        public static float RATIOYX = WIDTH / HEIGHT_ABOVE_CARPET;
        //public static float LOWGOAL_HEIGHT_ABOVE_CARPET = 7f / 12f; // 7 in.
        //public static float LOWGOAL_HEIGHT = 2f + 4f / 12f; //2 ft. 4 in.
        public static float LOWGOAL_WIDTH = 2f + 5f / 12f; //2 ft. 5 in.

        public static float DYNAMIC_HEIGHT_ABOVE_CARPET = 5f + (8f / 12f);  //5 ft. 8 in.
        public static float DYNAMIC_WIDTH = 1f + (11.5f / 12f); //1ft. 11.5 in.
        public static float DYNAMIC_HEIGHT = 4f / 12f; // 4 in.
        public static float STATIC_HEIGHT_ABOVE_CARPET = 3f + (1.5f / 12f); //3 ft. 1 ½ in.
        public static float STATIC_WIDTH = 4f / 12f; // 4 in.
        public static float STATIC_HEIGHT = 2f + (8f / 12f); // 2 ft. 8 in.
        public static float STATIC_BLACK_STRIPES_WIDTH = 2f / 12f; //2 in.

        public static float C = 10f;

        
        
    }
}
