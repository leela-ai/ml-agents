using System;

namespace Cozy_House_Generator.Scripts.Core
{
    ///////////////////////////////////////////
    /// <summary>  Converts data  </summary>
    //////////////////////////////////////////
    public static class Convert
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>  Converts a list of bool data into the 2D grid  </summary>
        /// 
        /// <remarks>  Note that the length of source bool list must be divisible by 2 without remainder  </remarks>
        /// 
        /// <param name="source">  Bool list that will convert to the bool grid  </param>
        /// 
        /// <returns>  Random grid of bools  </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static bool[,] Bool1D_to_Bool2D (bool[] source)
        {
            int sideSize = (int) Math.Sqrt (source.Length);
            var result   = new bool[sideSize, sideSize];

            for (int x = 0; x < sideSize; x++)
            {
                for (int y = 0; y < sideSize; y++)
                    result[x, y] = source[x * sideSize + y];
            }

            return result;
        }
    }
}