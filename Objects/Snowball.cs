using System;
using HlyssMG.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SnowballSpin.Objects
{
    class Snowball : Sprite
    {
        public const int KNOCKBACK = 8;

        public bool IsDummy { get; private set; }
        public Player Sender { get; private set; }
        public Vector2 StartPosition { get; private set; }
        public Vector2 PrevPosition { get; private set; }

        public Snowball(Player sender, bool dummy = false)
        {
            IsDummy = dummy;
            Sender = sender;
            Origin = new Vector2(0.5f);
        }

        public override void OnLoad(ContentManager content)
        {
            base.OnLoad(content);
            Texture = content.Load<Texture2D>("Gfx/snowball");

            StartPosition = Position;
        }

        public override void OnUpdate(float dt)
        {
            PrevPosition = Position;
            base.OnUpdate(dt);
        }
    }
}
