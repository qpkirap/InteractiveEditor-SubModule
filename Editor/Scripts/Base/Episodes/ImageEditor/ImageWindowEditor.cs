using Cysharp.Threading.Tasks;
using Module.InteractiveEditor.Configs;
using Module.InteractiveEditor.Editor;
using Module.Utils;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

public class ImageWindowEditor : EditorWindow
{
    private ObjectField imageField;
    private ScrollView scrollView;
    private TextField titleField;
    private VisualElement image;
    private ImageData imageData;
    
    private SerializedObject container;

    private CensureViewItem currentDraw;
    private bool isDrawing = false;
    
    private Vector2 center;
    
    public void CreateGUI()
    {
        var root = rootVisualElement;

        var uxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(Paths.ImagesWindowUxml);
        var styles = AssetDatabase.LoadAssetAtPath<StyleSheet>(Paths.ImagesWindowUss);
        
        uxml.CloneTree(root);
        root.styleSheets.Add(styles);

        titleField = root.Q<TextField>("title-field");
        imageField = root.Q<ObjectField>("image-field");
        scrollView = root.Q<ScrollView>();
        
        image = new VisualElement();
        
        scrollView.Add(image);
    }

    public void InjectActivation(ImageData imageData)
    {
        this.imageData = imageData;
        
        container = new(this.imageData);

        titleField.BindProperty(container.FindProperty("title"));
        
        imageField.objectType = typeof(Sprite);
        imageField.RegisterCallback<ChangeEvent<Object>>(UpdateImageField);
        
        UpdateImageField();

        if (this.imageData.Censures != null)
        {
            foreach (var imageDataCensure in this.imageData.Censures)
            {
                if (imageDataCensure == null) continue;
                
                var viewItem = CreateCensureBox(imageDataCensure.Position, imageDataCensure.Size.x, imageDataCensure.Size.y);
                
                viewItem.InjectData(imageDataCensure);
            }
        }
        
        image.RegisterCallback<MouseDownEvent>(OnMouseDown);
        image.RegisterCallback<MouseMoveEvent>(OnMouseMove);
        image.RegisterCallback<MouseUpEvent>(OnMouseUp);
    }

    private void OnDisable()
    {
        EditorUtility.SetDirty(imageData);
        
        imageField.Unbind();
        image.Unbind();
        
        image.UnregisterCallback<MouseDownEvent>(OnMouseDown);
        image.UnregisterCallback<MouseMoveEvent>(OnMouseMove);
        image.UnregisterCallback<MouseUpEvent>(OnMouseUp);
        
        AssetDatabase.SaveAssets();
    }

    #region Image

    private void UpdateImageField(ChangeEvent<Object> evn = null)
    {
        if (imageField != null)
        {
            if (evn != null)
            {
                var value = evn.newValue;
                
                imageField.SetValueWithoutNotify(value);
                
                var guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(value));

                if (!string.IsNullOrEmpty(guid))
                {
                    var assetReference = new AssetReference(guid);
                    var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(AssetDatabase.GUIDToAssetPath(guid));
                    
                    imageData.SetFieldValue(ImageData.ImageKey, assetReference);
                    imageData.SetFieldValue(ImageData.ImageCacheKey, new AddressableSprite(assetReference));
                    imageData.SetFieldValue(ImageData.ImageSizeKey, sprite != null ? new Vector2(sprite.rect.width, sprite.rect.height) : default);

                    UpdateCensureImageSize();
                }
                else
                {
                    imageData.SetFieldValue<AssetReference>(ImageData.ImageKey, default);
                    imageData.SetFieldValue<AddressableSprite>(ImageData.ImageCacheKey, new(default));
                    imageData.SetFieldValue<Vector2>(ImageData.ImageSizeKey, default);

                    UpdateCensureImageSize();
                }
            }
            
            if (imageData.Image is { RuntimeKeyIsValid: true })
            {
                var guid = imageData.Image.AssetGUID;
            
                var asset = AssetDatabase.LoadAssetAtPath<Sprite>(AssetDatabase.GUIDToAssetPath(guid));
            
                imageField.SetValueWithoutNotify(asset);
            }

            LoadImage(imageData);
        }
    }
    
    private void LoadImage(ImageData imageData)
    {
        if (imageData == null || imageData.Image is not { RuntimeKeyIsValid: true })
        {
            image.style.backgroundImage = null;
            
            image.style.height = 0;
            image.style.width = 0;
            
            return;
        }

        var guid = imageData.Image.AssetGUID;
        var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(AssetDatabase.GUIDToAssetPath(guid));

        if (sprite != null)
        {
            image.style.backgroundImage = new StyleBackground(sprite);
            
            image.style.height = sprite.rect.height;
            image.style.width = sprite.rect.width;
        }
    }
    
    #endregion
    
    #region Drawing
    
    private void OnMouseDown(MouseDownEvent evt)
    {
        if (evt.button == 0) // Левая кнопка мыши
        {
            isDrawing = true;
            center = image.WorldToLocal(evt.mousePosition);
        }
    }
    
    private void OnMouseMove(MouseMoveEvent evt)
    {
        if (isDrawing)
        {
            var width = 0f;
            var height = 0f;
            var left = 0f;
            var top = 0f;

            if (currentDraw == null)
            {
                currentDraw ??= CreateCensureBox(center, width, height);

                var censureData = new CensureData();
                
                imageData.AddToList(ImageData.CensuresKey, censureData);
                
                currentDraw.InjectData(censureData);

                UpdateCensureImageSize();
            }
            
            var localMousePosition = image.WorldToLocal(evt.mousePosition);
            
            if (localMousePosition.x < center.x)
            {
                width = center.x - localMousePosition.x;

                left = localMousePosition.x;
            }
            else
            {
                width = localMousePosition.x - center.x;

                left = center.x;
            }
        
            if (localMousePosition.y < center.y)
            {
                height = center.y - localMousePosition.y;

                top = localMousePosition.y;
            }
            else
            {
                height = localMousePosition.y - center.y;

                top = center.y;
            }
            
            currentDraw.style.left = left;
            currentDraw.style.top = top;

            currentDraw.style.width = width;
            currentDraw.style.height = height;
            
            currentDraw.UpdateData();
        }
    }

    private void UpdateCensureImageSize()
    {
        if (imageData.Censures == null) return;
        
        var size = imageData.GetFieldValue<Vector2>(ImageData.ImageSizeKey);
        
        foreach (var imageDataCensure in imageData.Censures)
        {
            imageDataCensure.SetFieldValue(CensureData.ImageSizeKey, size);
        }
    }
    
    private void OnMouseUp(MouseUpEvent evt)
    {
        if (evt.button == 0) // Левая кнопка мыши
        {
            isDrawing = false;
            
            currentDraw = null;
        }
    }
    
    private CensureViewItem CreateCensureBox(Vector2 position, float width, float height)
    {
        var item = new CensureViewItem
        {
            style =
            {
                position = Position.Absolute,
                left = position.x,
                top = position.y,
                width = width,
                height = height,
                backgroundColor = new StyleColor(Color.red),
                color = new StyleColor(Color.black),
                alignItems = Align.FlexStart,
                unityTextAlign = TextAnchor.MiddleCenter
            }
        };
        
        item.RegisterCallback<MouseDownEvent>(downEvent =>
        {
            if (downEvent.button == 1 && downEvent.target is CensureViewItem target)
            {
                image.Remove(target);
                    
                if (target.CensureData != null) imageData.RemoveFromList(ImageData.CensuresKey, target.CensureData);
            }
        });
        
        image.Add(item);

        return item;
    }
    
    #endregion
}
