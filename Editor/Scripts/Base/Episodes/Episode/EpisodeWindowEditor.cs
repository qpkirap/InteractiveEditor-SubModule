using System;
using System.Collections.Generic;
using Module.InteractiveEditor.Configs;
using Module.InteractiveEditor.Editor;
using Module.Utils;
using Module.Utils.Configs;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class EpisodeWindowEditor : EditorWindow
{
    private TextField titleTextField;
    private ListView imageListView;
    private EpisodeData episodeData;

    private SerializedObject container;

    public Action<ImageData> OnSelectImage;
    public Action<EpisodeWindowEditor> OnClose;
    
    public void InjectActivation(EpisodeData episodeData)
    {
        this.episodeData = episodeData;
        
        container = new(this.episodeData);
        
        titleTextField.BindProperty(container.FindProperty("title"));
        
        InitImageList();
    }
    
    private void OnDisable()
    {
        EditorUtility.SetDirty(episodeData);
        
        imageListView.makeItem = null;
        imageListView.unbindItem = null;
        imageListView.bindItem = null;
        imageListView.itemsSource = null;
        imageListView.itemsRemoved -= OnRemoved;

        AssetDatabase.SaveAssets();
        
        OnClose?.Invoke(this);
    }

    public void CreateGUI()
    {
        var root = rootVisualElement;

        var uxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(Paths.EpisodeWindowUxml);
        var styles = AssetDatabase.LoadAssetAtPath<StyleSheet>(Paths.EpisodeWindowUss);

        uxml.CloneTree(root);
        root.styleSheets.Add(styles);

        imageListView = root.Q<ListView>();
        titleTextField = root.Q<TextField>();
    }

    private void OnFocus()
    {
        InitImageList();
    }

    #region ImageList

    private void InitImageList()
    {
        imageListView.makeItem = null;
        imageListView.unbindItem = null;
        imageListView.bindItem = null;
        imageListView.itemsSource = null;
        imageListView.itemsRemoved -= OnRemoved;

        imageListView.showAddRemoveFooter = false;

        if (episodeData == null)
        {
            imageListView.Clear();
            return;
        }
        
        imageListView.showAddRemoveFooter = true;

        imageListView.makeItem = MakeItem;
        imageListView.bindItem = BindItem;
        imageListView.unbindItem = UnbindItem;
        imageListView.itemsSource = episodeData.GetFieldValue<List<ImageData>>(EpisodeData.ImageDatasKey);
        imageListView.itemsRemoved += OnRemoved;

        imageListView.Rebuild();
    }
    
    private void BindItem(VisualElement item, int index)
    {
        item.UnregisterCallback<MouseDownEvent>(OnDoubleClickItem);
        item.RegisterCallback<MouseDownEvent>(OnDoubleClickItem);
        
        var viewItem = (ImageDataViewItem)item;
        
        var instance = imageListView.itemsSource[index] ?? CreateItem();

        var list = imageListView.itemsSource;
        list[index] = instance;

        viewItem.InjectData((ImageData)instance);
    }
    
    private void OnDoubleClickItem(MouseDownEvent evt)
    {
        if (evt.clickCount == 1)
        {
            var item = (ImageDataViewItem)evt.target;
            
            if (item == null) return;
            
            OnSelectImage?.Invoke(item.ImageData);
        }
        if (evt.clickCount == 2)
        {
            TryCreateEditorWindow((ImageDataViewItem)evt.target);
        }
    }
    
    private void UnbindItem(VisualElement item, int index)
    {
        item.UnregisterCallback<MouseDownEvent>(OnDoubleClickItem);
    }
    
    private VisualElement MakeItem()
    {
        var item = new ImageDataViewItem();

        item.RegisterCallback<MouseDownEvent>(OnDoubleClickItem);

        return item;
    }
    
    private ImageData CreateItem()
    {
        var instance = ScriptableEntity.Create<ImageData>();

        Undo.RecordObject(episodeData, "Create ImageData");

        if (!Application.isPlaying)
        {
            AssetDatabase.AddObjectToAsset(instance, episodeData);
        }

        Undo.RegisterCreatedObjectUndo(instance, "Create ImageData");
        EditorUtility.SetDirty(episodeData);
        AssetDatabase.SaveAssets();

        return instance;
    }
    
    private void OnRemoved(IEnumerable<int> indexes)
    {
        var list = imageListView.itemsSource;
        
        foreach (var index in indexes)
        {
            var item = (ImageData)list[index];
            
            Undo.RecordObject(episodeData, "Delete");

            AssetDatabase.RemoveObjectFromAsset(item);

            Undo.DestroyObjectImmediate(item);

            EditorUtility.SetDirty(episodeData);
        }
    }
    
    private void TryCreateEditorWindow(ImageDataViewItem viewItem)
    {
        if (viewItem == null || viewItem.ImageData == null)
        {
            return;
        }

        var window = GetWindow<ImageWindowEditor>();
        
        window.Show();
        
        window.InjectActivation(viewItem.ImageData);
    }

    #endregion
}
