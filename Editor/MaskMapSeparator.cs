using UnityEngine;
using UnityEditor;

namespace UmStretch.MaskMapSeparator
{
    public class MaskMapSeparator : EditorWindow
    {
        private static MaskMapSeparator _window;
        private static Vector2 _minWindowSize = new(200, 300);

        [MenuItem("Tools/um-stretch/Mask Map Separator")]
        public static void OpenWindow()
        {
            _window = GetWindow<MaskMapSeparator>("Mask Map Separator");

            _window.minSize = _minWindowSize;
            _window.maxSize = _minWindowSize;
            _window.maxSize = Vector2.one * 10000;
        }

        void OnGUI()
        {
            _window ??= GetWindow<MaskMapSeparator>("Mask Map Separator");
        }
    }
}