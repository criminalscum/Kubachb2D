using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;

namespace Kubachb2D
{
    enum TileType
    {
        STONE, DIRT, GRASS, OAK_LEAVES, OAK_LOG, OAK_PLANKS, COBBLESTONE, STONE_BRICKS,
        SAND, GRAVEL, SNOWY_DIRT, SPRUCE_LEAVES, SPRUCE_LOG, SPRUCE_PLANKS, BRICKS, CLAY,
        WOOL, ICE, SNOW, BIRCH_LEAVES, BIRCH_LOG, BIRCH_PLANKS, GLASS, SANDSTONE,
        GOLD_ORE, IRON_ORE, COAL_ORE, DIAMOND_ORE, REDSTONE_ORE, LAPIS_ORE, OBSIDIAN, BEDROCK,
        GOLD_BLOCK, IRON_BLOCK, COAL_BLOCK, DIAMOND_BLOCK, REDSTONE_BLOCK, LAPIS_BLOCK, CACTUS,
        WORKBENCH, FURNACE, CHEST, TORCH, BED_BOT, BED_TOP,
        LAVA, WATER
    }
    class Tile : Transformable, Drawable
    {
        public const int TILE_SIZE = 64;
        public TileType type;
        public RectangleShape rectShape;
        public bool IsLit = false;
        public bool isAlternated = false;
        public int flow=0;
        public Dictionary<TileType, IntRect> textureRects = new Dictionary<TileType, IntRect>()
        {
            {TileType.STONE, new IntRect(0,0,16,16) }, {TileType.DIRT, new IntRect(16, 0, 16, 16) }, { TileType.GRASS, new IntRect(16*2,0,16,16) },
            {TileType.SAND, new IntRect(0,16,16,16) }, {TileType.GRAVEL, new IntRect(16,16,16,16) }, {TileType.SNOWY_DIRT, new IntRect(16*2, 16, 16, 16) },
            {TileType.WOOL, new IntRect(0,16*2,16,16) }, {TileType.ICE, new IntRect(16,16*2,16,16) }, {TileType.SNOW, new IntRect(16*2,16*2,16,16) },
            {TileType.OAK_LEAVES, new IntRect(16*3, 0,16,16) },{TileType.OAK_LOG, new IntRect(16*4,0,16,16) },{TileType.OAK_PLANKS, new IntRect(16*5,0,16,16) },
            {TileType.SPRUCE_LEAVES, new IntRect(16*3, 16,16,16) },{TileType.SPRUCE_LOG, new IntRect(16*4,16,16,16) },{TileType.SPRUCE_PLANKS, new IntRect(16*5,16,16,16) },
            {TileType.BIRCH_LEAVES, new IntRect(16*3, 16*2,16,16) },{TileType.BIRCH_LOG, new IntRect(16*4,16*2,16,16) },{TileType.BIRCH_PLANKS, new IntRect(16*5,16*2,16,16) },
            {TileType.COBBLESTONE, new IntRect(16*6,0,16,16) },{TileType.STONE_BRICKS, new IntRect(16*7,0,16,16) },{TileType.BRICKS, new IntRect(16*6,16,16,16) },
            {TileType.CLAY, new IntRect(16*7,16,16,16) },{TileType.GLASS, new IntRect(16*6,16*2,16,16) },{TileType.SANDSTONE, new IntRect(16*7,16*2,16,16) },
            {TileType.GOLD_ORE, new IntRect(0,16*3,16,16) }, {TileType.IRON_ORE, new IntRect(16,16*3,16,16) }, {TileType.COAL_ORE, new IntRect(16*2,16*3,16,16) },
            {TileType.DIAMOND_ORE, new IntRect(16*3,16*3,16,16) }, {TileType.REDSTONE_ORE, new IntRect(16*4,16*3,16,16) }, {TileType.LAPIS_ORE, new IntRect(16*5,16*3,16,16) },
            {TileType.OBSIDIAN, new IntRect(16*6, 16*3, 16,16) }, {TileType.BEDROCK, new IntRect(16*7,16*3,16,16) }, {TileType.GOLD_BLOCK, new IntRect(0,16*4,16,16) },
            {TileType.IRON_BLOCK, new IntRect(16,16*4,16,16) }, {TileType.COAL_BLOCK, new IntRect(16*2,16*4,16,16) }, {TileType.DIAMOND_BLOCK, new IntRect(16*3,16*4,16,16) },
            {TileType.REDSTONE_BLOCK, new IntRect(16*4,16*4,16,16) }, {TileType.LAPIS_BLOCK, new IntRect(16*5,16*4,16,16) }, {TileType.CACTUS, new IntRect(16*6,16*4,16,16)},
             {TileType.WORKBENCH, new IntRect(0,16*5,16,16) }, {TileType.CHEST, new IntRect(16*3,16*5,16,16) }, {TileType.TORCH, new IntRect(16*4,16*5,16,16) }
        };
        public Tile(TileType type)
        {
            this.type = type;
            rectShape = new RectangleShape(new Vector2f(TILE_SIZE, TILE_SIZE));
            setTexture();
        }
        public Tile(TileType type, Vector2f Pos)
        {
            this.type = type;
            rectShape = new RectangleShape(new Vector2f(TILE_SIZE, TILE_SIZE));
            Position = Pos;
            setTexture();
        }
        public void Update()
        {
            setTexture();
        }

        public bool amICollidable()
        {
            return !(type==TileType.OAK_LOG || type==TileType.OAK_LEAVES || type == TileType.SPRUCE_LOG || type == TileType.SPRUCE_LEAVES || type == TileType.BIRCH_LOG || type == TileType.BIRCH_LEAVES ||
                type == TileType.TORCH || type == TileType.CACTUS
                );
        }

        public void setTexture()
        {
            rectShape.Texture = Content.Blocks;
            rectShape.Size = new Vector2f(TILE_SIZE, TILE_SIZE);
            //rectShape.Origin = new Vector2f(TILE_SIZE / 2, TILE_SIZE / 2);
            switch (type)
            {
                case TileType.LAVA:
                    rectShape.TextureRect = new IntRect(flow*16,16*6,16,16);
                    break;
                case TileType.WATER:
                    rectShape.TextureRect = new IntRect(flow*16, 16 * 7, 16, 16);
                    break;
                case TileType.FURNACE:
                    if (IsLit)
                        rectShape.TextureRect = new IntRect(16 * 2, 16 * 5, 16, 16);
                    else
                        rectShape.TextureRect = new IntRect(16, 16 * 5, 16, 16);
                    break;
                case TileType.BED_BOT:
                    if (!isAlternated)
                        rectShape.TextureRect = new IntRect(16 * 5, 16 * 5, 16, 16);
                    else
                        rectShape.TextureRect = new IntRect(16*6,16*5,-16,16);
                    break;
                case TileType.BED_TOP:
                    if (!isAlternated)
                        rectShape.TextureRect = new IntRect(16 * 6, 16 * 5, 16, 16);
                    else
                        rectShape.TextureRect = new IntRect(16 * 7, 16 * 5, -16, 16);
                    break;
                default:
                    rectShape.TextureRect = textureRects[type];
                    break;
            }
        }
        public void Draw(RenderTarget target, RenderStates states)
        {
            states.Transform *= Transform;
            target.Draw(rectShape, states);
        }
    }
}
