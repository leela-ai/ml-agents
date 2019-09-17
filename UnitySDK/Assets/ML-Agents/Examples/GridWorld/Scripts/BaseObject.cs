using System;

namespace Blocksworld
{
    abstract public class BaseObject
    {

        abstract public Vec2 getPosition();

        public int color;
        public String texture;
        public float w, h;
        public String shape;


    }
}
