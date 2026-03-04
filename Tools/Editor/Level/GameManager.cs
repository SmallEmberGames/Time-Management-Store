using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : EditorWindow
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

    private void AddChapter()
    {
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
            _chapters.Rebuild();
        });
    }

    private void BindItem(ChapterInfoVisualElement elem, int i)
    {
        var label = elem.Q<Label>(name: "nameLabel");
        var button = elem.Q<Button>();

        ChapterData chapterData = _gameData.chapters[i];
        label.text = chapterData.name;
        button.text = $"Levels: {chapterData.levels.Count}";

        button.clicked += () =>
        {
            //LevelManager.Open();
            LevelManager.OpenWithData(chapterData);
        };
    }

    private void Chapters()
    {
        List<ChapterData> chapters = _gameData.chapters;

        // The ListView calls this to add visible items to the scroller.
        Func<VisualElement> makeItem = () =>
        {
            var chapterInfoVisualElement = new ChapterInfoVisualElement();

            var label = chapterInfoVisualElement.Q<Label>(name: "header Label");

            return chapterInfoVisualElement;
        };

        Action<VisualElement, int> bindItem = (e, i) => BindItem(e as ChapterInfoVisualElement, i);

        _chapters.fixedItemHeight = 55;
        _chapters.makeItem = makeItem;
        _chapters.bindItem = bindItem;
        _chapters.itemsSource = chapters;
        _chapters.selectionType = SelectionType.Multiple;
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

    public class ChapterInfoVisualElement : VisualElement
    {
        public ChapterInfoVisualElement()
        {
            var root = new VisualElement();

            root.style.paddingTop = 3f;
            root.style.paddingRight = 0f;
            root.style.paddingBottom = 15f;
            root.style.paddingLeft = 3f;
            root.style.borderBottomColor = Color.gray;
            root.style.borderBottomWidth = 1f;
            root.style.position = Position.Relative;
            var nameLabel = new Label() { name = "nameLabel" };
            nameLabel.style.fontSize = 14f;
            var levelContainer = new Button();
            levelContainer.style.flexDirection = FlexDirection.Row;
            levelContainer.style.paddingLeft = 15f;
            levelContainer.style.paddingRight = 15f;
            levelContainer.style.paddingTop = 1f;
            levelContainer.text = "Levels";

            root.Add(nameLabel);
            root.Add(levelContainer);
            Add(root);
        }
    }
}
