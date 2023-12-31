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
    private VisualElement image;
    private ImageData imageData;

    private VisualElement currentDraw;
    private bool isDrawing = false;
    private Vector2 center;
    private float radius;
    private float width;
    private float height;
    
    public void CreateGUI()
    {
        VisualElement root = rootVisualElement;

        var uxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(Paths.ImagesWindowUxml);
        var styles = AssetDatabase.LoadAssetAtPath<StyleSheet>(Paths.ImagesWindowUss);
        
        uxml.CloneTree(root);
        root.styleSheets.Add(styles);

        imageField = root.Q<ObjectField>("image-field");
        scrollView = root.Q<ScrollView>();
        
        image = new VisualElement();
        
        scrollView.Add(image);
    }

    public void InjectActivation(ImageData imageData)
    {
        this.imageData = imageData;
        
        imageField.objectType = typeof(Sprite);
        imageField.RegisterCallback<ChangeEvent<Object>>(UpdateImageField);
        
        UpdateImageField();
        
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
                    
                    imageData.SetFieldValue(ImageData.ImageKey, assetReference);
                    imageData.SetFieldValue(ImageData.ImageCacheKey, new AddressableSprite(assetReference));
                }
                else
                {
                    imageData.SetFieldValue<AssetReference>(ImageData.ImageKey, default);
                    imageData.SetFieldValue<AddressableSprite>(ImageData.ImageCacheKey, new());
                }
            }
            
            if (imageData.Image is { RuntimeKeyIsValid: true })
            {
                var guid = imageData.Image.AssetGUID;
            
                var asset = AssetDatabase.LoadAssetAtPath<Sprite>(AssetDatabase.GUIDToAssetPath(guid));
            
                imageField.SetValueWithoutNotify(asset);
            }

            LoadImageAsync(imageData);
        }
    }
    
    private async UniTask LoadImageAsync(ImageData imageData)
    {
        if (imageData == null || imageData.Image is not { RuntimeKeyIsValid: true })
        {
            image.style.backgroundImage = null;
            
            image.style.height = 0;
            image.style.width = 0;
            
            return;
        }

        var sprite = await imageData.Image.LoadAsync();

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
            radius = 0f;
        }
    }
    
    private void OnMouseMove(MouseMoveEvent evt)
    {
        if (isDrawing)
        {
            var localMousePosition = image.WorldToLocal(evt.mousePosition);
            
            radius = Vector2.Distance(center, localMousePosition);
            
            if (localMousePosition.x < center.x)
            {
                width = center.x - localMousePosition.x;
            }
            else
            {
                width = localMousePosition.x - center.x;
            }
        
            if (localMousePosition.y < center.y)
            {
                height = center.y - localMousePosition.y;
            }
            else
            {
                height = localMousePosition.y - center.y;
            }

            OnDrawCensureBox();
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
    
    private void OnDrawCensureBox()
    {
        if (isDrawing)
        {
            currentDraw ??= CreateCensureBox(center, 1, 1);
            
            currentDraw.style.left = center.x;
            currentDraw.style.top = center.y;

            currentDraw.style.width = width;
            currentDraw.style.height = height;
        }
    }
    
    private VisualElement CreateCensureBox(Vector2 position, float width, float height)
    {
        var item = new VisualElement
        {
            style =
            {
                position = Position.Absolute,
                left = position.x,
                top = position.y,
                width = width,
                height = height,
                backgroundColor = new StyleColor(Color.red),
            }
        };
        
        image.Add(item);

        return item;
    }
    
    #endregion
}
