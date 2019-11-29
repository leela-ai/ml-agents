using System;
using Cozy_House_Generator.Scripts.Core.DataTypes;
using Cozy_House_Generator.Scripts.House_Builders;
using UnityEngine;
using Random = System.Random;

namespace Cozy_House_Generator.Scripts
{
    [Serializable]
    public class Decorator : MonoBehaviour
    {
        [HideInInspector] public UseOnFloorResolver    useResolver     = new UseOnFloorResolver();
                          public DecoratorMaterialType materialSource;
        [Range(0, 100)]   public int                   chanceToBuild   = 100;

        
        
        public void Build(BuilderData buildData)
        {
            gameObject.SetActive(true);
            
            Renderer rend = GetComponent<MeshRenderer>();
            if (rend == null)
                return;

            switch (materialSource)
            {
                case DecoratorMaterialType.Sample1:
                    if (buildData.decoratorSample1 == null)
                        break;
                    rend.sharedMaterial = buildData.decoratorSample1;
                    break;
                
                case DecoratorMaterialType.Sample2:
                    if (buildData.decoratorSample2 == null)
                        break;
                    rend.sharedMaterial = buildData.decoratorSample2;
                    break;
                
                case DecoratorMaterialType.Sample3:
                    if (buildData.decoratorSample3 == null)
                        break;
                    rend.sharedMaterial = buildData.decoratorSample3;
                    break;
            }
        }
        

        public bool IsCanBeBuilded(Random rnd, int currentFloor, int floorsCount)
        {
            if (useResolver.CanUseOnFloor(currentFloor, floorsCount) == false)
                return false;
            
            if (chanceToBuild == 100)
                return true;

            if (chanceToBuild == 0)
                return false;
            
            return chanceToBuild > rnd.Next(0, 100);
        }

    }

    public enum DecoratorMaterialType
    {
        Own,
        Sample1,
        Sample2,
        Sample3
    }
}