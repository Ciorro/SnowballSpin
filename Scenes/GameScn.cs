using System;
using System.Collections.Generic;
using System.Linq;
using HlyssMG.Objects;
using HlyssMG.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SnowballSpin.Controllers;
using SnowballSpin.Network;
using SnowballSpin.Objects;

namespace SnowballSpin.Scenes
{
    class GameScn : Scene
    {
        private Level _level = new Level();

        private Player _player { get; set; }
        private List<Player> _dummies = new List<Player>();
        private bool _isOnline;

        public GameScn(bool isOnline = true)
        {
            _isOnline = isOnline;
            Root.Children.Add(_level);
            Root.Children.Add(new GameObject()
            {
                Name = "players"
            });
            Root.Children.Add(new GameObject()
            {
                Name = "snowballs"
            });

            if (!_isOnline)
            {
                _dummies.Add(new Player(_level));
                foreach (var dummy in _dummies)
                {
                    FindObject("players").Children.Add(dummy);
                    _level.World.Bodies.Add(dummy.Body);
                }
            }
        }

        public override void OnInit()
        {
            base.OnInit();


            if (!_isOnline)
            {
                _player = new Player(_level, true);
                // AI control if offline
                foreach (var dummy in _dummies)
                {
                    dummy.Controller = new AIController(_player);
                }
            }
            else
            {
                // var id = NetworkClient.GetPlayerId(); // Connect to server
                _player = new Player(_level, true);
            }
            FindObject("players").Children.Add(_player);
        }

        public override void Update(float dt)
        {
            base.Update(dt);
            if (_isOnline)
            {
                NetworkClient.Update();
                var gameState = NetworkClient.GetGameState();
                gameState.Players.RemoveAll(x => x.PlayerId == _player.Id);
                var newPlayers = gameState.Players.Where(x => !_dummies.Any(y => x.PlayerId == y.Id));
                var removedPlayers = _dummies.Where(x => !gameState.Players.Any(y => x.Id == y.PlayerId));
                FindObject("players").Children.RemoveAll(x => x is Player p && removedPlayers.Any(r => r.Id == p.Id));
                _dummies.RemoveAll(x => removedPlayers.Any(y => x.Id == y.Id));
                foreach (var p in newPlayers)
                {
                    var newPlayer = new Player(_level, id: p.PlayerId);
                    newPlayer.Controller = new NetworkPlayerController();
                    _dummies.Add(newPlayer);

                    FindObject("players").Children.Add(newPlayer);
                    _level.World.Bodies.Add(newPlayer.Body);
                }

                foreach (var p in _dummies)
                {
                    if (p.IsPlayerControlled) continue;

                    var data = gameState.Players.Where(x => x.PlayerId == p.Id).FirstOrDefault();
                    if (data == null) continue;

                    p.Body.Position = new Vector2(data.PositionX, data.PositionY);
                    p.Position = p.Body.Position;
                    p.Rotation = data.Rotation;
                    // System.Console.WriteLine($"SET {DateTime.Now.Millisecond}");
                }

                foreach (var s in gameState.Snowballs)
                {
                    if (s.PlayerId == _player.Id) continue;

                    var player = _dummies.Where(x => x.Id == s.PlayerId).FirstOrDefault();
                    player?.Shoot(player.Position, s.Rotation);
                    // System.Console.WriteLine("SZUT");
                }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Game.SceneManager.SetScene(new MenuScn());
            }

            //check snowball-player collisions
            foreach (var player in FindObject("players").Children)
            {
                foreach (Snowball snowball in FindObject("snowballs").Children)
                {
                    if (snowball.Sender == player)
                        continue;

                    var len = (snowball.Position - snowball.PrevPosition).Length();
                    var normal = Vector2.Normalize(snowball.Position - snowball.PrevPosition);

                    for (int i = 0; i < len; i++)
                    {
                        if (Vector2.Distance(snowball.PrevPosition + normal * i, player.Position) < Player.RADIUS)
                        {
                            (player as Player).Hit(snowball);

                            snowball.RemoveAllActions();
                            snowball.RemoveFromParent();

                            break;
                        }
                    }
                }
            }
        }
    }
}
