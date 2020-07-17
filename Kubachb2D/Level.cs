using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.System;
using SFML.Graphics;

namespace Kubachb2D
{
    class Level : Drawable
    {
        public int RenderPerFrame = 0;
        public Tile[,] tiles;

        public Level(Tile[,] tiles)
        {
            this.tiles = tiles;
        }

        public static Tile[,] Generate(int width)
        {
            Tile[,] genTiles = new Tile[width, 128];
            float[] nn = Program.Noise(91298412, 10, width, 10, 57);
            float[] ss = Program.Noise(91298412, 10, width, 10, 60);
            float[] temp = Program.Noise(91298412, 100,width,width/3,10);
            for (int x = 1; x < width; x++)
                genTiles[x, 127] = new Tile(TileType.BEDROCK, new Vector2f(x*Tile.TILE_SIZE, 127*Tile.TILE_SIZE));
            for (int x = 1; x < width; x++)
            {
                int y1 = (int)Math.Round(nn[x]);
                for (int y=y1; y < 127; y++)
                {
                    if (temp[x]<60 && temp[x]>40) {
                        if (y == y1)
                            genTiles[x, y] = new Tile(TileType.GRASS, new Vector2f(x * Tile.TILE_SIZE, y * Tile.TILE_SIZE));
                        else if (y < ss[x])
                            genTiles[x, y] = new Tile(TileType.DIRT, new Vector2f(x * Tile.TILE_SIZE, y * Tile.TILE_SIZE));
                        else if (y > Math.Round(ss[x]) - 1)
                            genTiles[x, y] = new Tile(TileType.STONE, new Vector2f(x * Tile.TILE_SIZE, y * Tile.TILE_SIZE)); }
                    else if (temp[x] <= 40) {
                        if (y == y1)
                            genTiles[x, y] = new Tile(TileType.SNOWY_DIRT, new Vector2f(x * Tile.TILE_SIZE, y * Tile.TILE_SIZE));
                        else if (y < ss[x])
                            genTiles[x, y] = new Tile(TileType.DIRT, new Vector2f(x * Tile.TILE_SIZE, y * Tile.TILE_SIZE));
                        else if (y > Math.Round(ss[x]) - 1)
                            genTiles[x, y] = new Tile(TileType.STONE, new Vector2f(x * Tile.TILE_SIZE, y * Tile.TILE_SIZE));
                    }
                    else if (temp[x] >= 60)
                    {
                        if (y >= y1 && y<=y1+2)
                            genTiles[x, y] = new Tile(TileType.SAND, new Vector2f(x * Tile.TILE_SIZE, y * Tile.TILE_SIZE));
                        else if (y < ss[x]+2)
                            genTiles[x, y] = new Tile(TileType.SANDSTONE, new Vector2f(x * Tile.TILE_SIZE, y * Tile.TILE_SIZE));
                        else if (y > Math.Round(ss[x]) - 4)
                            genTiles[x, y] = new Tile(TileType.STONE, new Vector2f(x * Tile.TILE_SIZE, y * Tile.TILE_SIZE));
                    }
                }
            }
            Random r = new Random(91298412);
            Generate_Vains(ref r, ref genTiles, TileType.COAL_ORE, width, ss, 5, 3, new Vector2i(3,10));
            Generate_Vains(ref r, ref genTiles, TileType.IRON_ORE, width, ss, 15, 5, new Vector2i(2, 6));
            Generate_Vains(ref r, ref genTiles, TileType.GOLD_ORE, width, ss, 30, 10, new Vector2i(3, 5));
            Generate_Vains(ref r, ref genTiles, TileType.REDSTONE_ORE, width, ss, 30, 8, new Vector2i(1, 4));
            Generate_Vains(ref r, ref genTiles, TileType.DIAMOND_ORE, width, ss, 40, 20, new Vector2i(3,8));
            Generate_Vains(ref r, ref genTiles, TileType.LAPIS_ORE, width, ss, 30, 20, new Vector2i(2, 4));
            List<int> br = new List<int>();
            for (int i=0; i<width/15;i++)
            {
                bool n = true;
                while (n)
                {
                    int a = r.Next(10, width-10);
                    if (Math.Round(nn[a])<=Math.Round(nn[a+1])&& Math.Round(nn[a])<=Math.Round(nn[a-1]) && !br.Contains(a))
                    {
                        List<Tile> tree = new List<Tile>();
                        if (temp[a] > 40 && temp[a] < 60)
                            if(r.Next()>Int32.MaxValue/2)
                                tree = Generate_Tree(TileType.OAK_LOG, TileType.OAK_LEAVES, r.Next(3, 5), a, (int)Math.Round(nn[a]) - 1);
                            else
                                tree = Generate_Tree(TileType.BIRCH_LOG, TileType.BIRCH_LEAVES, r.Next(3, 5), a, (int)Math.Round(nn[a]) - 1);
                        else if(temp[a]<=40)
                            tree = Generate_Tree(TileType.SPRUCE_LOG, TileType.SPRUCE_LEAVES, r.Next(3, 5), a, (int)Math.Round(nn[a]) - 1);
                        else if (temp[a] >= 60)
                            tree = Generate_Tree(TileType.CACTUS, TileType.REDSTONE_BLOCK, r.Next(3, 5), a, (int)Math.Round(nn[a]) - 1);
                        foreach (Tile t in tree)
                            if (t.type != TileType.REDSTONE_BLOCK)
                                genTiles[(int)t.Position.X / Tile.TILE_SIZE, (int)t.Position.Y / Tile.TILE_SIZE] = t;
                        for (int g = a - 3; g < a + 4; g++)
                            br.Add(g);
                        n = false;
                    }
                }
            }
            return genTiles;
        }

