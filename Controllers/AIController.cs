using HlyssMG.Utils;
using Microsoft.Xna.Framework;
using SnowballSpin.Objects;
using System;
using System.Diagnostics;

namespace SnowballSpin.Controllers
{
    class AIController : IPlayerController
    {
        enum AIAction
        {
            RotateLeft, RotateRight, None
        }

        private Player _target;
        private AIAction _action = AIAction.None;
        private Stopwatch _s = Stopwatch.StartNew();

        private int _nextDelay = 100;
        private Random _delayRnd = new Random();

        public AIController(Player target)
        {
            _target = target;
        }

        public void ControlPlayer(Player player)
        {
            AIAction prevAction = _action;

            if (_s.ElapsedMilliseconds > _nextDelay)
            {
                _s.Restart();
                _nextDelay = _delayRnd.Next(80, 150);

                UpdateAction(player);
            }

            if (_action != prevAction)
                player.Shoot();

            if (_action == AIAction.RotateLeft)
                player.RotateLeft();
            if (_action == AIAction.RotateRight)
                player.RotateRight();
        }

        private void UpdateAction(Player player)
        {
            if (player.Rotation < 0)
                player.Rotation += 360;

            var tDirection = HMGMath.RotationTowardsPoint(player.Position, _target.Position);

            //follow
            if (Vector2.Distance(_target.Position, player.Position) > 500)
            {
                tDirection += 180;
            }

            if (tDirection < 0)
                tDirection += 360;

            float rotLeft = (360 - tDirection + player.Rotation) % 360;

            if (rotLeft > 180)
                _action = AIAction.RotateRight;
            if (rotLeft < 180)
                _action = AIAction.RotateLeft;
        }
    }
}
