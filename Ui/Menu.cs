using HlyssMG.Actions;
using HlyssMG.Actions.Easings;
using HlyssMG.Input;
using HlyssMG.Input.Events;
using HlyssMG.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace SnowballSpin.Ui
{
    class Menu : GameObject
    {
        public const int ACCPET_TIME = 1;

        public Action<int> OnSelected { get; set; }

        string[] _options;
        Text _cursor;
        int _currentOption = 0;
        float _elapsed = 0;
        Sprite _bar;

        SoundEffect _sfx;

        public Menu(params string[] options)
        {
            _options = options;

            Anchor = new Vector2(0.5f, 0);
            Origin = new Vector2(0.5f);
        }

        public override void OnLoad(ContentManager content)
        {
            base.OnLoad(content);

            var font = content.Load<SpriteFont>("Fonts/slkscr_lg");

            for (int i = 0; i < _options.Length; i++)
            {
                Text text = new Text(font, _options[i])
                {
                    Origin = new Vector2(0.5f),
                    Position = new Vector2(0, i * 80),
                    Color = Color.Green
                };

                Text shadow = new Text(font, _options[i])
                {
                    Origin = new Vector2(0.5f),
                    Position = new Vector2(0, i * 80) + new Vector2(3),
                    Color = Color.Black
                };

                Children.Add(shadow);
                Children.Add(text);
            }

            Text cursor = new Text(font, ">                <")
            {
                Origin = new Vector2(0.5f),
                Color = Color.DarkRed
            };
            Children.Add(cursor);
            _cursor = cursor;

            _sfx = content.Load<SoundEffect>("Audio/click");
        }

        public override void OnAdded()
        {
            base.OnAdded();
            Texture2D barTexture = new Texture2D(Scene.Game.GraphicsDevice, 1, 1);
            barTexture.SetData(new byte[] { 139, 0, 0, 255 });
            _bar = new Sprite(barTexture);
            _bar.Origin = new Vector2(0.5f);
            Children.Add(_bar);
        }

        public override void OnEvent(EventToken ev)
        {
            base.OnEvent(ev);

            if (ev.GetEvent() is KeyboardEvent kev)
            {
                if (kev.Type == EventType.Released)
                {
                    if (kev.Key == Keys.Down)
                        _currentOption++;
                    if (kev.Key == Keys.Up)
                        _currentOption--;

                    _currentOption = Math.Min(_options.Length - 1, Math.Max(_currentOption, 0));

                    _cursor.RemoveAllActions();
                    _cursor.AddAction(new MoveToAction(0.2f, new Vector2(0, _currentOption * 80), new EaseOutExpo()));

                    _elapsed = 0;
                    _sfx.Play();
                }
            }
        }

        public override void OnUpdate(float dt)
        {
            base.OnUpdate(dt);

            var ks = Keyboard.GetState();

            if (ks.IsKeyDown(Keys.Up) || ks.IsKeyDown(Keys.Down))
            {
                _elapsed += dt;
            }

            if (_elapsed >= ACCPET_TIME)
            {
                OnSelected?.Invoke(_currentOption);
            }

            _bar.Position = new Vector2(0, _currentOption * 80);
            _bar.Scale = new Vector2(Math.Min(700, 700 * _elapsed / ACCPET_TIME), 5);
        }
    }
}
