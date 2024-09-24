using HlyssMG.Actions;
using HlyssMG.Actions.Easings;
using HlyssMG.Input;
using HlyssMG.Input.Events;
using HlyssMG.Objects;
using HlyssMG.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SnowballSpin.Controllers;
using SnowballSpin.Physics;
using System;

namespace SnowballSpin.Objects
{
    class Player : Sprite
    {
        public const int RADIUS = 64;

        public bool IsPlayerControlled { get; private set; }
        public IPlayerController Controller { get; set; }
        public Body Body { get; private set; }

        public bool IsDead { get; set; }

        public bool IsAIControlled => Controller is AIController;

        private Level _level;
        private SoundEffect _splashSfx;
        private SoundEffect _shootSfx;
        private SoundEffect _hitSfx;

        public Player(Level level, bool isControllable = false)
        {
            _level = level;
            IsPlayerControlled = isControllable;

            Body = new Body();
            Origin = new Vector2(0.5f);

            Children.Add(new CameraController(this));

            Random _colorRnd = new Random();

            int r = _colorRnd.Next(70, 255);
            int g = _colorRnd.Next(70, 255);
            int b = _colorRnd.Next(70, 255);

            Color = new Color(r, g, b);
        }

        public override void OnLoad(ContentManager content)
        {
            base.OnLoad(content);
            Texture = content.Load<Texture2D>("Gfx/player");

            _splashSfx = content.Load<SoundEffect>("Audio/splash");
            _shootSfx = content.Load<SoundEffect>("Audio/throw");
            _hitSfx = content.Load<SoundEffect>("Audio/hit");
        }

        public override void OnAdded()
        {
            base.OnAdded();

            if (IsPlayerControlled)
            {
                _level.World.Bodies.Add(Body);
                Body.Position = _level.GetRandomSpawnpoint();
                Controller = new KeyboardController();
            }
            else if (IsAIControlled)
            {
                Body.Position = _level.GetRandomSpawnpoint();
            }
        }

        public override void OnUpdate(float dt)
        {
            base.OnUpdate(dt);

            if (!IsDead)
            {
                if (IsPlayerControlled || IsAIControlled)
                {
                    Position = Body.Position;
                }

                if (Controller != null)
                {
                    Controller.ControlPlayer(this);
                }

                if (!_level.IsLand(Position) && (IsPlayerControlled || IsAIControlled))
                {
                    IsDead = true;
                    RemoveAllActions();

                    AddAction(new ActionSequence(
                        new ScaleToAction(0.5f, Vector2.Zero),
                        new CallFuncAction((_) =>
                        {
                            _level.World.Bodies.Remove(Body);
                            (_ as GameObject).RemoveFromParent();
                        }, this)
                    ));

                    _splashSfx.Play();
                }
            }
        }

        public void Hit(Snowball snowball)
        {
            var travelVec = (snowball.Position - snowball.StartPosition);
            var travelLen = travelVec.Length();

            if (!snowball.IsDummy)
            {
                Body.Velocity += travelVec * (1 - (travelLen / 500f)) * Snowball.KNOCKBACK;
            }

            _hitSfx.Play();
        }

        public void Shoot(Vector2 position = default, float rotation = 0)
        {
            if (position == default)
            {
                position = Position;
                rotation = Rotation;
            }

            RemoveAllActions();

            Body.Velocity += HMGMath.VectorFromRotation(Rotation + 180) * 300;
            bool dummy = !(IsPlayerControlled || IsAIControlled);

            Snowball snowball = new Snowball(this, dummy);
            snowball.Position = position;

            var rotationVector = HMGMath.VectorFromRotation(rotation);
            snowball.Position += rotationVector * (Size.X / 2 + 32);

            snowball.AddAction(new ActionSequence(
                new ActionParallel(
                    new MoveByAction(0.4f, rotationVector * 500, new EaseOutCirc()),
                    new ActionSequence(
                        new WaitAction(0.3f),
                        new FadeOutAction(0.1f)
                    )
                ),
                new CallFuncAction((_) => { (_ as GameObject).RemoveFromParent(); }, snowball)
            ));

            Scene.FindObject("snowballs").Children.Add(snowball);
            _shootSfx.Play(0.1f, 0, 0);
        }

        public void RotateLeft()
        {
            RemoveAllActions();
            AddAction(new RotateByAction(1, -45, new EaseOutExpo()));
        }

        public void RotateRight()
        {
            RemoveAllActions();
            AddAction(new RotateByAction(1, 45, new EaseOutExpo()));
        }

        public override void OnEvent(EventToken ev)
        {
            base.OnEvent(ev);

            //DEBUG
            if (ev.GetEvent() is MouseButtonEvent mbev)
            {
                if (mbev.Type == EventType.Released)
                {
                    Vector2 mLoc = Scene.Game.Renderer.Camera.ScreenToWorld(mbev.Location);

                    if (mbev.Button == MouseButton.Right)
                        Body.Velocity += (mLoc - Body.Position) * 5;
                    else if (mbev.Button == MouseButton.Left)
                        Body.Velocity += (mLoc - Body.Position) * 5;

                }
            }
        }
    }
}
