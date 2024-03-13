using UnityEngine.UIElements;

public static class VisualElementExtensions
{
    public static T AddChild<T>(this VisualElement element, T child) where T : VisualElement
    {
        element.Add(child);
        return child;
    }
}