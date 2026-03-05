using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset _uxml;

    [SerializeField]
    private VisualTreeAsset _uxmlChapter;

    private static string headFolderName = "TestFolder";
    private static string chaptersFolderName = "Chapters";
    private static string gameDataName = "GameData.asset";
    private static string chapterName = "Chapter";
    private static string levelName = "Level";

    private Button _createButton;
    private Button _addButton;
    private ListView _chapters;

    private GameData _gameData;

    public static void Open()
    {
        EditorWindow window = GetWindow<GameManager>();
        window.titleContent = new GUIContent("Game Manager");

        //Limit size of window
        window.minSize = new Vector2(450, 200);
    }

    private void CreateGUI()
    {
        if (_uxml == null)
        {
            Debug.LogWarning($"UXML not assigned to {GetType().Name}");
            return;
        }

        _uxml.CloneTree(rootVisualElement);

        _createButton = rootVisualElement.Q<Button>("CreateDataButton");
        _addButton = rootVisualElement.Q<Button>("AddChapter");
        _chapters = rootVisualElement.Q<ListView>("Chapters");

        SetData();
    }

    private void SetData()
    {
        bool hasGameData = CheckIfExists(out _gameData);
        SetButtons(hasGameData);

        if (hasGameData)
        {
            Chapters();
            RegisterChangeTracking();
        }
    }

    private void SetButtons(bool exists)
    {
        #region Create Button
        _createButton.visible = !exists;
        _createButton.style.display = !exists ? DisplayStyle.Flex : DisplayStyle.None;
        _createButton.clicked += CreateOnClick;
        #endregion

        #region Add Button
        _addButton.visible = exists;
        _addButton.clicked += AddChapter;
        #endregion

        _chapters.visible = exists;
    }

    #region ADD
    private void AddChapter()
    {
        //Create new chapter and add it to the list in gamedata
        int newChapterIndex = _gameData.chapters.Count;
        string chapterPath = $"Assets/{headFolderName}/{chaptersFolderName}/{newChapterIndex}_{chapterName}.asset";
        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<ChapterData>(), chapterPath);
        AssetDatabase.SaveAssets();

        ChapterData newChapter = AssetDatabase.LoadAssetAtPath<ChapterData>(chapterPath);
        _gameData.chapters.Add(newChapter);

        CreateChapterFolder($"{newChapterIndex}_{chapterName}");
    }

    public static void AddLevel(ChapterData chapterData)
    {
        CreateChapterFolder(chapterData.name);

        int newLevelIndex = chapterData.levels.Count + 1;
        string levelPath = $"Assets/{headFolderName}/{chaptersFolderName}/{chapterData.name}/{newLevelIndex}_{levelName}.asset";
        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<LevelData>(), levelPath);
        AssetDatabase.SaveAssets();

        LevelData newLevel = AssetDatabase.LoadAssetAtPath<LevelData>(levelPath);
        chapterData.levels.Add(newLevel);
    }

    private static void CreateChapterFolder(string chapterName)
    {
        //Add folder for the levels to get into
        if (!AssetDatabase.IsValidFolder($"Assets/{headFolderName}/{chaptersFolderName}/{chapterName}"))
        {
            AssetDatabase.CreateFolder($"Assets/{headFolderName}/{chaptersFolderName}", chapterName);
        }
    }
    #endregion

    private void RegisterChangeTracking()
    {
        var _serializedConfigs = new SerializedObject(_gameData);
        rootVisualElement.TrackSerializedObjectValue(_serializedConfigs, _ =>
        {
            ForceUpdateGameManager();
        });
    }

    private void ForceUpdateGameManager()
    {
        Chapters();
        _chapters.Rebuild();
    }

    private void BindItem(VisualElement elem, int i)
    {
        var label = elem.Q<Label>(name: "nameLabel");
        var button = elem.Q<Button>(name: "button");
        var deleteButton = elem.Q<Button>(name: "delete");

        ChapterData chapterData = _gameData.chapters[i];
        if (chapterData == null) //If it has been deleted outside of out tool 
        {
            _gameData.chapters.RemoveAt(i);
            _chapters.Remove(elem);
            return;
        }

        label.text = chapterData.name;
        button.text = $"Levels: {chapterData.levels.Count}";

        button.clicked += () =>
        {
            LevelManager.OpenWithData(chapterData, i);
        };

        deleteButton.clicked += () =>
        {
            bool wantToDelete = EditorUtility.DisplayDialog($"Remove {chapterData.name}",
                $"Are you sure you want to remove {chapterData.name}", "Agree", "Cancel");

            if (wantToDelete)
            {
                Debug.Log($"Want to delete {chapterData.name}");
                RemoveChapter(chapterData, i);
            }
           /* EditorPopup.Open($"Remove {chapterData.name}",
                $"Are you sure you want to remove {chapterData.name}", i);*/
        };
    }

    private void RemoveChapter(ChapterData chapterData, int i)
    {
        //Remove folder
        string folderPath = $"Assets/{headFolderName}/{chaptersFolderName}/{chapterData.name}";
        if (AssetDatabase.IsValidFolder(folderPath))
        {
            AssetDatabase.DeleteAsset(folderPath);
        }

        //Remove object
        if (AssetDatabase.AssetPathExists($"{folderPath}.asset"))
        {
            AssetDatabase.DeleteAsset($"{folderPath}.asset");
        }

        //Remove from list
        _gameData.chapters.RemoveAt(i);

        //Rename all that came before 
        //So if this is 3 we should rename 4 and up
        if (i < _gameData.chapters.Count - 1)
        {
            return;
        }

        for (int c = i; c < _gameData.chapters.Count; c++)
        {
            string name = _gameData.chapters[c].name;
            name = name.Replace($"{c + 1}", $"{c}");
            ChangeChapterName(_gameData.chapters[c], name);
        }
    }

    public static ChapterData ChangeChapterName(ChapterData chapterData, string newName)
    {
        //change folder name
        string folderPath = $"Assets/{headFolderName}/{chaptersFolderName}/{chapterData.name}";
        if (AssetDatabase.IsValidFolder(folderPath))
        {
            AssetDatabase.RenameAsset(folderPath, newName);
        }

        //change object name
        if (AssetDatabase.AssetPathExists($"{folderPath}.asset"))
        {
            AssetDatabase.RenameAsset($"{folderPath}.asset", $"{newName}.asset");
        }

        //Set data
        string newPath = $"Assets/{headFolderName}/{chaptersFolderName}/{newName}.asset";
        ChapterData newChapter = AssetDatabase.LoadAssetAtPath<ChapterData>(newPath);
        return newChapter;
    }

    public static void RemoveLevel(ChapterData chapterData, int i)
    {
        //Remove level object
        string levelAssetPath = $"Assets/{headFolderName}/{chaptersFolderName}/{chapterData.name}/{chapterData.levels[i].name}.asset";
        if (AssetDatabase.AssetPathExists(levelAssetPath))
        {
            AssetDatabase.DeleteAsset(levelAssetPath);
        }

        //Remove from list
        chapterData.levels.RemoveAt(i);
    }

    private void Chapters()
    {
        List<ChapterData> chapters = _gameData.chapters;

        // The ListView calls this to add visible items to the scroller.
        Func<VisualElement> makeItem = () =>
        {
            var chapterInfoVisualElement = new VisualElement();
            if (_uxmlChapter != null)
            {
                _uxmlChapter.CloneTree(chapterInfoVisualElement);
            }
            else
            {
                Debug.LogWarning($"Chapter UXML not assigned to {GetType().Name}");
            }

            return chapterInfoVisualElement;
        };

        Action<VisualElement, int> bindItem = (e, i) => BindItem(e, i);

        _chapters.fixedItemHeight = 100;
        _chapters.makeItem = makeItem;
        _chapters.bindItem = bindItem;
        _chapters.itemsSource = chapters;
        _chapters.selectionType = SelectionType.Multiple;
    }

    public static void UpdateChapters()
    {
        GameManager window = GetWindow<GameManager>();
        window.ForceUpdateGameManager();

    }

    #region Create / check if exists
    private void CreateOnClick()
    {
        if (!AssetDatabase.IsValidFolder($"Assets/{headFolderName}"))
        {
            AssetDatabase.CreateFolder("Assets", headFolderName);
        }
        if (!AssetDatabase.IsValidFolder($"Assets/{headFolderName}/{chaptersFolderName}"))
        {
            AssetDatabase.CreateFolder($"Assets/{headFolderName}", chaptersFolderName);
        }
        if (!AssetDatabase.AssetPathExists($"Assets/{headFolderName}/{gameDataName}"))
        {
            CreateScriptableObject();
        }

        SetData();
    }

    private bool CheckIfExists(out GameData gameData)
    {
        gameData = null;
        bool hasHeadFolder = AssetDatabase.IsValidFolder($"Assets/{headFolderName}");
        bool hasGameDataAsset = AssetDatabase.AssetPathExists($"Assets/{headFolderName}/{gameDataName}");
        bool hasChapterFolder = AssetDatabase.IsValidFolder($"Assets/{headFolderName}/{chaptersFolderName}");

        if (!hasHeadFolder || !hasGameDataAsset || !hasChapterFolder)
        {
            return false;
        }

        gameData = AssetDatabase.LoadAssetAtPath<GameData>($"Assets/{headFolderName}/{gameDataName}");
        return true;
    }

    private void CreateScriptableObject()
    {
        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<GameData>(), $"Assets/{headFolderName}/{gameDataName}");
        AssetDatabase.SaveAssets();
    }
    #endregion
}
