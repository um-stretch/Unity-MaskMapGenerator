using UnityEngine;
using UnityEditor;
using System.IO;

namespace UmStretch.MaskMap
{
    public class MaskMapSeparator : EditorWindow
    {
        private static MaskMapSeparator _window;
        private static Vector2 _minWindowSize = new(315, 420);

        private Texture2D _inputTexture;
        private Texture2D _prevInputTexture;
        // Metallic, AO, Detail, Smoothness
        private Texture2D[] _outputTextures = new Texture2D[4];

        private static string _outputName = "MaskMapTextures";
        private static string _saveLocation = Config.defaultSaveLocation;

        private static GUIStyle _labelStyle;

        private Rect _inputRect;
        private Rect[] _outputRects = new Rect[4];

        [MenuItem("Tools/um-stretch/Mask Map Separator")]
        public static void OpenWindow()
        {
            _window = GetWindow<MaskMapSeparator>("Mask Map Separator");

            _window.minSize = _minWindowSize;
            _window.maxSize = _minWindowSize;
            _window.maxSize = Vector2.one * 10000;

            _labelStyle = EditorStyles.whiteMiniLabel;
            _labelStyle.alignment = TextAnchor.MiddleCenter;
        }

        void OnGUI()
        {
            if (_window == null)
                OpenWindow();

            DrawInputTexture();
            if (_inputTexture != _prevInputTexture)
            {
                _inputTexture = Utilities.MakeReadable(_inputTexture);
                RefreshOutputTextures();
                _prevInputTexture = _inputTexture;
            }

            GUILayout.Space(40);

            GUILayout.BeginHorizontal();
            DrawOutputTexture("Metallic", _outputTextures[0]);
            DrawOutputTexture("Occlusion", _outputTextures[1]);
            DrawOutputTexture("Detail", _outputTextures[2]);
            DrawOutputTexture("Smoothness", _outputTextures[3]);
            GUILayout.EndHorizontal();

            GUILayout.FlexibleSpace();
            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));

            // Output name
            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("Name", "Name of the output folder (and base name for textures)."), EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();

            // View source
            if (GUILayout.Button(new GUIContent("?", "View source.")))
            {
                Application.OpenURL("https://github.com/um-stretch/Unity-MaskMapGenerator");
            }
            GUILayout.EndHorizontal();

            _outputName = GUILayout.TextField(_outputName, GUILayout.Width(_window.position.width * 0.66f));

            // Save location
            GUILayout.Label(new GUIContent("Save Location"), EditorStyles.boldLabel);
            GUILayout.BeginHorizontal();

            _saveLocation = EditorGUILayout.TextField(_saveLocation);

            if (GUILayout.Button(new GUIContent("...", "Browse"), GUILayout.Width(24)))
            {
                string projectRoot = Directory.GetParent(Application.dataPath).FullName;
                projectRoot = projectRoot.Replace("\\", "/");

                string absolutePath = EditorUtility.OpenFolderPanel("Save Location", "Assets", "");

                if (!string.IsNullOrEmpty(absolutePath) && absolutePath.StartsWith(projectRoot))
                {
                    string savePath = absolutePath.Substring(projectRoot.Length + 1);
                    _saveLocation = savePath;
                }
                else
                {
                    _saveLocation = Config.defaultSaveLocation;
                }
            }
            GUILayout.EndHorizontal();

            if (GUILayout.Button(new GUIContent("Extract Textures", "Save extracted textures."), GUILayout.Height(48)))
            {
                SaveExtractedTextures();
            }
        }

        private void DrawInputTexture()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.BeginVertical();
            GUILayout.Label("Input Texture", EditorStyles.whiteBoldLabel);
            _inputTexture = (Texture2D)EditorGUILayout.ObjectField(_inputTexture, typeof(Texture2D), false, GUILayout.Height(96), GUILayout.Width(96));
            _inputRect = GUILayoutUtility.GetLastRect();
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

            _outputName = _inputTexture.name;

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

        private void DrawOutputTexture(string label, Texture2D texture)
        {
            GUILayout.FlexibleSpace();
            GUILayout.BeginVertical();
            GUILayout.Label(label, _labelStyle);
            EditorGUILayout.ObjectField(texture, typeof(Texture2D), false, GUILayout.Height(64), GUILayout.Width(64));
            _outputRects[0] = GUILayoutUtility.GetLastRect();
            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
        }

        private void SaveExtractedTextures()
        {
            string path = Path.Combine(_saveLocation, _outputName);
            Directory.CreateDirectory(path);

            Utilities.SaveToPng(_outputTextures[0], path, $"{_outputName}_metallic");
            Utilities.SaveToPng(_outputTextures[1], path, $"{_outputName}_ao");
            Utilities.SaveToPng(_outputTextures[2], path, $"{_outputName}_detail");
            Utilities.SaveToPng(_outputTextures[3], path, $"{_outputName}_smoothness");
        }
    }
}