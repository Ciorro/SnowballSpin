using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace SnowballSpin.Physics
{
    class World
    {
        public List<Body> Bodies { get; set; } = new List<Body>();

        public void Update(float dt)
        {
            foreach (var body in Bodies)
            {
                body.Update(dt);
            }

            for (int i = 0; i < Bodies.Count; i++)
            {
                for (int j = i + 1; j < Bodies.Count; j++)
                {
                    Collide(Bodies[i], Bodies[j]);
                }
            }
        }

        private void Collide(Body b1, Body b2)
        {
            float rSum = b1.Radius + b2.Radius;

            if (Vector2.Distance(b1.Position, b2.Position) < rSum)
            {
                //resolve collision
                float percent = 0;

                var v1 = b1.Velocity;
                var v2 = b2.Velocity;
                var distLen = v1.Length() + v2.Length();

                if (distLen != 0)
                {
                    percent = v1.Length() / distLen;
                }
                else
                {
                    percent = 0.5f;
                }

                var intersectionDist = rSum - (b1.Position - b2.Position).Length();
                var norm = Vector2.Normalize(b1.Position - b2.Position);

                b1.Position += norm * intersectionDist * percent;
                b2.Position -= norm * intersectionDist * (1 - percent);

                //dynamics
                float dist = Vector2.Distance(b1.Position, b2.Position);

                if (dist == 0)
                    dist = 1f;

                Vector2 normal = new Vector2()
                {
                    X = (b2.Position.X - b1.Position.X) / dist,
                    Y = (b2.Position.Y - b1.Position.Y) / dist
                };

                Vector2 tangent = new Vector2(-normal.Y, normal.X);

                Vector2 dotTan = new Vector2()
                {
                    X = b1.Velocity.X * tangent.X + b1.Velocity.Y * tangent.Y,
                    Y = b2.Velocity.X * tangent.X + b2.Velocity.Y * tangent.Y
                };

                Vector2 dotNormal = new Vector2()
                {
                    X = b1.Velocity.X * normal.X + b1.Velocity.Y * normal.Y,
                    Y = b2.Velocity.X * normal.X + b2.Velocity.Y * normal.Y
                };

                //set new velocities
                b1.Velocity = new Vector2()
                {
                    X = tangent.X * dotTan.X + normal.X * dotNormal.Y,
                    Y = tangent.Y * dotTan.X + normal.Y * dotNormal.Y
                };

                b2.Velocity = new Vector2()
                {
                    X = tangent.X * dotTan.Y + normal.X * dotNormal.X,
                    Y = tangent.Y * dotTan.Y + normal.Y * dotNormal.X
                };
            }
        }
    }
}
