# Odin Inspector Editor Windows

This document describes the new Odin Inspector-based editor windows that replace the traditional Unity EditorWindow implementations.

## Overview

The following windows have been redesigned using Odin Inspector's `OdinEditorWindow` for better user experience, improved maintainability, and enhanced functionality:

1. **ActorWindow** - Actor editing and management
2. **EpisodesWindow** - Episode collection management 
3. **EpisodeWindow** - Individual episode editing
4. **ImageWindow** - Image data editing and censure management
5. **ImageViewerWindow** - Visual censure area editing with mouse drawing

## Key Benefits

✅ **Improved User Experience**: Better UI organization with collapsible sections and intuitive controls
✅ **Enhanced Functionality**: Built-in validation, context-aware actions, and better data visualization
✅ **Instant Access**: Double-click any supported asset to open in the appropriate specialized editor
✅ **Reduced Code Complexity**: Declarative attribute-based approach vs manual GUI code
✅ **Better Integration**: Seamless Unity serialization and undo system integration
✅ **Consistent Design**: Unified look and feel across all editor windows

## Usage and Access Methods

### Double-Click Asset Opening
**New Feature**: Assets now automatically open in their appropriate Odin editors when double-clicked in the Project window:

- **EpisodeData** assets → Opens [EpisodeWindow](EpisodeWindow)
- **ImageData** assets → Opens [ImageWindow](ImageWindow) 
- **Actor** assets → Opens [ActorWindow](ActorWindow)
- **StoryObject** assets → Opens InteractiveGraphView (as before)

**Priority System**: The handlers use Unity's OnOpenAsset priority system to ensure the correct editor opens:
- Priority 0 (Highest): Specific asset types (EpisodeData, ImageData, Actor)
- Priority 1 (Lower): StoryObject assets for InteractiveGraphView

This ensures that double-clicking an EpisodeData or ImageData will always open the specialized Odin editor, not the graph view.

This provides immediate access to specialized editing capabilities without needing to navigate through menus.

### Menu Access
Windows can also be accessed through:
- **Main Menu**: `InteractiveEditor > Odin Windows > [Window Name]`
- **Bulk Access**: `InteractiveEditor > Open All Windows`

## Window Descriptions

### ActorWindow
**Menu**: `InteractiveEditor > Actor Editor`
**Purpose**: Edit individual Actor objects with inline editor capabilities

**Features**:
- Inline editing of Actor properties using Odin Inspector attributes
- Quick actions for opening in inspector and saving assets
- Context-aware window titles
- Automatic dirty marking and asset saving

**Usage**:
```csharp
// Open window with specific actor
ActorWindow.ShowWindow(myActor);

// Or inject actor into existing window
actorWindow.InjectActivation(myActor);
```

### EpisodesWindow
**Menu**: `InteractiveEditor > Episodes`
**Purpose**: Manage episodes collection for StoryObjects

**Features**:
- Automatic StoryObject detection from selection
- List management with drag/drop reordering
- Custom add/remove functions with proper asset handling
- Context-aware episode creation and deletion
- Integration with EditorsCache for current StoryObject

**Key Capabilities**:
- Creates new episodes as sub-assets of StoryObject
- Handles Undo operations properly
- Automatic refresh when selection changes
- Quick access to individual episode editors

### EpisodeWindow
**Menu**: `InteractiveEditor > Episode Editor`
**Purpose**: Edit individual Episode objects and manage their images

**Features**:
- Inline editing of episode properties
- **Image Preview List**: Displays small previews (50x50px) of each ImageData's sprite
- Visual indicators for missing sprites (red border, placeholder icon)
- Click-to-select functionality for image sprites
- Rich image information display (sprite name, dimensions, censure count)
- Quick access to images manager
- Add new image functionality
- Seamless integration with ImageWindow

**Enhanced Image Display**:
- **Preview Thumbnails**: Each ImageData shows a 50x50 pixel preview of its sprite
- **Smart Fallbacks**: Missing sprites show a placeholder icon with red border
- **Information Display**: Shows image title, sprite name, dimensions, and censure area count
- **Interactive Previews**: Click on any preview to select and ping the sprite asset
- **Visual Status**: Color-coded borders indicate sprite presence (normal/missing)

**Usage**:
```csharp
// Open for specific episode
EpisodeWindow.ShowWindow(myEpisode);
```

### ImageWindow
**Menu**: `InteractiveEditor > Image Editor`
**Purpose**: Edit ImageData objects including sprite assignment and censure management

**Features**:
- Image sprite assignment with automatic size detection
- Censure area management with list interface
- Auto-sizing and bulk operations for censure areas
- Integration with ImageViewerWindow for visual editing
- Real-time synchronization of image size to censure data

**Advanced Features**:
- **Smart Image Handling**: Automatically updates filename and size when sprite changes
- **Censure Management**: Add, remove, and modify censure areas with validation
- **Visual Integration**: Launch ImageViewerWindow for mouse-based censure creation

