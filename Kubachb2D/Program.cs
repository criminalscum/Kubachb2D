using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.Window;
using SFML.System;
using System.Collections.Generic;

namespace Kubachb2D
{
    class Program
    {
        static RenderWindow win;
        public static RenderWindow Window { get { return win; } }
        public static uint WIDTH = 1280;
        public static uint HEIGHT = 720;
        public static State state;
        public static Game game;
        public static Random rand= new Random();
        static void Main(string[] args)
        {
            win = new RenderWindow(new VideoMode(WIDTH, HEIGHT), "Kubachb 2D", Styles.Close);
            win.SetVerticalSyncEnabled(true);
            win.Closed += Win_closed;
            win.KeyReleased += Win_KeyReleased;
            Content.Load();
            Clock clock = new Clock();
            clock.Restart();
            state = State.Game;
            //Level l = new Level(new Tile[100,60]);
            Level l = new Level(Level.Generate(500));
            game = new Game(l);
            while (win.IsOpen)
            {
                win.DispatchEvents();
                win.Clear(Color.Black);
                switch (state)
                {
                    case State.Game:
                        for (int i = 0; i < 1; i++)
                        {
                            Time elapsed = clock.Restart();
                            game.Update(elapsed);
                        }
                        game.Draw();
                        break;
                    default:
                        break;
                }
                win.Display();
            }
        }

        private static void Win_KeyReleased(object sender, KeyEventArgs e)
        {
            switch (e.Code)
            {
                case Keyboard.Key.P:
                    game.freecam = !game.freecam;
                    if(game.freecam)
                        game.camera = new Freecam(game.player.Position);
                    break;
            }
        }

        private static void Win_closed(object sender, EventArgs e)
        {
            win.Close();
            Environment.Exit(0);
        }
        public static float[] Noise(int seed, int scale ,int width, int ip, int offset)
        {
            rand = new Random(seed);
            float[] noise = new float[width/ip];
            for (int i = 0; i < noise.Length; i++)
                noise[i] = rand.Next(0, scale);
            float[] noises = new float[width];
            for (int i = 0; i < width/ip-1; i++)
            {
                noises[i * ip] = noise[i];
                for (int x = 0; x < ip; x++)
                {
                    noises[i * ip + x] = Interpolate(noise[i], noise[i + 1], (float)x / ip)+offset;
                }
            }
                
            return noises;
        }
        public static float Interpolate(float p1, float p2, float fraction)
        {
            return p1 + (p2 - p1) * fraction;
        }

    }

    enum State
    {
        MainMenu,
        Game,
        SettingsMenu
    }
}
