using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SnowballSpin.Objects;

namespace SnowballSpin.Controllers
{
    class KeyboardController : IPlayerController
    {
        private KeyboardState _kState = Keyboard.GetState();

        public void ControlPlayer(Player player)
        {
            var ks = Keyboard.GetState();

            if (ks.IsKeyDown(Keys.Left))
            {
                player.RotateLeft();
            }

            if (ks.IsKeyDown(Keys.Right))
            {
                player.RotateRight();
            }

            if ((_kState.IsKeyDown(Keys.Right) && !ks.IsKeyDown(Keys.Right) ||
                _kState.IsKeyDown(Keys.Left) && !ks.IsKeyDown(Keys.Left)))
            {
                player.Shoot();
            }

            _kState = ks;

            //DEBUG
            if (ks.IsKeyDown(Keys.A))
                player.Body.Velocity += new Vector2(-50, 0);
            if (ks.IsKeyDown(Keys.D))
                player.Body.Velocity += new Vector2(50, 0);
            if (ks.IsKeyDown(Keys.W))
                player.Body.Velocity += new Vector2(0, -50);
            if (ks.IsKeyDown(Keys.S))
                player.Body.Velocity += new Vector2(0, 50);
        }
    }
}
