using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace CommonTools
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Vec2Int
    {
        public int x;
        public int y;

        public Vec2Int(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public Vec2Int(Vector3 v)
        {
            this.x = (int)v.x;
            this.y = (int)v.y;
        }

        public Vec2Int(Vector2 v)
        {
            this.x = (int)v.x;
            this.y = (int)v.y;
        }

        public static Vec2Int operator +(Vec2Int a, Vec2Int b)
        => new Vec2Int(a.x + b.x, a.y + b.y);

        public static Vec2Int operator -(Vec2Int a, Vec2Int b)
        => new Vec2Int(a.x - b.x, a.y - b.y);

        public static Vec2Int operator -(Vec2Int a)
        => new Vec2Int(-a.x, -a.y);

        public static Vec2Int operator /(Vec2Int a, int div)
        => new Vec2Int(a.x/div, a.y/div);

        public static Vec2Int operator *(Vec2Int a, int b)
        => new Vec2Int(a.x * b, a.y * b);
        

        public static Vector3 ToVector3(Vec2Int a)
        {
            return new Vector3(a.x, a.y, 0.0f);
        }

        public static float Distance(Vec2Int a, Vec2Int b)
        {
            int xDiff = (b.x - a.x);
            int yDiff = (b.y - a.y);
            return Mathf.Sqrt(xDiff * xDiff + yDiff * yDiff);
        }

        public static float DistanceSquare(Vec2Int a, Vec2Int b)
        {
            int xDiff = b.x - a.x;
            int yDiff = b.y - a.y;
            return xDiff * xDiff + yDiff * yDiff;
        }

        public float Magnitude()
        {
            return Mathf.Sqrt(x * x + y * y);
        }

        public static Vec2Int Zero()
        {
            return new Vec2Int(0, 0);
        }

        public static Vec2Int One()
        {
            return new Vec2Int(1, 1);
        }
    }
}