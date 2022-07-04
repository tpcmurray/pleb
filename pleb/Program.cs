using System;

namespace pleb
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new PlebGame())
                game.Run();
        }
    }
}
