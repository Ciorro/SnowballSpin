using HlyssMG.Actions;
using HlyssMG.Actions.Easings;
using HlyssMG.Objects;
using Microsoft.Xna.Framework;
using System;

namespace SnowballSpin.Objects
{
    class CameraController : GameObject
    {
        private Player _player;

        public CameraController(Player player)
        {
            _player = player;
        }

        public override void OnUpdate(float dt)
        {
            base.OnUpdate(dt);

            RemoveAllActions();
            AddAction(new MoveToAction(1, _player.Position, new EaseOutExpo()));

            float camX = MathF.Min(Level.MAP_WIDTH * Level.TILE_SIZE - 1920 / 2, MathF.Max(1920 / 2, Position.X));
            float camY = MathF.Min(Level.MAP_HEIGHT * Level.TILE_SIZE - 1080 / 2, MathF.Max(1080 / 2, Position.Y));

            Scene.Game.Renderer.Camera.Center = new Vector2((int)camX, (int)camY);
        }
    }
}
