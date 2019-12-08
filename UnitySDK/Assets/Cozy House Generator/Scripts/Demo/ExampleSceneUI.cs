using Cozy_House_Generator.Scripts.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Cozy_House_Generator.Scripts.Demo
{
    public class ExampleSceneUI : MonoBehaviour
    {
        public InputField     seedLabel;
        public HouseGenerator[] houses;
        //public Toggle         hideCeiling;
        public Slider clipping;
        public Camera mainCamera;
        
        

        private void Start()
        {
            houses = FindObjectsOfType<HouseGenerator>();
            clipping.onValueChanged.AddListener(arg0 => { mainCamera.nearClipPlane = clipping.value;});
        }


        public void GenerateRandom()
        {
            int seed = RND.Seed();

            foreach (var house in houses)
                house.GenerateWithSeed(seed);
            
            seedLabel.text = seed.ToString();
        }

        public void GenerateFromSeed()
        {
            int.TryParse(seedLabel.text, out var seed);
            foreach (var house in houses)
            {
                house.GenerateWithSeed(seed);
            }
        }
    }
}