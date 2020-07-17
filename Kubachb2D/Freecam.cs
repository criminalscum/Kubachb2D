using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.System;
using SFML.Graphics;
using SFML.Window;

namespace Kubachb2D
{
    class Freecam : Transformable
    {
        public float MOVE_SPEED = 256f;
        public Freecam(Vector2f Pos)
        {
            Position = Pos;
        }
        public void Update(Time elapsed)
        {
            bool lkey = Keyboard.IsKeyPressed(Keyboard.Key.A);
            bool rkey = Keyboard.IsKeyPressed(Keyboard.Key.D);
            bool ukey = Keyboard.IsKeyPressed(Keyboard.Key.W);
            bool dkey = Keyboard.IsKeyPressed(Keyboard.Key.S);
            if (lkey)
                Position = new Vector2f(Position.X-MOVE_SPEED*elapsed.AsSeconds(), Position.Y);
            if (rkey)
                Position = new Vector2f(Position.X + MOVE_SPEED * elapsed.AsSeconds(), Position.Y);
            if (ukey)
                Position = new Vector2f(Position.X, Position.Y-MOVE_SPEED*elapsed.AsSeconds());
            if (dkey)
                Position = new Vector2f(Position.X, Position.Y+MOVE_SPEED*elapsed.AsSeconds());

        }
    }
}
