using HlyssMG;
using SnowballSpin.Scenes;
using System;

namespace SnowballSpin
{
    public static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            using (var game = new GameBase(new ConfigScn()))
            {
                game.Run();
            }
        }
    }
}