        private static void Generate_Vains(ref Random r, ref Tile[,] tiles, TileType type, int width, float[] stonel,int mindgrlvl, int rarity, Vector2i binVain)
        {
            for(int i = 0; i < width / rarity; i++)
            {
                int x = r.Next(5, width-5);
                int y = r.Next((int)Math.Round(stonel[x])+mindgrlvl, 123);
                List<Tile> v = Generate_Ore_Vain(x, y, type, r, binVain.X, binVain.Y);
                foreach (Tile t in v)
                {
                    tiles[(int)t.Position.X/Tile.TILE_SIZE, (int)t.Position.Y/Tile.TILE_SIZE] = t;
                }
            }
        }

        private static List<Tile> Generate_Tree(TileType trunk, TileType leaves, int trunkheight, int xcenterb, int ycenterb)
        {
            List<Tile> tree = new List<Tile>();
            for (int i = 0; i < trunkheight; i++)
                tree.Add(new Tile(trunk, new Vector2f(xcenterb * Tile.TILE_SIZE, ycenterb * Tile.TILE_SIZE - (i * Tile.TILE_SIZE))));
            for (int i = trunkheight; i < trunkheight + 3; i++)
                for (int x = -2; x < 3; x++)
                    if (i < trunkheight + 1)
                        tree.Add(new Tile(leaves, new Vector2f(xcenterb*Tile.TILE_SIZE+(x*Tile.TILE_SIZE), ycenterb*Tile.TILE_SIZE-i*Tile.TILE_SIZE)));
                    else if(Math.Abs(x)<=1)
                        tree.Add(new Tile(leaves, new Vector2f(xcenterb * Tile.TILE_SIZE + (x * Tile.TILE_SIZE), ycenterb*Tile.TILE_SIZE - i * Tile.TILE_SIZE)));
            return tree;
        }

        private static List<Tile> Generate_Ore_Vain(int xcenterb, int ycenterb, TileType btype, Random r, int rmin, int rmax)
        {
            List<Tile> vain = new List<Tile>();
            for(int i=0;i<r.Next(rmin,rmax); i++)
            {
                if (i == 0)
                    vain.Add(new Tile(btype, new Vector2f(xcenterb*Tile.TILE_SIZE, ycenterb*Tile.TILE_SIZE)));
                else
                {
                    Tile n = vain[r.Next(vain.Count)];
                    int a = r.Next(4);
                    switch (a)
                    {
                        case 0:
                            vain.Add(new Tile(btype, new Vector2f(n.Position.X+Tile.TILE_SIZE, n.Position.Y)));
                            break;
                        case 1:
                            vain.Add(new Tile(btype, new Vector2f(n.Position.X-Tile.TILE_SIZE, n.Position.Y)));
                            break;
                        case 2:
                            vain.Add(new Tile(btype, new Vector2f(n.Position.X, n.Position.Y+Tile.TILE_SIZE)));
                            break;
                        case 3:
                            vain.Add(new Tile(btype, new Vector2f(n.Position.X, n.Position.Y-Tile.TILE_SIZE)));
                            break;
                    }
                }
            }
            return vain;
        }

        public void Generate_Debug()
        {
            for(int y=9; y<60; y++)
                for(int x=1;x<10;x++)
                {
                    
                    if (y >= 10)
                    {
                        Tile t = new Tile(TileType.DIRT);
                        t.Position = new Vector2f(x * Tile.TILE_SIZE, y * Tile.TILE_SIZE);
                        t.isAlternated = true;
                        t.Update();
                        tiles[x, y] = t;
                    }
                    else if (x%2==0) {
                        Tile t = new Tile(TileType.STONE);
                        t.Position = new Vector2f(x * Tile.TILE_SIZE, y * Tile.TILE_SIZE);
                        t.isAlternated = true;
                        t.Update();
                        tiles[x, y] = t;
                    }
                    
                }
        }

        public Tile GetTileWithCoords(int x, int y)
        {
            if (x < 0 || x > tiles.GetLength(0)-1 || y < 0 || y > tiles.GetLength(1)-1)
                return null;
            if (tiles[x, y] != null)
                return tiles[x, y];
            else
                return null;
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            int n = 0;
            for (int x = 0; x < tiles.GetLength(0); x++)
                for (int y = 0; y < tiles.GetLength(1); y++)
                    if (tiles[x, y] != null)
                    {
                        if (!Program.game.freecam)
                            if (Math.Abs(Program.game.player.Position.X - tiles[x, y].Position.X) < Program.WIDTH / 2f + Tile.TILE_SIZE &&
                                Math.Abs(Program.game.player.Position.Y - tiles[x, y].Position.Y) < Program.HEIGHT / 2f + Tile.TILE_SIZE)
                            {
                                n++;
                                target.Draw(tiles[x, y]);
                            }
                        if (Program.game.freecam)
                        {
                            if (Math.Abs(Program.game.camera.Position.X - tiles[x, y].Position.X) < Program.WIDTH / 2f + Tile.TILE_SIZE &&
                                Math.Abs(Program.game.camera.Position.Y - tiles[x, y].Position.Y) < Program.HEIGHT / 2f + Tile.TILE_SIZE)
                            {
                                n++;
                                target.Draw(tiles[x, y]);
                            }
                        }
                    }
            RenderPerFrame = n;
        }
    }
}
