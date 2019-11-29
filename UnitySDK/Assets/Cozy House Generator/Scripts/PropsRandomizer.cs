using System;
using UnityEngine;
using Random = System.Random;

namespace Cozy_House_Generator.Scripts
{   ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>  Makes props more diverse  </summary>
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////// 
    public class PropsRandomizer : MonoBehaviour
    {
        public int seed;

        
        /////////////////////////////////////////////////////////////////////
        /// <summary>  Customizes the props with a random seed  </summary>
        //////////////////////////////////////////////////////////////////// 
        public void CastomizeWithRandomSeedButton()
        {
            seed = (int) DateTime.Now.Ticks & 0x0000FFFF;
            CastomizeButton(seed);
        }

        
        ////////////////////////////////////////////////////////////////
        /// <summary>  Customizes the props with a seed  </summary>
        /// 
        /// <param name="seed">  The SEED that will be used  </param>
        ///////////////////////////////////////////////////////////////
        public void CastomizeButton(int seed)
        {
            var rnd = new Random(seed);
            Castomize(rnd); 
        }
                         
        
        ///////////////////////////////////////////////////////////////////////////////////////////////////////        
        /// <summary>  Disables or random rotate some parts of props  </summary>
        /// 
        /// <param name="rnd">                   The standard .NET random that will use             </param>
        /// <param name="resetBeforeCastomize">  Use it if you want to customize from unity editor  </param>
        //////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Castomize(Random rnd , bool resetBeforeCastomize = true)
        {
            if(resetBeforeCastomize)
                Reset();
            
            foreach (Transform child in transform)
            {
                var subProps = child.GetComponent<SubProps>();
                if (subProps == null)
                    continue;

                subProps.Randomize(rnd);
            }
        }

        
        public void Reset()
        {
            foreach (Transform child in transform)
            {
                var subProps = child.GetComponent<SubProps>();
                if (subProps == null)
                    continue;

                subProps.gameObject.SetActive(true);

                if (subProps.addRandomPositionOffset)
                    subProps.ResetPosition();

                if (subProps.addRandomRotationOffset)
                    subProps.ResetRotation();
            }
        }
    }
}