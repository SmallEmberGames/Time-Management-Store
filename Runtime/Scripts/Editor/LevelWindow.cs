using UnityEditor;
using UnityEngine;

public class LevelWindow : EditorWindow
{
    private static string headFolderName = "TestFolder";
    private static string chaptersFolderName = "Chapters";
    private static string gameDataName = "GameData.asset";
    private static string chapterName = "Chapter_";


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

        GameData gameData = AssetDatabase.LoadAssetAtPath<GameData>($"Assets/{headFolderName}/{gameDataName}");
        /* GUI.Box(new Rect(10, 150, 100, 100), gameData.chapters.Count.ToString());
         GUI.Box(new Rect(10, 100, 100, 100), gameData.chapters.Count.ToString());*/
        #region Style
        GUIStyle header = new GUIStyle();
        header.fontSize = 20;
        header.fontStyle = FontStyle.Bold;
        header.normal.textColor = Color.white;
        GUI.color = Color.white;
        #endregion

        GUI.Label(new Rect(10, 10, 100, 50), "Chapters", header);
        if (GUI.Button(new Rect(10, 50, 40, 30), "Add"))
        {
            AddChapter(ref gameData);
        }
    }

    #region Chapters
    private void AddChapter(ref GameData gameData)
    {
        Debug.Log("Add chapter");
        //Create new chapter and add it to the list in gamedata
        int newChapterIndex = gameData.chapters.Count + 1;
        string chapterPath = $"Assets/{headFolderName}/{chaptersFolderName}/{chapterName}{newChapterIndex}.asset";
        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<ChapterData>(), chapterPath);
        AssetDatabase.SaveAssets();

        ChapterData newChapter = AssetDatabase.LoadAssetAtPath<ChapterData>(chapterPath);
        gameData.chapters.Add(newChapter);
    }

    /*private void CreateScriptableObject()
        {
            AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<GameData>(), $"Assets/{headFolderName}/{gameDataName}");
            AssetDatabase.SaveAssets();
        }*/

    //Todo Show already made chapters in a list (clickable)
    #endregion

    #region Check if exists
    private bool CheckIfExists()
    {
        if (!AssetDatabase.IsValidFolder($"Assets/{headFolderName}"))
        {
            if (GUI.Button(new Rect(10, 10, 50, 50), "Click"))
            {
                AssetDatabase.CreateFolder("Assets", headFolderName);
                AssetDatabase.CreateFolder($"Assets/{headFolderName}", chaptersFolderName);
                CreateScriptableObject();
            }
            return false;
        }
        else
        {
            bool gameDataAssetExists = AssetDatabase.AssetPathExists($"Assets/{headFolderName}/{gameDataName}");
            bool chapterFolderExists = AssetDatabase.IsValidFolder($"Assets/{headFolderName}/{chaptersFolderName}");

            if (!gameDataAssetExists || !chapterFolderExists)
            {
                if (GUI.Button(new Rect(10, 10, 50, 50), "Click"))
                {
                    if (!gameDataAssetExists)
                    {
                        CreateScriptableObject();
                    }
                    if (!chapterFolderExists)
                    {
                        AssetDatabase.CreateFolder($"Assets/{headFolderName}", chaptersFolderName);
                    }
                }
                return false;
            }
        }

        return true;
    }

    private void CreateScriptableObject()
    {
        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<GameData>(), $"Assets/{headFolderName}/{gameDataName}");
        AssetDatabase.SaveAssets();
    }
    #endregion
}
