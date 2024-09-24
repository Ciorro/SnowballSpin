using HlyssMG.Actions;
using HlyssMG.Objects;
using HlyssMG.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using SnowballSpin.Ui;
using System.Collections.Generic;

namespace SnowballSpin.Scenes
{
    class MenuScn : Scene
    {
        public override void Start()
        {
            base.Start();

            Game.Renderer.Camera = null;

            var font = Content.Load<SpriteFont>("Fonts/slkscrb_lg");

            Root.Children = new List<GameObject>
            {
                new Sprite(Content.Load<Texture2D>("Gfx/menu_bg"))
                {
                    Origin = new Vector2(0.5f),
                    Position = ScreenArea.Center.ToVector2()
                },
                new Text(font, "Snowball Spin!")
                {
                    Color = Color.Black,
                    Origin = new Vector2(0.5f, 0),
                    Scale = new Vector2(1.5f),
                    Position = new Vector2(1920 / 2, 50) + new Vector2(5),
                    Name = "shadow"
                },
                new Text(font, "Snowball Spin!")
                {
                    Color = Color.Green,
                    Origin = new Vector2(0.5f, 0),
                    Scale = new Vector2(1.5f),
                    Position = new Vector2(1920 / 2, 50),
                    Name = "header"
                },
                new Menu("Play", "Settings", "Credits", "Exit")
                {
                    Position = ScreenArea.Center.ToVector2(),
                    OnSelected = HandleMenu
                }
            };

            if (MediaPlayer.State != MediaState.Playing)
            {
                MediaPlayer.Play(Content.Load<Song>("Audio/JumpAndRun"));
                MediaPlayer.Volume = 0.0f;
                MediaPlayer.IsRepeating = true;
            }
        }

        public override void Update(float dt)
        {
            base.Update(dt);

            if (FindObject("shadow").ActionCount == 0)
            {
                FindObject("shadow").AddAction(new ActionSequence(
                    new MoveByAction(0.5f, new Vector2(2.5f)),
                    new MoveByAction(0.5f, new Vector2(-2.5f))
                ));
            }

            if (FindObject("header").ActionCount == 0)
            {
                FindObject("header").AddAction(new ActionSequence(
                    new MoveByAction(0.5f, new Vector2(-2.5f)),
                    new MoveByAction(0.5f, new Vector2(2.5f))
                ));
            }
        }

        private void HandleMenu(int optionId)
        {
            if (optionId == 0)
            {
                MediaPlayer.Stop();
                Game.SceneManager.SetScene(new GameScn());
            }
            else if (optionId == 1)
            {
                Game.SceneManager.SetScene(new SettingsScn());
            }
            else if (optionId == 3)
            {
                MediaPlayer.Stop();
                Game.Exit();
            }
        }
    }
}
