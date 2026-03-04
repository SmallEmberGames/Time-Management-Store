using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class MenuItems
{
    [MenuItem("Tools/Restaurant/ChapterManager")]
    private static void NewChapterManager()
    {
        GameManager.Open();
    }
}
