using Cozy_House_Generator.Scripts;
using UnityEditor;

namespace Cozy_House_Generator.Editor.CustomInspector
{
    [CustomEditor(typeof(Decorator))]
    public class DecoratorEditor : UnityEditor.Editor
    {
        private Decorator tar;
        
        private void OnEnable()
        {
            tar = (Decorator) target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            UseOnFloorResolverDrawer.Draw(tar.useResolver);
        }
    }
}