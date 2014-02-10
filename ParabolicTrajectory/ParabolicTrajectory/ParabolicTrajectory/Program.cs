using System;

namespace ParabolicTrajectory
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (TrajectoryGame game = new TrajectoryGame())
            {
                game.Run();
            }
        }
    }
#endif
}

