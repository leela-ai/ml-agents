using UnityEditor;
using UnityEngine;

namespace Cozy_House_Generator.Editor.CustomInspector
{
    public class TextStyle
    {
        public readonly GUIStyle headerMontserrat           = new GUIStyle();
        public readonly GUIStyle smallHeaderMontserrat      = new GUIStyle();
        public readonly GUIStyle nanoHeaderMontserrat      = new GUIStyle();
        public readonly GUIStyle smallWhiteTextMontserrat   = new GUIStyle();
        public readonly GUIStyle smallBlackTextMontserrat   = new GUIStyle();
        public readonly GUIStyle smallGreenTextRegular      = new GUIStyle();
        
        private Font     fontMontserrat;
        
        public void Initialize()
        {
            fontMontserrat = (Font)AssetDatabase.LoadAssetAtPath("Assets/Cozy House Generator/Editor/Res/Montserrat-Regular.ttf", typeof(Font));

            headerMontserrat.font = fontMontserrat;
            headerMontserrat.fontSize = 32;
            headerMontserrat.normal.textColor = new Color(252, 247, 248);
            headerMontserrat.alignment = TextAnchor.UpperCenter;
            
            smallHeaderMontserrat.font = fontMontserrat;
            smallHeaderMontserrat.fontSize = 20;
            smallHeaderMontserrat.normal.textColor = new Color(252, 247, 248);
            smallHeaderMontserrat.alignment = TextAnchor.UpperCenter;
            
            nanoHeaderMontserrat.font = fontMontserrat;
            nanoHeaderMontserrat.fontSize = 12;
            nanoHeaderMontserrat.normal.textColor = new Color(252, 247, 248);
            nanoHeaderMontserrat.alignment = TextAnchor.UpperCenter;

            smallWhiteTextMontserrat.font = fontMontserrat;
            smallWhiteTextMontserrat.fontSize = 12;
            smallWhiteTextMontserrat.normal.textColor = new Color(252, 247, 248);
            smallWhiteTextMontserrat.alignment = TextAnchor.MiddleCenter;
            
            smallBlackTextMontserrat.font = fontMontserrat;
            smallBlackTextMontserrat.fontSize = 12;
            smallBlackTextMontserrat.normal.textColor = Color.black;
            smallBlackTextMontserrat.alignment = TextAnchor.MiddleCenter;
            
            smallGreenTextRegular.normal.textColor = Color.green;
        }
    }
}