using System.Collections.Generic;
using Module.InteractiveEditor.Configs;
using Module.InteractiveEditor.Editor;
using Module.Utils;
using Module.Utils.Configs;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class ActorsWindowEditor : EditorWindow
{
    private StoryObject storyObject;
    private ListView listView;

    [MenuItem("InteractiveEditor/Actors")]
    public static void ShowExample()
    {
        ActorsWindowEditor wnd = GetWindow<ActorsWindowEditor>();
        wnd.titleContent = new GUIContent("ActorsWindowEditor");
    }

    public void CreateGUI()
    {
        VisualElement root = rootVisualElement;

        var uxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(Paths.ActorsWindowUxml);
        var styles = AssetDatabase.LoadAssetAtPath<StyleSheet>(Paths.ActorsWindowUss);

        uxml.CloneTree(root);
        root.styleSheets.Add(styles);

        listView = root.Q<ListView>();

        OnSelectionChange();
    }

    private void BindItem(VisualElement item, int index)
    {
        item.UnregisterCallback<MouseDownEvent>(OnDoubleClickItem);
        item.RegisterCallback<MouseDownEvent>(OnDoubleClickItem);
        
        var actorViewItem = (ActorViewItem)item;
        
        var instance = listView.itemsSource[index] ?? CreateItem();

        var list = listView.itemsSource;
        list[index] = instance;

        actorViewItem.InjectData((Actor)instance);
    }


    private void UnbindItem(VisualElement item, int index)
    {
        item.UnregisterCallback<MouseDownEvent>(OnDoubleClickItem);
    }

    private VisualElement MakeItem()
    {
        var item = new ActorViewItem();

        item.RegisterCallback<MouseDownEvent>(OnDoubleClickItem);

        return item;
    }

    private Actor CreateItem()
    {
        var instance = ScriptableEntity.Create<Actor>();

        Undo.RecordObject(storyObject, "Create Actor");

        if (!Application.isPlaying)
        {
            AssetDatabase.AddObjectToAsset(instance, storyObject);
        }

        Undo.RegisterCreatedObjectUndo(instance, "Create Actor");
        EditorUtility.SetDirty(storyObject);
        AssetDatabase.SaveAssets();

        return instance;
    }

    private void OnSelectionChange()
    {
        storyObject = Selection.activeObject as StoryObject;

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
        listView.itemsSource = storyObject.GetFieldValue<List<Actor>>(StoryObject.ActorsKey);
        listView.itemsRemoved += OnRemoved;

        listView.Rebuild();
    }

    private void OnRemoved(IEnumerable<int> indexes)
    {
        var list = listView.itemsSource;
        
        foreach (var index in indexes)
        {
            var item = (Actor)list[index];
            
            Undo.RecordObject(storyObject, "Delete Actor");

            AssetDatabase.RemoveObjectFromAsset(item);

            Undo.DestroyObjectImmediate(item);

            EditorUtility.SetDirty(storyObject);
        }
    }

    private void OnDoubleClickItem(MouseDownEvent evt)
    {
        if (evt.clickCount == 2)
        {
            TryCreateEditorWindow((ActorViewItem)evt.target);
        }
    }

    private void OnDisable()
    {
        listView.makeItem = null;
        listView.unbindItem = null;
        listView.bindItem = null;
        listView.itemsSource = null;
        listView.itemsRemoved -= OnRemoved;

        AssetDatabase.SaveAssets();
    }

    private void TryCreateEditorWindow(ActorViewItem viewItem)
    {
        if (viewItem == null || viewItem.Actor == null)
        {
            return;
        }

        var window = GetWindow<ActorWindowEditor>();
        
        window.InjectActivation(viewItem.Actor);
        
        window.Show();
    }
}
