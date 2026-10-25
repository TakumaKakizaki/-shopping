using UnityEditor;
using UnityEngine;

/// <summary>実行時にはパスを保存し、編集時には SceneAsset を直接選択します。</summary>
[CustomPropertyDrawer(typeof(SceneAssetPathAttribute))]
public sealed class SceneAssetPathDrawer : PropertyDrawer
{
    private const float WarningHeight = 42f;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        Rect fieldRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        SceneAsset currentScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(property.stringValue);
        EditorGUI.showMixedValue = property.hasMultipleDifferentValues;
        EditorGUI.BeginChangeCheck();
        SceneAsset selectedScene = (SceneAsset)EditorGUI.ObjectField(fieldRect, label, currentScene, typeof(SceneAsset), false);
        if (EditorGUI.EndChangeCheck())
        {
            property.stringValue = selectedScene == null ? string.Empty : AssetDatabase.GetAssetPath(selectedScene);
        }
        EditorGUI.showMixedValue = false;

        string warning = GetWarning(property);
        if (warning != null)
        {
            Rect warningRect = new Rect(position.x, fieldRect.yMax + EditorGUIUtility.standardVerticalSpacing,
                position.width, WarningHeight);
            EditorGUI.HelpBox(warningRect, warning, MessageType.Warning);
        }
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight + (GetWarning(property) == null
            ? 0f : EditorGUIUtility.standardVerticalSpacing + WarningHeight);
    }

    private static string GetWarning(SerializedProperty property)
    {
        if (property.hasMultipleDifferentValues) return null;
        string path = property.stringValue;
        if (string.IsNullOrEmpty(path)) return "Select a scene asset to start the game.";
        if (AssetDatabase.LoadAssetAtPath<SceneAsset>(path) == null)
            return "The scene asset is missing. Select the target scene again.";
        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            if (scene.enabled && scene.path == path) return null;
        }
        return "Enable this scene in Build Profiles > Scene List before starting the game.";
    }
}
