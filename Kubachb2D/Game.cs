using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Kubachb2D
{
    class Game
    {
        public Level level;
        public Player player;
        public Freecam camera;
        public bool displayFPS = true;
        public bool displayRenderPerFrame = true;
        public bool freecam = false;
        private Text FPS;
        private Text RenderPerFrame;
        private Text RenderPerSecond;
        private int Frames;

        public Game(Level lv)
        {
            FPS = new Text("", Content.GameFont, 24);
            RenderPerFrame = new Text("", Content.GameFont, 24);
            RenderPerSecond = new Text("", Content.GameFont, 24);
            player = new Player(new Vector2f(100,-800));
            camera = new Freecam(new Vector2f());
            level = lv;
        }
        public void Update(Time time)
        {
            if (freecam)
                camera.Update(time);
            player.Update(time);
            View v = Program.Window.GetView();
            if (!freecam)
                v.Center = player.Position;
            else
                v.Center = camera.Position;
            Program.Window.SetView(v);
            Frames = (int)Math.Round((1f / time.AsSeconds()));
            if (displayFPS)
            {
                FPS.DisplayedString = Frames.ToString() + " FPS";
                FPS.Position = v.Center + new Vector2f(Program.WIDTH / 2.5f, Program.HEIGHT / 2.5f-30f);
            }
            if (displayRenderPerFrame)
            {
                RenderPerFrame.DisplayedString = level.RenderPerFrame.ToString() + " RPF";
                RenderPerFrame.Position = new Vector2f(FPS.Position.X, FPS.Position.Y + 30f);
                RenderPerSecond.Position = new Vector2f(RenderPerFrame.Position.X, RenderPerFrame.Position.Y + 30f);
                RenderPerSecond.DisplayedString = (level.RenderPerFrame * Frames).ToString() + " RPS";
            }
        }
        public void Draw()
        {
            Program.Window.Draw(level);
            Program.Window.Draw(player);
            if (displayFPS)
                Program.Window.Draw(FPS);
            if (displayRenderPerFrame)
            {
                Program.Window.Draw(RenderPerFrame);
                Program.Window.Draw(RenderPerSecond);
            }
        }
    }
}
