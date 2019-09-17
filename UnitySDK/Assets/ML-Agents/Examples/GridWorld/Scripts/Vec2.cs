using System;

namespace Blocksworld
{
    public class Vec2
    {
        public int x;
        public int y;

        public int compareTo(Vec2 o)
        {
            if (o.x == x && o.y == y)
            {
                return 0;
            }
            else
            {
                return -1;
            }
        }

        public bool equals(Vec2 o)
        {
            return (o.x == x && o.y == y);
        }

        public Vec2(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public Vec2(Vec2 p)
        {
            this.x = p.x;
            this.y = p.y;
        }

        public Vec2()
        {
            this.x = 1;
            this.y = 1;
        }

        public Vec2 copy()
        {
            return new Vec2(x, y);
        }

        public void _add(Vec2 a)
        {
            this.x += a.x;
            this.y += a.y;
        }


        public Vec2 Add(Vec2 a)
        {
            Vec2 n = new Vec2(x, y);
            n.x += a.x;
            n.y += a.y;
            return n;
        }

        public String toString()
        {
            return ($"Vec x {x} y {y}");
        }

    }

}
