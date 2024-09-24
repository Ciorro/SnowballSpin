using Microsoft.Xna.Framework;

namespace SnowballSpin.Objects
{
    struct Tile
    {
        public const int LAND_EDGE_Y = 0;
        public const int LAND_CORNER_Y = 16;
        public const int WATER_EDGE_Y = 32;
        public const int WATER_CORNER_Y = 48;

        public int X { get; set; }
        public int Y { get; set; }
        public Rectangle EdgeTextureRect { get; set; }
        public Rectangle CornerTextureRect { get; set; }

        public Tile(int x, int y, Rectangle edgeTextureRect, Rectangle cornerTextureRect)
        {
            X = x;
            Y = y;
            EdgeTextureRect = edgeTextureRect;
            CornerTextureRect = cornerTextureRect;
        }
    }
}
