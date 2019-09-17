using System;
using System.Collections.Generic;

namespace Blocksworld
{
    public class SimpleObject : BaseObject
    {

        public int minx = 0;
        public int maxx = 2;
        public int miny = 0;
        public int maxy = 2;

        public BlocksWorldSensoriMotorSystem getSMS()
        {
            return sms;
        }
        public String name;

        public Vec2 pos = new Vec2();

        private BlocksWorldSensoriMotorSystem sms;

        public SimpleObject(BlocksWorldSensoriMotorSystem s, String name, int color, String texture, int x, int y)
        {
            this.name = name;
            this.sms = s;
            this.color = color;
            this.texture = texture;
            pos.x = x;
            pos.y = y;
        }

        public SimpleObject(BlocksWorldSensoriMotorSystem s, String name, int color, String texture, String shape, int x, int y)
        {
            this.name = name;
            this.sms = s;
            this.color = color;
            this.texture = texture;
            this.shape = shape;
            pos.x = x;
            pos.y = y;
        }


        public void setLimit(int minx, int miny, int maxx, int maxy)
        {
            this.minx = minx;
            this.maxx = maxx;
            this.miny = miny;
            this.maxy = maxy;
        }


        override public Vec2 getPosition()
        {
            return pos;
        }

        public Vec2 setPosition(int x, int y)
        {
            pos.x = x;
            pos.y = y;
            return pos;
        }


        public Vec2 setPosition(Vec2 p)
        {
            pos = p;
            return pos;
        }


        public Vec2 remove()
        {
            pos.x = -1000;
            pos.y = -1000;
            return pos;
        }
        public String toString()
        {
            return $"{name}@({pos.x},{pos.y})";
        }

        public virtual Dictionary<String, Object> toMap()
        {
            Dictionary<String, Object> obj = new Dictionary<String, Object>();
            obj.Add("name", name);
            obj.Add("x", pos.x);
            obj.Add("y", pos.y);
            String c = $"rgb({(color >> 16) & 0xFF},{(color >> 8) & 0xFF},{color & 0xFF})";
            obj.Add("color", c);
            obj.Add("shape", shape);
            obj.Add("texture", texture);
            return obj;
        }

    }


}
