using System;
using System.Collections.Generic;

namespace Acoross.Game2d
{
    /// <summary>
    /// basic moving unit
    /// </summary>
    public abstract class Collider
    {
        public GameObject gameObject { get; }
        public Vector2d position
        {
            get
            {
                if (gameObject == null)
                    return offset;

                return gameObject.position + offset;
            }
        }
        public Vector2d rotation
        {
            get
            {
                if (gameObject == null)
                    return Vector2d.X;

                return gameObject.rotation;
            }
        }

        public Vector2d offset { get; protected set; }

        public Collider(GameObject owner, Vector2d offset)
        {
            this.gameObject = owner;
            this.offset = offset;
        }

        public Collider(GameObject owner)
        {
            this.gameObject = owner;
            this.offset = Vector2d.Zero;
        }

        public abstract bool IsCollidingTo(Collider col);
        public abstract bool IsCollidingTo(CircleCollider col);
        public abstract bool IsCollidingTo(LineCollider col);
        public abstract bool IsCollidingTo(BoxCollider col);

        public void OnColliding(Collider col)
        {
            gameObject?.OnColliding(col);
        }
    }

    public static class CollisionDetector
    {
        public static bool CircleAndCircle(CircleCollider s1, CircleCollider s2)
        {
            var midline = s1.position - s2.position;
            var limit = (s1.radius + s2.radius);
            if (midline.sqMagnitude() <= limit * limit)
            {
                return true;
            }

            return false;
        }

        // harf-space to circle
        public static bool CircleAndHaflSpace(CircleCollider circle, LineCollider line)
        {
            var dist = (line.normal * circle.position) - circle.radius - line.lineOffset;
            if (dist < 0)
                return true;

            return false;
        }

