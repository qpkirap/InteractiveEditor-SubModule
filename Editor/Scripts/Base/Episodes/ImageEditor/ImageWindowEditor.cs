using Cysharp.Threading.Tasks;
using Module.InteractiveEditor.Configs;
using Module.InteractiveEditor.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class ImageWindowEditor : EditorWindow
{
    private VisualElement image;
    private ImageData imageData;
    
    private bool isDrawing = false;
    private Vector2 center;
    private float radius;

    // [MenuItem("Window/UI Toolkit/ImageWindowEditor")]
    // public static void ShowExample()
    // {
    //     ImageWindowEditor wnd = GetWindow<ImageWindowEditor>();
    //     wnd.titleContent = new GUIContent("ImageWindowEditor");
    // }

    public void InjectData(ImageData imageData)
    {
        this.imageData = imageData;

        if (imageData.Image != null)
        {
            LoadImageAsync(imageData);
        }
        
        image.RegisterCallback<MouseDownEvent>(OnMouseDown);
        image.RegisterCallback<MouseMoveEvent>(OnMouseMove);
        image.RegisterCallback<MouseUpEvent>(OnMouseUp);
    }
    
    private void OnMouseDown(MouseDownEvent evt)
    {
        if (evt.button == 0) // Левая кнопка мыши
        {
            isDrawing = true;
            center = evt.mousePosition;
            radius = 0f;
        }
    }
    
    private void OnMouseMove(MouseMoveEvent evt)
    {
        if (isDrawing)
        {
            Vector2 mousePosition = evt.mousePosition;
            radius = Vector2.Distance(center, mousePosition);

            // Здесь вы можете нарисовать круглую область на вашем элементе
            // используя полученные координаты и радиус
        }
    }
    
    private void OnMouseUp(MouseUpEvent evt)
    {
        if (evt.button == 0) // Левая кнопка мыши
        {
            isDrawing = false;
        }
    }

    private async UniTask LoadImageAsync(ImageData imageData)
    {
        if (imageData == null || imageData.Image is not { RuntimeKeyIsValid: true }) return;

        var sprite = await imageData.Image.LoadAsync();

        if (sprite != null)
        {
            image.style.backgroundImage = new StyleBackground(sprite);
            
            rootVisualElement.transform.scale = new Vector3(sprite.rect.width, sprite.rect.height, 1);
        }
    }

    public void CreateGUI()
    {
        VisualElement root = rootVisualElement;

        var uxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(Paths.ImagesWindowUxml);
        var styles = AssetDatabase.LoadAssetAtPath<StyleSheet>(Paths.ImagesWindowUss);
        
        uxml.CloneTree(root);
        root.styleSheets.Add(styles);

        image = root.Q<VisualElement>();
    }
}
