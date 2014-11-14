using System;

namespace TankGame
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (TankGame tankGame = new TankGame())
            {
                tankGame.Run();
            }
        }
    }
#endif
}