        public static bool BoxAndHalfSpace(BoxCollider box, LineCollider line)
        {
            var vs = box.GetVertexes();
            foreach (var v in vs)
            {
                float vertexDist = v * line.normal;
                if (vertexDist <= line.lineOffset)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool BoxAndCircle(BoxCollider box, CircleCollider sphere)
        {
            var relCenter = sphere.position - box.position;

            float dist;

            dist = relCenter.x;
            if (dist > box.halfSize.x)
                dist = box.halfSize.x;
            if (dist < -box.halfSize.x)
                dist = -box.halfSize.x;
            var closestX = dist;

            dist = relCenter.y;
            if (dist > box.halfSize.y)
                dist = box.halfSize.y;
            if (dist < -box.halfSize.y)
                dist = -box.halfSize.y;
            var closestY = dist;

            dist = new Vector2d(closestX - relCenter.x, closestY - relCenter.y).sqMagnitude();
            if (dist > sphere.radius * sphere.radius)
                return true;

            return false;
        }

        public static bool HalfSpaceAndHalfSpace(LineCollider line1, LineCollider line2)
        {
            if (line1.normal.Unit() == -1 * line2.normal.Unit())
            {
                if (line1.lineOffset + line2.lineOffset < 0)
                    return false;
            }

            return true;
        }
        
        class Segment1d
        {
            public readonly float x1;
            public readonly float x2;

            public Segment1d(float x1, float x2)
            {
                this.x1 = x1;
                this.x2 = x2;
            }
        }

        static Segment1d transfer(BoxCollider box, Vector2d line)
        {
            float min = float.MaxValue;
            float max = float.MinValue;

            var vs = box.GetVertexes();
            foreach(var v in vs)
            {
                var x = v * line;
                if (x < min)
                    min = x;
                if (x > max)
                    max = x;
            }

            return new Segment1d(min, max);
        }

        static bool isIntercrossing(Segment1d seg1, Segment1d seg2)
        {
            if (seg1.x1 > seg2.x2)
                return false;
            if (seg1.x2 < seg2.x1)
                return false;

            return true;
        }

        public static bool BoxAndBox(BoxCollider box1, BoxCollider box2)
        {
            var ls = box1.GetLines();
            foreach (var line in ls)
            {
                var transfer1 = transfer(box1, line);
                var transfer2 = transfer(box2, line);

                if (!isIntercrossing(transfer1, transfer2))
                    return false;
            }

            var ls2 = box2.GetLines();
            foreach (var line in ls2)
            {
                var transfer1 = transfer(box1, line);
                var transfer2 = transfer(box2, line);

                if (!isIntercrossing(transfer1, transfer2))
                    return false;
            }

            return true;
        }
    }

    public class CircleCollider : Collider
    {
        public float radius { get; }

        public CircleCollider(GameObject owner, float x, float y, float radius)
            : base(owner)
        {
            offset = new Vector2d(x, y);
            this.radius = radius;
        }

        public override bool IsCollidingTo(Collider col)
        {
            return col.IsCollidingTo(this);
        }

        public override bool IsCollidingTo(CircleCollider col)
        {
            return CollisionDetector.CircleAndCircle(col, this);
        }

        public override bool IsCollidingTo(LineCollider col)
        {
            return CollisionDetector.CircleAndHaflSpace(this, col);
        }

        public override bool IsCollidingTo(BoxCollider col)
        {
            return CollisionDetector.BoxAndCircle(col, this);
        }
    }

    public class LineCollider : Collider
    {
        public Vector2d normal { get; }
        public float lineOffset { get; }

        public LineCollider(float x, float y, float lineOffset)
            : base(null)
        {
            normal = (new Vector2d(x, y)).Unit();
            this.lineOffset = lineOffset;
        }

        public override bool IsCollidingTo(Collider col)
        {
            return col.IsCollidingTo(this);
        }

        public override bool IsCollidingTo(CircleCollider col)
        {
            return CollisionDetector.CircleAndHaflSpace(col, this);
        }

        public override bool IsCollidingTo(LineCollider col)
        {
            return CollisionDetector.HalfSpaceAndHalfSpace(col, this);
        }

        public override bool IsCollidingTo(BoxCollider col)
        {
            return CollisionDetector.BoxAndHalfSpace(col, this);
        }
    }

    // rectangle
    public class BoxCollider : Collider
    {
        public Vector2d halfSize;

        public BoxCollider(GameObject owner, Vector2d halfSize, Vector2d offset) : base(owner, offset)
        {
        }

        public Vector2d[] GetVertexes()
        {
            Vector2d[] vs = new Vector2d[4];
            vs[0] = position + new Vector2d(halfSize.x, 0).Rotate(rotation);
            vs[1] = position + new Vector2d(0, halfSize.y).Rotate(rotation);
            vs[2] = position + new Vector2d(-halfSize.x, 0).Rotate(rotation);
            vs[3] = position + new Vector2d(0, -halfSize.y).Rotate(rotation);
            return vs;
        }

        public Vector2d[] GetLines()
        {
            Vector2d[] lines = new Vector2d[4];
            var vs = GetVertexes();
            lines[0] = (vs[0] - vs[1]);
            lines[1] = (vs[1] - vs[2]);
            lines[2] = (vs[2] - vs[3]);
            lines[3] = (vs[3] - vs[0]);
            return lines;
        }

        public override bool IsCollidingTo(Collider col)
        {
            return col.IsCollidingTo(this);
        }

        public override bool IsCollidingTo(LineCollider col)
        {
            return CollisionDetector.BoxAndHalfSpace(this, col);
        }

        public override bool IsCollidingTo(BoxCollider col)
        {
            return CollisionDetector.BoxAndBox(this, col);
        }

        public override bool IsCollidingTo(CircleCollider col)
        {
            return CollisionDetector.BoxAndCircle(this, col);
        }
    }

    public class Collision
    {
        public Collider[] colliders { get; } = new Collider[2];
        public Collision(Collider s1, Collider s2)
        {
            colliders[0] = s1;
            colliders[1] = s2;
        }
    }

    public class ColliderWorld
    {
        HashSet<Collider> colliders { get; } = new HashSet<Collider>();
        List<Collision> collisions;

        public bool Add(Collider col)
        {
            return colliders.Add(col);
        }

        public bool Remove(Collider col)
        {
            return colliders.Remove(col);
        }

        public void CheckCollision()
        {
            collisions = new List<Collision>();

            foreach (var col1 in colliders)
            {
                foreach (var col2 in colliders)
                {
                    if (col1 == col2)
                        continue;

                    if (col1.IsCollidingTo(col2))
                    {
                        collisions.Add(new Collision(col1, col2));
                    }
                }
            }
        }

        public void ResolveCollisions()
        {
            foreach (var col in collisions)
            {
                col.colliders[0].OnColliding(col.colliders[1]);
                col.colliders[1].OnColliding(col.colliders[0]);
            }
        }
    }
}
