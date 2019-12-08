using System;
using UnityEngine;
using Random = System.Random;

namespace Cozy_House_Generator.Scripts
{   ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>  The addition part of props (like food on a table or alarm at a nightstand)  </summary>
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////// 
    [Serializable]
    public class SubProps : MonoBehaviour
    {
        public Quaternion  defaultRotation;
        public Vector3     defaultPosition;
        public int         chanceToPlace                = 50;
        public bool        addRandomRotationOffset;
        public int         minRandomRotation;
        public int         maxRandomRotation;
        public bool        addRandomPositionOffset;
        public int         minXPosOffsetCm;
        public int         maxXPosOffsetCm;
        public int         minZPosOffsetCm;
        public int         maxZPosOffsetCm;

        ///////////////////////////////////////////////////
        /// <summary>  Sets default position  </summary>
        //////////////////////////////////////////////////
        public void ResetPosition()
        {
            transform.localPosition = defaultPosition;
        }
        
        
        ///////////////////////////////////////////////////
        /// <summary>  Sets default rotation  </summary>
        ////////////////////////////////////////////////// 
        public void ResetRotation()
        {
            transform.localRotation = defaultRotation;
        }

        ///////////////////////////////////////////////////////////////////////////
        /// <summary>  Sets current position and rotation as default  </summary>
        //////////////////////////////////////////////////////////////////////////
        public void SetCurrentTransformAsDefault()
        {
            defaultPosition = transform.localPosition;
            defaultRotation = transform.localRotation;
        }

        
        ////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>  Disables or enables sub props and adds random rotation and position offset  </summary>
        /// 
        /// <param name="rnd">  The standard .NET random that will use  </param>
        //////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Randomize(Random rnd)
        {

            gameObject.SetActive(false);
            if (rnd.Next(0, 100) >= chanceToPlace) return;
            gameObject.SetActive(true);
            
            if (addRandomPositionOffset)
            {
                var pos = transform.localPosition;
                
                pos = new Vector3(
                    pos.x + (float) rnd.Next(minXPosOffsetCm, maxXPosOffsetCm) / 100,
                    pos.y,
                    pos.z + (float) rnd.Next(minZPosOffsetCm, maxZPosOffsetCm) / 100);
                transform.localPosition = pos;
            }

            if (addRandomRotationOffset)
            {
                transform.localRotation = Quaternion.Euler(defaultRotation.eulerAngles.x,
                    defaultRotation.eulerAngles.y + rnd.Next(minRandomRotation, maxRandomRotation),
                    defaultRotation.eulerAngles.z);
                    
            }
        }
    }
    
}