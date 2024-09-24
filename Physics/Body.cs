using Microsoft.Xna.Framework;

namespace SnowballSpin.Physics
{
    class Body
    {
        public int Radius { get; set; } = 64;
        public BodyType BodyType { get; set; } = BodyType.Dynamic;
        public Vector2 Velocity { get; set; } = new Vector2();
        public Vector2 Position { get; set; } = new Vector2();

        private float _elapsed = 0;

        public void Update(float dt)
        {
            if (BodyType == BodyType.Static)
            {
                Velocity = Vector2.Zero;
                return;
            }

            Position += Velocity * dt;

            _elapsed += dt;
            if (_elapsed >= 1f / 60)
            {
                Velocity *= 0.95f;
                _elapsed = 0;
            }
        }
    }
}
