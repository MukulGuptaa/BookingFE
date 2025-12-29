#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[InitializeOnLoad]
public static class AutoDisableImageRaycast
{
    static AutoDisableImageRaycast()
    {
        // Listen to new object creation
        ObjectFactory.componentWasAdded += OnComponentAdded;
    }

    private static void OnComponentAdded(Component component)
    {
        if (component is Image image)
        {
            image.raycastTarget = false;
            image.maskable = false;
            EditorUtility.SetDirty(image);
        }
    }
}
#endif