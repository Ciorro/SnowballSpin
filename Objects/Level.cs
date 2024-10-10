using HlyssMG.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SnowballSpin.Physics;
using System;

namespace SnowballSpin.Objects
{
    class Level : GameObject, HlyssMG.Graphics.IDrawable
    {
        public const int MAP_WIDTH = 40;
        public const int MAP_HEIGHT = 40;
        public const int TILE_SIZE = 96;

        private int[,] _map = new int[MAP_WIDTH, MAP_HEIGHT];

        private Tile[] _tiles = new Tile[MAP_WIDTH * MAP_WIDTH];
        private Texture2D _texture;

        public World World { get; set; } = new World();

        public Color Color { get; set; } = Color.White;
        public BlendState BlendState { get; set; } = BlendState.AlphaBlend;
        public SamplerState SamplerState { get; set; } = SamplerState.PointWrap;

        public override void OnAdded()
        {
            base.OnAdded();

            //load tiles texture
            _texture = Scene.Content.Load<Texture2D>("Gfx/tileset");

            //remove newlines from map file
            string map = Properties.Resources.SingleMap.Replace(Environment.NewLine, "");

            //fill _map array
            for (int y = 0; y < MAP_HEIGHT; y++)
            {
                for (int x = 0; x < MAP_WIDTH; x++)
                {
                    _map[x, y] = int.Parse(map[y * MAP_WIDTH + x].ToString());
                }
            }

            CreateTiles();
        }

        private void CreateTiles()
        {
            Random rockRnd = new Random();

            for (int i = 0; i < _tiles.Length; i++)
            {
                byte edge = 0b0000;
                byte corner = 0b0000;

                byte mask = 1;

                int x = i % MAP_WIDTH;
                int y = i / MAP_WIDTH;

                int loop = 0;

                //detect edges and corners
                for (int j = 0; j < 9; j++)
                {
                    if (j == 4) continue;

                    int rX = x + (j % 3) - 1;
                    int rY = y + (j / 3) - 1;

                    if (rX >= 0 && rX < MAP_WIDTH && rY >= 0 && rY < MAP_HEIGHT)
                    {
                        if (j % 2 == 0)
                        {
                            if (_map[rX, rY] == 0)
                                corner |= mask;
                        }
                        else
                        {
                            if (_map[rX, rY] == 0)
                                edge |= mask;
                        }
                    }
                    else if (j % 2 == 0)
                    {
                        corner |= mask;
                    }
                    else
                    {
                        edge |= mask;
                    }

                    if (loop % 2 == 1)
                        mask = (byte)(mask << 1);
                    loop++;
                }

                //set tiles
                if (_map[x, y] > 0)
                {
                    var edgeRect = new Rectangle(16 * edge, Tile.LAND_EDGE_Y, 16, 16);
                    var cornerRect = new Rectangle(16 * corner, Tile.LAND_CORNER_Y, 16, 16);

                    _tiles[i] = new Tile(x * TILE_SIZE, y * TILE_SIZE, edgeRect, cornerRect);

                    //add stone
                    if (/*(_map[x, y] > 0 && rockRnd.Next(100) < 5) || */_map[x, y] == 2)
                    {
                        Children.Add(new Sprite(_texture)
                        {
                            Position = new Vector2(x, y) * TILE_SIZE,
                            SamplerState = SamplerState.PointClamp,
                            TextureRect = new Rectangle(0, 64, 16, 16),
                            Size = new Vector2(16),
                            Scale = new Vector2(6)
                        });

                        World.Bodies.Add(new Body()
                        {
                            Radius = 34,
                            BodyType = BodyType.Static,
                            Position = new Vector2(x, y) * TILE_SIZE + new Vector2(TILE_SIZE) / 2
                        });
                    }
                }
                else
                {
                    var edgeRect = new Rectangle(16 * edge, Tile.WATER_EDGE_Y, 16, 16);
                    var cornerRect = new Rectangle(16 * corner, Tile.WATER_CORNER_Y, 16, 16);

                    _tiles[i] = new Tile(x * TILE_SIZE, y * TILE_SIZE, edgeRect, cornerRect);
                }
            }
        }

        public override void OnUpdate(float dt)
        {
            base.OnUpdate(dt);
            World.Update(dt);
        }

        public void Draw(SpriteBatch batch)
        {
            for (int i = 0; i < _tiles.Length; i++)
            {
                //draw tile with edges
                batch.Draw(_texture,
                    new Rectangle(_tiles[i].X, _tiles[i].Y, TILE_SIZE, TILE_SIZE),
                    _tiles[i].EdgeTextureRect,
                    Color);
                //draw corners on top of the tile
                batch.Draw(_texture,
                    new Rectangle(_tiles[i].X, _tiles[i].Y, TILE_SIZE, TILE_SIZE),
                    _tiles[i].CornerTextureRect,
                    Color);
            }
        }

        public Vector2 GetRandomSpawnpoint()
        {
            Random rnd = new Random();
            int x = rnd.Next(0, MAP_WIDTH);

            for (int i = 0; i < MAP_WIDTH; i++)
            {
                if (x == MAP_WIDTH)
                    x = 0;

                int y = rnd.Next(0, MAP_HEIGHT);

                for (int j = 0; j < MAP_HEIGHT; j++)
                {
                    if (y == MAP_HEIGHT)
                        y = 0;

                    if (_map[x, y] == 1)
                        return new Vector2(x, y) * TILE_SIZE + new Vector2(TILE_SIZE) / 2;
                    y++;
                }
                x++;
            }

            return Vector2.Zero;
        }

        public bool IsLand(Vector2 position)
        {
            position /= TILE_SIZE;

            int x = (int)position.X;
            int y = (int)position.Y;

            if (x < 0 || x >= MAP_WIDTH || y < 0 || y >= MAP_HEIGHT)
                return false;

            return _map[x, y] > 0;
        }
    }
}
