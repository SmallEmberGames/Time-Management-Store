using UnityEditor;
using UnityEngine;

public class LevelWindow : EditorWindow
{

    [MenuItem("Window/TEST")]
    public static void ShowWindow()
    {
        EditorWindow editorWindow = GetWindow<LevelWindow>();
        editorWindow.titleContent = new GUIContent("LevelWindow");

        //Limit size of window
        editorWindow.minSize = new Vector2(450, 200);
        editorWindow.maxSize = new Vector2(1920, 720);
    }

    public void OnGUI()
    {
        if (!CheckIfExists())
        {
            return;
        }


    }

    private bool CheckIfExists()
    {
        if (!AssetDatabase.IsValidFolder("Assets/TestFolder"))
        {
            if (GUI.Button(new Rect(10, 10, 50, 50), "Click"))
            {
                CreateFolder();
                CreateScriptableObject();
            }
            return false;
        }
        else
        {
            //Debug.Log($"Asset exists {AssetDatabase.AssetPathExists("Assets/TestFolder/LevelData.asset")}");
            if (!AssetDatabase.AssetPathExists("Assets/TestFolder/LevelData.asset")) //Check if we have the levelData
            {
                if (GUI.Button(new Rect(10, 10, 50, 50), "Click"))
                {
                    CreateScriptableObject();
                }
                return false;
            }
        }

        return true;
    }

    private void CreateFolder()
    {
        AssetDatabase.CreateFolder("Assets", "TestFolder");
    }

    private void CreateScriptableObject()
    {
        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<LevelData>(), "Assets/TestFolder/LevelData.asset");
        AssetDatabase.SaveAssets();
    }
}
