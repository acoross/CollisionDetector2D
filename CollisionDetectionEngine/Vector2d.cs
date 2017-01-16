using System;

namespace Acoross.Game2d
{
    public class Vector2d
    {
        public readonly float x;
        public readonly float y;

        public Vector2d(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public float sqMagnitude()
        {
            return x * x + y * y;
        }

        public float Magnitude()
        {
            return (float)Math.Sqrt(sqMagnitude());
        }

        public Vector2d Rotate(Vector2d rhs)
        {
            return new Vector2d(x * rhs.x - y * rhs.y, x * rhs.y + y * rhs.x);
        }

        public Vector2d Rotate(float rad)
        {
            return this.Rotate(new Vector2d((float)Math.Cos(rad), (float)Math.Sin(rad)));
        }

        public Vector2d Unit()
        {
            return this * (1 / Magnitude());
        }

        public static Vector2d operator *(Vector2d op1, float op2)
        {
            return new Vector2d(op1.x * op2, op1.y * op2);
        }

        public static Vector2d operator *(float op1, Vector2d op2)
        {
            return op2 * op1;
        }

        public static Vector2d operator /(Vector2d dividend, float divisor)
        {
            if (Math.Abs(divisor) < float.Epsilon)
            {
                throw new DivideByZeroException();
            }

            return new Vector2d(dividend.x / divisor, dividend.y / divisor);
        }

        public static float operator * (Vector2d op1, Vector2d op2)
        {
            return op1.x * op2.x + op1.y * op2.y;
        }

        public static Vector2d operator +(Vector2d op1, Vector2d op2)
        {
            return new Vector2d(op1.x + op2.x, op1.y + op2.y);
        }

        public static Vector2d operator -(Vector2d op1, Vector2d op2)
        {
            return new Vector2d(op1.x - op2.x, op1.y - op2.y);
        }
         
        public static bool operator ==(Vector2d op1, Vector2d op2)
        {
            if (op1.x == op2.x && op1.y == op2.y)
            {
                return true;
            }

            return false;
        }

        public static bool operator !=(Vector2d op1, Vector2d op2)
        {
            return !(op1 == op2);
        }

        public override bool Equals(object op)
        {
            return this == (Vector2d)op;
        }

        public override int GetHashCode()
        {
            return ((int)(x * 73856093) ^ (int)(y * 19349663));
        }

        public static Vector2d Zero = new Vector2d(0, 0);
        public static Vector2d X = new Vector2d(1, 0);
        public static Vector2d Y = new Vector2d(0, 1);
    }
}
