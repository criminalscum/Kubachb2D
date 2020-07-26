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
    class Player : Transformable, Drawable
    {
        public RectangleShape leftArm;
        public RectangleShape rightArm;
        public RectangleShape leftLeg;
        public RectangleShape rightLeg;
        public RectangleShape body;
        public RectangleShape head;

        public RectangleShape crosshair;
        public RectangleShape debugBlock;

        public const int HEAD_SIZE = Tile.TILE_SIZE/2;
        public const int BODY_SIZE = 3*Tile.TILE_SIZE/4;
        public const float HORIZONTAL_ACCELERATION = 512f;
        public const float HORIZONTAL_MOVESPEED = 256f;
        public const float GRAVITY = 256f;
        public Vector2f Velocity;
        public bool flying = true;

        private float animtime;
        private float breaktime;
        private Vector2i oldbCursor;

        public Player(Vector2f pos)
        {
            crosshair = new RectangleShape(new Vector2f(Tile.TILE_SIZE, Tile.TILE_SIZE));
            crosshair.Texture = Content.BreakAnim;
            crosshair.TextureRect = new IntRect(0,0,16,16);
            crosshair.FillColor = Color.White;
            debugBlock = new RectangleShape(new Vector2f(Tile.TILE_SIZE, Tile.TILE_SIZE));
            debugBlock.FillColor = Color.Transparent;
            debugBlock.OutlineThickness = crosshair.OutlineThickness = 1;
            debugBlock.OutlineColor = Color.Red;
            crosshair.OutlineColor = Color.White;

            breaktime = 0f;
            animtime = 0f;

            Position = pos;
            head = new RectangleShape(new Vector2f(HEAD_SIZE, HEAD_SIZE));
            leftArm = new RectangleShape(new Vector2f(0.5f*HEAD_SIZE, BODY_SIZE));
            rightArm = new RectangleShape(new Vector2f(0.5f * HEAD_SIZE, BODY_SIZE));
            rightLeg = new RectangleShape(new Vector2f(0.5f * HEAD_SIZE, BODY_SIZE));
            leftLeg = new RectangleShape(new Vector2f(0.5f * HEAD_SIZE, BODY_SIZE));
            body = new RectangleShape(new Vector2f(0.5f * HEAD_SIZE, BODY_SIZE));
            leftArm.Texture = Content.Player;
            rightArm.Texture = Content.Player;
            leftLeg.Texture = Content.Player;
            rightLeg.Texture = Content.Player;
            body.Texture = Content.Player;
            head.Texture = Content.Player;
            head.TextureRect = new IntRect(25,0,8,8);
            leftArm.TextureRect = new IntRect(0,0,4,12);
            rightArm.TextureRect = new IntRect(5,0,4,12);
            rightLeg.TextureRect = new IntRect(10,0,4,12);
            leftLeg.TextureRect = new IntRect(15,0,4,12);
            body.TextureRect = new IntRect(20,0,4,12);
            head.Origin = new Vector2f(HEAD_SIZE/2, HEAD_SIZE);
            body.Origin = new Vector2f(0.25f*HEAD_SIZE,BODY_SIZE/2);
            leftArm.Origin = new Vector2f(0.25f*HEAD_SIZE,0);
            rightArm.Origin = new Vector2f(0.25f * HEAD_SIZE, 0);
            rightLeg.Origin = new Vector2f(0.25f * HEAD_SIZE, 0);
            leftLeg.Origin = new Vector2f(0.25f * HEAD_SIZE, 0);
        }

        public void Update(Time elapsed)
        {
            Position += Velocity * elapsed.AsSeconds();
            body.Position = Position;
            head.Position = Position - new Vector2f(0,0.5f*BODY_SIZE);
            leftArm.Position = Position - new Vector2f(0,0.375f*Tile.TILE_SIZE);
            rightArm.Position = Position - new Vector2f(0,0.375f*Tile.TILE_SIZE);
            rightLeg.Position = Position - new Vector2f(0, -0.375f*Tile.TILE_SIZE);
            leftLeg.Position = Position - new Vector2f(0,-0.375f*Tile.TILE_SIZE);

            bool lkeypressed = Mouse.IsButtonPressed(Mouse.Button.Left) && !Program.game.freecam;
            bool dkey = Keyboard.IsKeyPressed(Keyboard.Key.D)&&!Program.game.freecam;
            bool akey = Keyboard.IsKeyPressed(Keyboard.Key.A) && !Program.game.freecam;
            bool spacekey = Keyboard.IsKeyPressed(Keyboard.Key.Space) && !Program.game.freecam;

            Vector2i mousepos = Mouse.GetPosition(Program.Window);
            if (!Program.game.freecam)
                head.Rotation = 50f * Convert.ToSingle(Math.Atan((mousepos.Y - (0.5 * Program.HEIGHT) + 60) / (mousepos.X - (0.5f * Program.WIDTH))));

            if (lkeypressed)
            {
                breaktime += elapsed.AsSeconds();
                if (mousepos.X - (0.5f * Program.WIDTH) < 0)
                {
                    if (rightArm.Rotation < 30f)
                        rightArm.Rotation = 60f;
                    else
                        rightArm.Rotation -= 120 * elapsed.AsSeconds();
                }
                else
                {
                    if (rightArm.Rotation > -30f)
                        rightArm.Rotation = -60f;
                    else
                        rightArm.Rotation += 120 * elapsed.AsSeconds();
                }
            }
            else
            {
                breaktime = 0f;
                rightArm.Rotation = 0f;
            }

            int blockOnX = (int)((Position.X / Tile.TILE_SIZE));
            int blockOnY = (int)Math.Ceiling((Position.Y + 1.125f * Tile.TILE_SIZE) / Tile.TILE_SIZE);

            Tile myTile = Program.game.level.GetTileWithCoords(blockOnX, blockOnY);
            Tile l1 = Program.game.level.GetTileWithCoords(blockOnX-1, blockOnY-1);
            Tile l2 = Program.game.level.GetTileWithCoords(blockOnX - 1, blockOnY - 2);
            Tile r1 = Program.game.level.GetTileWithCoords(blockOnX + 1, blockOnY - 1);
            Tile r2 = Program.game.level.GetTileWithCoords(blockOnX + 1, blockOnY - 2);

            bool r1b = (r1 != null) && r1.amICollidable();
            bool r2b = (r2 != null) && r2.amICollidable();
            bool l1b = (l1 != null) && l1.amICollidable();
            bool l2b = (l2 != null) && l2.amICollidable();

            Vector2f vcenter = Program.Window.GetView().Center;
            Vector2f cursorPos = vcenter + new Vector2f(mousepos.X-(0.5f*Program.WIDTH), mousepos.Y-(0.5f*Program.HEIGHT));
            Vector2i bCursor = new Vector2i((int)(cursorPos.X/Tile.TILE_SIZE), (int)(cursorPos.Y/Tile.TILE_SIZE));
            crosshair.Position = new Vector2f(bCursor.X*Tile.TILE_SIZE, bCursor.Y*Tile.TILE_SIZE);
            if (bCursor != oldbCursor)
                breaktime = 0f;
            Tile inTile = Program.game.level.GetTileWithCoords(blockOnX, blockOnY-1);
            Tile bTile = Program.game.level.GetTileWithCoords(bCursor.X, bCursor.Y);
            oldbCursor = bCursor;
            /*if(bTile != null)
                Console.WriteLine(bTile.type);*/
            if(bTile != null)
            {
                float blockbrtime = 0f;
                bool unb = false;
                switch (Tile.binstr[bTile.type])
                {
                    case ToolType.NONE:
                        blockbrtime = 0.35f;
                        break;
                    case ToolType.SHOVEL:
                        blockbrtime = 0.75f;
                        break;
                    case ToolType.WOODEN_PICKAXE:
                        blockbrtime = 7.5f;
                        break;
                    case ToolType.UNBREAKABLE:
                        unb = true;
                        break;
                    case ToolType.HAND:
                        blockbrtime = 0.65f;
                        break;
                    case ToolType.STONE_PICKAXE:
                        blockbrtime = 15f;
                        break;
                    case ToolType.IRON_PICKAXE:
                        blockbrtime = 15f;
                        break;
                    case ToolType.DIAMOND_PICKAXE:
                        blockbrtime = 100f;
                        break;
                    case ToolType.AXE:
                        blockbrtime = 3f;
                        break;
                }
                int x = !unb ? (int)Math.Floor((11*breaktime)/blockbrtime) : 0;
                if (x > 10) {
                    Program.game.level.BreakTileWithCoords(bCursor.X, bCursor.Y); 
                }
                else
                    crosshair.TextureRect = new IntRect(16*x,0,16,16);
            }
            else
                crosshair.TextureRect = new IntRect(0, 0, 16, 16);

            if (myTile!=null)
                debugBlock.Position = myTile.Position;
            if(myTile != null && myTile.amICollidable() && Velocity.Y >= 0f)
            {
                if (Math.Abs(Position.Y - myTile.Position.Y + 1.125f * Tile.TILE_SIZE) < (7f)/(1f/elapsed.AsSeconds()) * Tile.TILE_SIZE)
                {
                    Velocity.Y = 0f;
                    flying = false;
                    Position = new Vector2f(Position.X, myTile.Position.Y - 1.125f * Tile.TILE_SIZE);
                }
            }
            if (myTile == null || !myTile.amICollidable())
                flying = true;
            if (Velocity.Y < 512f && (flying))
            {
                Velocity.Y += GRAVITY * elapsed.AsSeconds();
            }

            if (Velocity.X > 0)
            {
                if (r1b)
                {
                    if (Position.X + (0.4f * HEAD_SIZE) > r1.Position.X)
                        Velocity.X = -32f;
                }if (r2b)
                {
                    if (Position.X + (0.4f * HEAD_SIZE) > r2.Position.X)
                        Velocity.X = -32f;
                }
            } else if (Velocity.X < 0)
            {
                if (l1b)
                {
                    if (Position.X - (0.4f * HEAD_SIZE) < l1.Position.X + Tile.TILE_SIZE)
                        Velocity.X = 32f;
                }
                if (l2b)
                {
                    if (Position.X - (0.4f * HEAD_SIZE) < l2.Position.X + Tile.TILE_SIZE)
                        Velocity.X = 32f;
                }
            }
            Tile n = Program.game.level.GetTileWithCoords(blockOnX, blockOnY - 1);
            while (n != null && n.amICollidable())
            {
                Position = new Vector2f(Position.X, Position.Y-16f);
                blockOnX = (int)((Position.X / Tile.TILE_SIZE));
                blockOnY = (int)Math.Ceiling((Position.Y + 1.125f * Tile.TILE_SIZE) / Tile.TILE_SIZE);
                n = Program.game.level.GetTileWithCoords(blockOnX, blockOnY - 1);
            }

            if ((spacekey && myTile != null && myTile.amICollidable()) && !flying)
            {
                flying = true;
                Velocity.Y = -185f;
            }

            if (dkey)
            {
                if (Velocity.X < HORIZONTAL_MOVESPEED)
                    Velocity.X += HORIZONTAL_ACCELERATION * elapsed.AsSeconds();
                else
                    Velocity.X = HORIZONTAL_MOVESPEED;
            }
            if (akey)
            {
                if (Velocity.X > -HORIZONTAL_MOVESPEED)
                    Velocity.X -= HORIZONTAL_ACCELERATION * elapsed.AsSeconds();
                else
                    Velocity.X = -HORIZONTAL_MOVESPEED;
            }
            if (!(dkey ^ akey))
            {
                if (Velocity.X > 0.1f || Velocity.X < -0.1f)
                    if (Velocity.X > 0)
                        Velocity.X -= HORIZONTAL_ACCELERATION * elapsed.AsSeconds();
                    else
                        Velocity.X += HORIZONTAL_ACCELERATION * elapsed.AsSeconds();
                else
                    Velocity.X = 0f;
                if(rightLeg.Rotation != 0f)
                {
                    if (Math.Abs(rightLeg.Rotation) < 5f)
                    {
                        rightLeg.Rotation = 0f;
                        leftLeg.Rotation = 0f;
                        leftArm.Rotation = 0f;
                    } else
                    {
                        if (rightLeg.Rotation > 0f)
                            rightLeg.Rotation -= 180f * elapsed.AsSeconds();
                        else
                            rightLeg.Rotation += 180f * elapsed.AsSeconds();
                        leftArm.Rotation = rightLeg.Rotation;
                        leftLeg.Rotation = -rightLeg.Rotation;
                    }
                    if (!lkeypressed)
                        rightArm.Rotation = -leftArm.Rotation;
                    animtime = (float)Math.Asin(rightLeg.Rotation/40f)*0.125f;
                }
            } else
            {
                rightLeg.Rotation = (float)Math.Sin(animtime*8f) * 40f * (dkey ? 1 : -1);
                leftArm.Rotation = rightLeg.Rotation;
                leftLeg.Rotation = -rightLeg.Rotation;
                if (!lkeypressed)
                    rightArm.Rotation = -leftArm.Rotation;
                animtime += elapsed.AsSeconds();
            }

            if(!Program.game.freecam)
                if (mousepos.X - (0.5f * Program.WIDTH) < 0)
                {
                    head.Scale = new Vector2f(-1, 1);
                    body.Scale = new Vector2f(-1, 1);
                    rightLeg.Scale = new Vector2f(-1, 1);
                    rightArm.Scale = new Vector2f(-1, 1);
                    leftArm.Scale = new Vector2f(-1, 1);
                    leftLeg.Scale = new Vector2f(-1, 1);
                }
                else
                {
                    head.Scale = new Vector2f(1, 1);
                    body.Scale = new Vector2f(1, 1);
                    rightArm.Scale = new Vector2f(1, 1);
                    rightLeg.Scale = new Vector2f(1, 1);
                    leftArm.Scale = new Vector2f(1, 1);
                    leftLeg.Scale = new Vector2f(1, 1);
                }
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            target.Draw(leftArm);
            target.Draw(leftLeg);
            target.Draw(body);
            target.Draw(rightArm);
            target.Draw(rightLeg);
            target.Draw(head);
            target.Draw(crosshair);

            target.Draw(debugBlock);
        }
        int rDivide(float x, float y)
        {

            int a = (int)(x / y);
            if (a > 0)
                return a;
            else
                return a;
        }
    }
}
