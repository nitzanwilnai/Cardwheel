using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace CommonTools
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Vec2
    {
        public float x;
        public float y;

        public Vec2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public Vec2(Vector3 v)
        {
            this.x = v.x;
            this.y = v.y;
        }

        public Vec2(Vector2 v)
        {
            this.x = v.x;
            this.y = v.y;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return (this.x == ((Vec2)obj).x && this.y == ((Vec2)obj).y);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(Vec2 a, Vec2 b)
        {
            return (a.x == b.x && a.y == b.y);
        }

        public static bool operator !=(Vec2 a, Vec2 b)
        {
            return !(a.x == b.x && a.y == b.y);
        }

        public static Vec2 operator +(Vec2 a, Vec2 b)
        => new Vec2(a.x + b.x, a.y + b.y);

        public static Vec2 operator -(Vec2 a, Vec2 b)
        => new Vec2(a.x - b.x, a.y - b.y);

        public static Vec2 operator -(Vec2 a)
        => new Vec2(-a.x, -a.y);

        public static Vec2 operator *(Vec2 a, float b)
        => new Vec2(a.x * b, a.y * b);


        public static Vector3 ToVector3(Vec2 a)
        {
            return new Vector3(a.x, a.y, 0.0f);
        }

        public static float Distance(Vec2 a, Vec2 b)
        {
            float xDiff = (b.x - a.x);
            float yDiff = (b.y - a.y);
            return Mathf.Sqrt(xDiff * xDiff + yDiff * yDiff);
        }

        public static float DistanceSquare(Vec2 a, Vec2 b)
        {
            float xDiff = b.x - a.x;
            float yDiff = b.y - a.y;
            return xDiff * xDiff + yDiff * yDiff;
        }

        public float Magnitude()
        {
            return Mathf.Sqrt(x * x + y * y);
        }

        public void Normalize()
        {
            float m = Magnitude();
            if (m > 0)
            {
                x /= m;
                y /= m;
            }
            else
            {
                x = 0;
                y = 0;
            }
        }

        public Vec2 Normal()
        {
            float m = Magnitude();
            Vec2 a = new Vec2(x, y);
            if (m > 0)
            {
                a.x /= m;
                a.y /= m;
            }
            else
            {
                a.x = 0;
                a.y = 0;
            }
            return a;
        }

        public static Vec2 Zero()
        {
            return new Vec2(0.0f, 0.0f);
        }

        public static Vec2 One()
        {
            return new Vec2(1.0f, 1.0f);
        }

        private const double DegToRad = Math.PI / 180.0d;
        private const double RadToDeg = 180.0d / Math.PI;

        public static double Angle(Vec2 a, Vec2 b)
        {
            double sin = a.x * b.y - b.x * a.y;
            double cos = a.x * b.x + a.y * b.y;

            return Math.Atan2(sin, cos) * RadToDeg;
        }

        public void Rotate(double degrees)
        {
            double radians = degrees * DegToRad;
            double ca = Math.Cos(radians);
            double sa = Math.Sin(radians);
            x = (float)(ca * x - sa * y);
            y = (float)(sa * x + ca * y);
        }
    }
}