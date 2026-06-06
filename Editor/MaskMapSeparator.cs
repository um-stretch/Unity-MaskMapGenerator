using UnityEngine;
using UnityEditor;
using UmStretch.MaskMapGenerator;

namespace UmStretch.MaskMapSeparator
{
    public class MaskMapSeparator : EditorWindow
    {
        private static MaskMapSeparator _window;
        private static Vector2 _minWindowSize = new(315, 420);

        private Texture2D _inputTexture;
        private Texture2D _prevInputTexture;
        // Metallic, AO, Detail, Smoothness
        private Texture2D[] _outputTextures = new Texture2D[4];

        private static string _outputName;
        private static string _saveLocation = Config.defaultSaveLocation;

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

            DrawInputTexture();
            if (_inputTexture != _prevInputTexture)
            {
                RefreshOutputTextures();
                _prevInputTexture = _inputTexture;
            }

            GUILayout.Space(40);
            DrawOutputTextures();
        }

        private void DrawInputTexture()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.BeginVertical();
            GUILayout.Label("Input Texture", EditorStyles.centeredGreyMiniLabel);
            _inputTexture = (Texture2D)EditorGUILayout.ObjectField(_inputTexture, typeof(Texture2D), false, GUILayout.Height(96), GUILayout.Width(96));
            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        private void RefreshOutputTextures()
        {
            if (_inputTexture == null)
            {
                _outputTextures = new Texture2D[4];
                return;
            }

            int w = _inputTexture.width;
            int h = _inputTexture.height;

            Color[] inputPixels = _inputTexture.GetPixels();
            Color[][] texPixels = new Color[4][];
            for (int i = 0; i < 4; i++) texPixels[i] = new Color[inputPixels.Length];

            if (Config.useMultithreading)
            {
                System.Threading.Tasks.Parallel.For(0, inputPixels.Length, i =>
                {
                    Color c = inputPixels[i];

                    texPixels[0][i] = AsGrayscaleColor(c.r);
                    texPixels[1][i] = AsGrayscaleColor(c.g);
                    texPixels[2][i] = AsGrayscaleColor(c.b);
                    texPixels[3][i] = AsGrayscaleColor(c.a);
                });
            }
            else
            {
                for (int i = 0; i < inputPixels.Length; i++)
                {
                    Color c = inputPixels[i];

                    texPixels[0][i] = AsGrayscaleColor(c.r);
                    texPixels[1][i] = AsGrayscaleColor(c.g);
                    texPixels[2][i] = AsGrayscaleColor(c.b);
                    texPixels[3][i] = AsGrayscaleColor(c.a);
                }
            }

            for (int i = 0; i < 4; i++)
            {
                Texture2D newTex = new Texture2D(w, h, TextureFormat.RGBA32, false);
                newTex.SetPixels(texPixels[i]);
                newTex.Apply();
                _outputTextures[i] = newTex;
            }
        }

        private Color AsGrayscaleColor(float value)
        {
            return new Color(value, value, value, 1f);
        }

        private void DrawOutputTextures()
        {
            GUILayout.BeginHorizontal();
            for (int i = 0; i < 4; i++)
            {
                Texture2D tex = _outputTextures[i];
                if (tex == null)
                    continue;

                GUILayout.FlexibleSpace();
                EditorGUILayout.ObjectField(tex, typeof(Texture2D), false, GUILayout.Height(64), GUILayout.Width(64));
                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();
        }
    }
}