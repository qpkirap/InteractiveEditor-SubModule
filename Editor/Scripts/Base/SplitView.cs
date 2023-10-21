using UnityEngine.UIElements;

namespace Module.InteractiveEditor.Editor
{
    public class SplitView : TwoPaneSplitView
    {
        public new class UxmlFactory : UxmlFactory<SplitView, TwoPaneSplitView.UxmlTraits> { }
    }
}