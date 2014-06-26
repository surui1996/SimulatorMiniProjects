using System;

namespace Simulator.Main
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (Simulator game = new Simulator())
            {
                game.Run();
            }
        }
    }
#endif
}