### ImageViewerWindow
**Purpose**: Visual editor for censure areas with mouse drawing capabilities

**Features**:
- **Mouse Drawing**: Left-click and drag to create censure areas
- **Visual Deletion**: Right-click on areas to remove them
- **Real-time Preview**: See changes immediately with color-coded areas
- **Aspect Ratio Preservation**: Maintains image proportions in display
- **Coordinate Mapping**: Accurate translation between screen and image space

**Controls**:
- **Left Click + Drag**: Create new censure area
- **Right Click**: Delete existing censure area
- **Visual Feedback**: Yellow for temporary areas, red with transparency for existing

## Migration from Old Windows

### Removed Duplicate Functionality
To streamline the user experience, the following redundant features have been removed:

- **Context Menu Items**: `Assets > Interactive Editor > Edit with [Window]` menu items (replaced by double-click)
- **"Open in Inspector" Buttons**: Removed from windows since the Odin inline editor provides better functionality
- **Manual Asset Selection**: Direct double-click is now preferred over right-click menus

### Improved Workflow
**Before**: Right-click asset → Navigate context menu → Select editor
**After**: Double-click asset → Editor opens instantly

### Before (Traditional EditorWindow)
```csharp
public class ActorWindowEditor : EditorWindow
{
    private SerializedObject container;
    private SerializedProperty titleProperty;
    private UnityEditor.Editor editor;
    
    public void InjectActivation(Actor actor)
    {
        // Manual GUI setup with IMGUI containers
        // Complex property binding
        // Manual editor creation and cleanup
    }
}
```

### After (Odin Inspector)
```csharp
public class ActorWindow : OdinEditorWindow
{
    [ShowInInspector, HideLabel]
    [InlineEditor(InlineEditorObjectFieldModes.Hidden)]
    private Actor currentActor;
    
    public void InjectActivation(Actor actor)
    {
        currentActor = actor; // Automatic serialization and UI generation
    }
}
```

## Technical Implementation Details

### Attribute Usage Patterns

**Data Display**:
```csharp
[ShowInInspector, ReadOnly]
[LabelText("Image Size")]
private Vector2 imageSize;
```

**Inline Editing**:
```csharp
[ShowInInspector, HideLabel]
[InlineEditor(InlineEditorObjectFieldModes.Hidden)]
private EpisodeData currentEpisode;
```

**List Management**:
```csharp
[ShowInInspector]
[ListDrawerSettings(
    ShowIndexLabels = true,
    DraggableItems = true,
    CustomAddFunction = nameof(CreateNewEpisode),
    CustomRemoveElementFunction = nameof(RemoveEpisode)
)]
private List<EpisodeData> episodes;
```

**Action Buttons**:
```csharp
[FoldoutGroup("Quick Actions")]
[Button("Save Changes", ButtonSizes.Medium)]
private void SaveChanges() { /* implementation */ }
```

### Window Lifecycle Management

**Proper Setup**:
```csharp
protected override void OnEnable()
{
    base.OnEnable();
    Selection.selectionChanged += OnSelectionChanged;
}

protected override void OnDisable()
{
    Selection.selectionChanged -= OnSelectionChanged;
    SaveChanges(); // Ensure data is saved
    base.OnDisable();
}
```

### Context-Aware Opening

**Static Factory Methods**:
```csharp
public static void ShowWindow(Actor actor)
{
    var window = GetWindow<ActorWindow>();
    window.titleContent = new GUIContent($"Actor Editor - {actor.Title}");
    window.currentActor = actor;
    window.Show();
}
```

## Best Practices

1. **Always use `[ShowInInspector]`** for properties that should appear in Odin windows
2. **Implement proper lifecycle management** with OnEnable/OnDisable
3. **Use FoldoutGroup** to organize related functionality
4. **Provide context-aware static Show methods** for easy window opening
5. **Handle null states gracefully** with appropriate user feedback
6. **Save changes automatically** in OnDisable to prevent data loss
7. **Use EnableIf/DisableIf** for conditional button states
8. **Provide clear user instructions** with InfoBox attributes

## Performance Considerations

- Odin Inspector windows automatically handle serialization and deserialization
- Use `[NonSerialized]` for temporary data that shouldn't persist
- Implement proper cleanup in OnDestroy for heavy resources
- Consider using ReadOnly attributes for display-only data

## Future Enhancements

Potential improvements for the Odin Inspector windows:

1. **Enhanced Visual Editing**: More sophisticated drawing tools in ImageViewerWindow
2. **Batch Operations**: Multi-select capabilities in EpisodesWindow
3. **Templates**: Predefined episode and actor templates
4. **Import/Export**: JSON-based configuration import/export
5. **Validation**: Real-time validation with custom attribute implementations

The new Odin Inspector-based windows provide a much more maintainable and user-friendly editing experience while preserving all functionality from the original implementations.