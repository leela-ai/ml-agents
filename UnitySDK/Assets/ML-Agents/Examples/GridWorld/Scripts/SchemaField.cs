using System;
using System.Collections.Generic;

namespace Blocksworld
{
    public class SchemaField
    {

        public int minX;
        public int maxX;
        public int minY;
        public int maxY;

        public SchemaField(int x1, int y1, int x2, int y2)
        {
            minX = x1;
            maxX = x2;
            minY = y1;
            maxY = y2;

        }

    }
}

