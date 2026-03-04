using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class ChapterManager2 : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset _uxml;

    private static string headFolderName = "TestFolder";
    private static string chaptersFolderName = "Chapters";
    private static string gameDataName = "GameData.asset";
    private static string chapterName = "Chapter_";

    private Button _createButton;
    private Button _addButton;
    private ListView _chapters;

    private GameData _gameData;
    private ChapterData _currentChapter;

    public static void Open()
    {
        EditorWindow window = GetWindow<ChapterManager2>();
        window.titleContent = new GUIContent("Chapter Manager");

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
        // bool gameData
        //rootVisualElement.Bind(_serializedConfigs);
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
        _createButton.SetEnabled(!exists);
        _createButton.visible = !exists;
        _createButton.clicked += CreateOnClick;
        #endregion

        #region Add Button
        _addButton.SetEnabled(exists);
        _addButton.visible = exists;
        _addButton.clicked += AddChapter;
        #endregion
    }

    private void AddChapter()
    {
        Debug.Log("Add chapter");
        //Create new chapter and add it to the list in gamedata
        int newChapterIndex = _gameData.chapters.Count + 1;
        string chapterPath = $"Assets/{headFolderName}/{chaptersFolderName}/{chapterName}{newChapterIndex}.asset";
        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<ChapterData>(), chapterPath);
        AssetDatabase.SaveAssets();

        ChapterData newChapter = AssetDatabase.LoadAssetAtPath<ChapterData>(chapterPath);
        _gameData.chapters.Add(newChapter);
    }

    private void RegisterChangeTracking()
    {
        var _serializedConfigs = new SerializedObject(_gameData);
        rootVisualElement.TrackSerializedObjectValue(_serializedConfigs, _ =>
        {
            // New chapter could be added
            Chapters();
        });
    }

    private void Chapters()
    {
        List<ChapterData> chapters = _gameData.chapters;
        var items = new List<string>(chapters.Count);
        for (var i = 0; i < chapters.Count; i++)
        {
            items.Add(chapters[i].name);
        }

        Func<VisualElement> makeItem = () => new Label();

        Action<VisualElement, int> bindItem = (e, i) => ((Label)e).text = items[i];

        var listView = rootVisualElement.Q<ListView>();
        listView.makeItem = makeItem;
        listView.bindItem = bindItem;
        listView.itemsSource = chapters;
        listView.selectionType = SelectionType.Multiple;

        // Callback invoked when the user double clicks an item
        listView.itemsChosen += (selectedItems) =>
        {
           // _currentChapter = (ChapterData)selectedItems;
           // Levels();
            Debug.Log("Items chosen: " + string.Join(", ", selectedItems));
        };

        // Callback invoked when the user changes the selection inside the ListView
        listView.selectedIndicesChanged += (selectedIndices) =>
        {
            Debug.Log("Index selected: " + string.Join(", ", selectedIndices));
            //listView.itemsSource
            // Note: selectedIndices can also be used to get the selected items from the itemsSource directly or
            // by using listView.viewController.GetItemForIndex(index).
        };
    }

    private void Levels()
    {
        if (_currentChapter == null)
        {
            return;
        }

        List<LevelData> levels = _currentChapter.levels;
        var items = new List<string>(levels.Count);
        for (var i = 0; i < levels.Count; i++)
        {
            items.Add(levels[i].name);
        }

        Func<VisualElement> makeItem = () => new Label();

        Action<VisualElement, int> bindItem = (e, i) => ((Label)e).text = items[i];

        var listView = rootVisualElement.Q<ListView>();
        listView.makeItem = makeItem;
        listView.bindItem = bindItem;
        listView.itemsSource = levels;
        listView.selectionType = SelectionType.Multiple;

        // Callback invoked when the user double clicks an item
        listView.itemsChosen += (selectedItems) =>
        {
            Debug.Log("Items chosen: " + string.Join(", ", selectedItems));
        };

        // Callback invoked when the user changes the selection inside the ListView
        listView.selectedIndicesChanged += (selectedIndices) =>
        {
            Debug.Log("Index selected: " + string.Join(", ", selectedIndices));

            // Note: selectedIndices can also be used to get the selected items from the itemsSource directly or
            // by using listView.viewController.GetItemForIndex(index).
        };
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
