using HlyssMG.Objects;
using HlyssMG.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SnowballSpin.Controllers;
using SnowballSpin.Objects;

namespace SnowballSpin.Scenes
{
    class GameScn : Scene
    {
        private Level _level = new Level();

        private Player _player { get; set; }
        private Player[] _dummies = new Player[0];

        public GameScn()
        {
            Root.Children.Add(_level);
            Root.Children.Add(new GameObject()
            {
                Name = "players"
            });
            Root.Children.Add(new GameObject()
            {
                Name = "snowballs"
            });

            _dummies = new Player[1]
            {
                    new Player(_level)
            };

            foreach (var dummy in _dummies)
            {
                FindObject("players").Children.Add(dummy);
                _level.World.Bodies.Add(dummy.Body);
            }
        }

        public override void OnInit()
        {
            base.OnInit();

            _player = new Player(_level, true);
            FindObject("players").Children.Add(_player);

            foreach (var dummy in _dummies)
            {
                dummy.Controller = new AIController(_player);
            }
        }

        public override void Update(float dt)
        {
            base.Update(dt);

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
