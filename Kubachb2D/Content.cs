using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;

namespace Kubachb2D
{
    class Content
    {
        public const string CONTENT_DIR = "Content\\";
        public static Texture Blocks;
        public static Texture Player;
        public static Font GameFont;

        public static void Load()
        {
            Blocks = new Texture(CONTENT_DIR+"blocks.png");
            Player = new Texture(CONTENT_DIR+"player.png");
            GameFont = new Font(CONTENT_DIR+"Arial.ttf");
        }
    }
}
