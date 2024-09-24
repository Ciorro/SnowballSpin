﻿using HlyssMG.Actions;
using HlyssMG.Objects;
using HlyssMG.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SnowballSpin.Ui;
using System.Collections.Generic;

namespace SnowballSpin.Scenes.Settings
{
    class FullscreenCfgScn : Scene
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
                new Text(font, "Fullscreen")
                {
                    Color = Color.Black,
                    Origin = new Vector2(0.5f, 0),
                    Scale = new Vector2(1.5f),
                    Position = new Vector2(1920 / 2, 50) + new Vector2(5),
                    Name = "shadow"
                },
                new Text(font, "Fullscreen")
                {
                    Color = Color.Green,
                    Origin = new Vector2(0.5f, 0),
                    Scale = new Vector2(1.5f),
                    Position = new Vector2(1920 / 2, 50),
                    Name = "header"
                },
                new Menu("On", "Off",  "Back")
                {
                    Position = ScreenArea.Center.ToVector2(),
                    OnSelected = HandleMenu
                }
            };
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
                Game.IsFullscreen = true;
            }
            else if (optionId == 1)
            {
                Game.IsFullscreen = false;
            }
            else if (optionId == 2)
            {
                Game.SceneManager.SetScene(new SettingsScn());
            }
        }
    }
}
