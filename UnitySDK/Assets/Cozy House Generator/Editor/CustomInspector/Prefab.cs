using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace Cozy_House_Generator.Editor.CustomInspector
{
    public static class Prefab
    {
        public static void MarkDirty()
        {
#if UNITY_EDITOR

            if (EditorApplication.isPlaying == false)
            {

                var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
                EditorSceneManager.MarkSceneDirty(prefabStage != null
                                                      ? prefabStage.scene
                                                      : SceneManager.GetActiveScene());
            }
#endif
        }

        public static void SetValue<T>(ref T target, T value)
        {
            if (target == null && value == null)
                return;
            
            if (target == null && value != null || target != null && value == null || target.Equals(value) == false)
            {
                target = value;
                MarkDirty();
            }
        }
    }
}