using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class LevelManager : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset _uxml;

    private LevelManager _window;

    private ChapterData _previousChapterData;
    private ChapterData _chapterData;
    private SerializedObject _serializedConfigs;

    private Label _title;

    public static void OpenWithData(ChapterData data)
    {
        LevelManager window = GetWindow<LevelManager>("Level Manager", typeof(GameManager));
        window.minSize = new Vector2(450, 200);
        window._chapterData = data;
        window.Show();
        window._window = window;

        if (data != null)
        {
            Debug.Log($"OpenWithData {data.name}");
        }
    }

    protected void Update()
    {
        if (_chapterData != _previousChapterData)
        {
            _previousChapterData = _chapterData;
            _window.Repaint();
            SetData();
            Debug.Log("TEST");
            return;
        }
    }

    /* public static void Open()
     {
         EditorWindow window = GetWindow<LevelManager>("Level Manager", typeof(ChapterManager));

         //Limit size of window
         window.minSize = new Vector2(450, 200);

         //_chapterData = chapterData;
     }*/

    private void SetData()
    {
        Debug.Log($"HERE {_chapterData == null}");
        if (_chapterData == null)
        {
            return;
        }

        _title.text = _chapterData.name;
        _serializedConfigs = new SerializedObject(_chapterData);
        rootVisualElement.Bind(_serializedConfigs);
    }

    private void CreateGUI()
    {
        Debug.Log($"LevelManager CreateGUI");
        if (_uxml == null)
        {
            Debug.LogWarning($"UXML not assigned to {GetType().Name}");
            return;
        }

        _uxml.CloneTree(rootVisualElement);

        _title = rootVisualElement.Q<Label>("Title");
        SetData();
    }
}
