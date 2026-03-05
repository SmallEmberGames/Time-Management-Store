using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;

public class LevelManager : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset _uxml;

    [SerializeField]
    private VisualTreeAsset _uxmlLevel;

    private LevelManager _window;

    private int _currentChapterIndex;
    private int _previousChapterIndex;
    private ChapterData _chapterData;
    private SerializedObject _serializedConfigs;

    private Label _title;
    private TextField _chapterName;
    private ListView _levels;
    private Button _addButton;
    private Button _setNewChapterName;

    public static void OpenWithData(ChapterData data, int index)
    {
        LevelManager window = GetWindow<LevelManager>("Level Manager", typeof(GameManager));
        window.minSize = new Vector2(450, 200);
        window._chapterData = data;
        window._currentChapterIndex = index;
        window.Show();
        window._window = window;
    }

    protected void Update()
    {
        if (_currentChapterIndex != _previousChapterIndex)
        {
            _previousChapterIndex = _currentChapterIndex;
            _window.Repaint();
            SetData();
            return;
        }
    }

    private void CreateGUI()
    {
        if (_uxml == null)
        {
            Debug.LogWarning($"UXML not assigned to {GetType().Name}");
            return;
        }

        _uxml.CloneTree(rootVisualElement);

        _title = rootVisualElement.Q<Label>("Title");
        _levels = rootVisualElement.Q<ListView>("Levels");
        _addButton = rootVisualElement.Q<Button>("AddLevel");

        _chapterName = rootVisualElement.Q<TextField>("chapterName");
        _setNewChapterName = rootVisualElement.Q<Button>("setNewChapterName");

        SetData();
    }

    private void SetData()
    {
        if (_chapterData == null)
        {
            return;
        }

        _serializedConfigs = new SerializedObject(_chapterData);
        rootVisualElement.Bind(_serializedConfigs);

        _title.text = _chapterData.name;

        _setNewChapterName.clicked += () =>
        {
            string newName = $"{_currentChapterIndex}_{_chapterName.value}";

            if (newName == "" || newName == _chapterData.name)
            {
                return;
            }

            //Set new name
            ChapterData newChapterData = GameManager.ChangeChapterName(_chapterData, newName);
            _chapterData = newChapterData;
            _window.Repaint();
            SetData();
            GameManager.UpdateChapters();
        };

        Levels();
        RegisterChangeTracking();
        _addButton.clicked += AddLevel;
    }

    private void AddLevel()
    {
        _addButton.SetEnabled(false);
        GameManager.AddLevel(_chapterData);
    }

    private void Levels()
    {
        List<LevelData> levels = _chapterData.levels;

        // The ListView calls this to add visible items to the scroller.
        Func<VisualElement> makeItem = () =>
        {
            var chapterInfoVisualElement = new VisualElement();

            if (_uxmlLevel != null)
            {
                _uxmlLevel.CloneTree(chapterInfoVisualElement);
            }
            else
            {
                Debug.LogWarning($"Chapter UXML not assigned to {GetType().Name}");
            }

            return chapterInfoVisualElement;
        };

        Action<VisualElement, int> bindItem = (e, i) => BindItem(e, i);

        _levels.Clear();
        _levels.fixedItemHeight = 100;
        _levels.makeItem = makeItem;
        _levels.bindItem = bindItem;
        _levels.itemsSource = levels;
        _levels.selectionType = SelectionType.Multiple;

        _addButton.SetEnabled(true);
    }

    private void BindItem(VisualElement elem, int i)
    {
        var label = elem.Q<Label>(name: "nameLabel");
        var button = elem.Q<Button>(name: "button");
        var deleteButton = elem.Q<Button>(name: "delete");

        LevelData levelData = _chapterData.levels[i];
        label.text = levelData.name;
        button.text = $"Press??";

        button.clicked += () =>
        {
            //LevelManager.OpenWithData(chapterData);
        };

        deleteButton.clicked += () =>
        {
            bool wantToDelete = EditorUtility.DisplayDialog($"Remove {levelData.name}",
                $"Are you sure you want to remove {levelData.name}", "Agree", "Cancel");

            if (wantToDelete)
            {
                GameManager.RemoveLevel(_chapterData, i);
            }
            /* EditorPopup.Open($"Remove {chapterData.name}",
                 $"Are you sure you want to remove {chapterData.name}", i);*/
        };
    }

    private void RegisterChangeTracking()
    {
        rootVisualElement.Unbind();

        var _serializedConfigs = new SerializedObject(_chapterData);
        rootVisualElement.TrackSerializedObjectValue(_serializedConfigs, _ =>
        {
            // New chapter could be added
            Levels();
            _levels.Rebuild();
            GameManager.UpdateChapters();
        });
    }
}
