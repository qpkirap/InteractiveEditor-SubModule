using System.Collections.Generic;
using Module.InteractiveEditor.Configs;
using Module.InteractiveEditor.Editor;
using Module.Utils;
using Module.Utils.Configs;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class EpisodesWindowEditor : EditorWindow
{
    private StoryObject storyObject;
    private ListView listView;
    
    [MenuItem("InteractiveEditor/Episodes")]
    public static void ShowEditor()
    {
        EditorsCache.Init();
        
        EpisodesWindowEditor wnd = GetWindow<EpisodesWindowEditor>();
        wnd.titleContent = new GUIContent("EpisodesWindowEditor");
    }

    public void CreateGUI()
    {
        VisualElement root = rootVisualElement;

        var uxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(Paths.EpisodesWindowUxml);
        var styles = AssetDatabase.LoadAssetAtPath<StyleSheet>(Paths.EpisodesWindowUss);

        uxml.CloneTree(root);
        root.styleSheets.Add(styles);

        listView = root.Q<ListView>();
        
        OnSelectionChange();
    }
    
    private void OnDisable()
    {
        EditorUtility.SetDirty(storyObject);
        
        listView.makeItem = null;
        listView.unbindItem = null;
        listView.bindItem = null;
        listView.itemsSource = null;
        listView.itemsRemoved -= OnRemoved;

        AssetDatabase.SaveAssets();
    }
    
    private void OnSelectionChange()
    {
        var storyObject = Selection.activeObject as StoryObject;

        SelectChange(storyObject);
    }

    private void SelectChange(StoryObject storyObject)
    {
        if (storyObject == null) return;

        this.storyObject = storyObject;
        
        listView.makeItem = null;
        listView.unbindItem = null;
        listView.bindItem = null;
        listView.itemsSource = null;
        listView.itemsRemoved -= OnRemoved;

        listView.showAddRemoveFooter = false;

        if (storyObject == null)
        {
            listView.Clear();
            return;
        }
        
        listView.showAddRemoveFooter = true;

        listView.makeItem = MakeItem;
        listView.bindItem = BindItem;
        listView.unbindItem = UnbindItem;
        listView.itemsSource = storyObject.GetFieldValue<List<EpisodeData>>(StoryObject.EpisodeDatasKey);
        listView.itemsRemoved += OnRemoved;

        listView.Rebuild();
    }
    
    private void BindItem(VisualElement item, int index)
    {
        item.UnregisterCallback<MouseDownEvent>(OnDoubleClickItem);
        item.RegisterCallback<MouseDownEvent>(OnDoubleClickItem);
        
        var viewItem = (EpisodeViewItem)item;
        
        var instance = listView.itemsSource[index] ?? CreateItem();

        var list = listView.itemsSource;
        list[index] = instance;

        viewItem.InjectData((EpisodeData)instance);
    }
    
    private void UnbindItem(VisualElement item, int index)
    {
        item.UnregisterCallback<MouseDownEvent>(OnDoubleClickItem);
    }
    
    private void OnDoubleClickItem(MouseDownEvent evt)
    {
        if (evt.clickCount == 2)
        {
            TryCreateEditorWindow((EpisodeViewItem)evt.target);
        }
    }
    
    private VisualElement MakeItem()
    {
        var item = new EpisodeViewItem();

        item.RegisterCallback<MouseDownEvent>(OnDoubleClickItem);

        return item;
    }
    
    private EpisodeData CreateItem()
    {
        var instance = ScriptableEntity.Create<EpisodeData>();

        Undo.RecordObject(storyObject, "Create Episode");

        if (!Application.isPlaying)
        {
            AssetDatabase.AddObjectToAsset(instance, storyObject);
        }

        Undo.RegisterCreatedObjectUndo(instance, "Create Episode");
        EditorUtility.SetDirty(storyObject);
        AssetDatabase.SaveAssets();

        return instance;
    }
    
    private void OnRemoved(IEnumerable<int> indexes)
    {
        var list = listView.itemsSource;
        
        foreach (var index in indexes)
        {
            var item = (EpisodeData)list[index];
            
            Undo.RecordObject(storyObject, "Delete");

            AssetDatabase.RemoveObjectFromAsset(item);

            Undo.DestroyObjectImmediate(item);

            EditorUtility.SetDirty(storyObject);
        }
    }
    
    private void TryCreateEditorWindow(EpisodeViewItem viewItem)
    {
        if (viewItem == null || viewItem.EpisodeData == null)
        {
            return;
        }

        var window = GetWindow<EpisodeWindowEditor>();
        
        window.InjectActivation(viewItem.EpisodeData);
        
        window.Show();
    }
}
