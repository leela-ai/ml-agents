using System;

namespace Cozy_House_Generator.Scripts.Core
{
    ////////////////////////////////////////////////////////
    /// <summary>  Generates some random data  </summary>
    ///////////////////////////////////////////////////////
    public static class RND
    {	/////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>  Returns a random list of bool  </summary>
        /// 
        /// <param name="rnd">         The standard .NET random that will use                 </param>
        /// <param name="sideSize">    Side size of the grid that you want to get in the end  </param>
        /// <param name="randomizer">  How random must result from 0 to 255                   </param>
        /// 
        /// <returns>  Random list of bools  </returns>
        /////////////////////////////////////////////////////////////////////////////////////////////////
        public static bool[] Next (Random rnd, int sideSize, byte randomizer)
        {
            int length = sideSize * sideSize;
            var bufer  = new byte[length];
            var result = new bool[length];

            rnd.NextBytes (bufer);

            for (int i = 0; i < length; i++)
                result[i] = bufer[i] < randomizer;

            return result;
        }


        public static int Seed()
        {
            return (int) DateTime.Now.Ticks & 0x0000FFFF;
        }
    }
}