using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Module.Utils.Editor
{
    public class KeyScriptFileUtils
    {
        public static bool CheckScriptFile(KeyScriptData scriptData)
        {
            var filePath = scriptData.getPathValue?.Invoke();

            if (string.IsNullOrEmpty(filePath))
            {
                if (GUILayout.Button($"Создать {scriptData.FileName}.cs", GUILayout.Height(20)))
                {
                    CreateScriptFile(scriptData);
                }

                return false;
            }

            return true;
        }

        public static void UpdateScriptFile(KeyScriptData scriptData)
        {
            var routLinkPath = Application.dataPath + scriptData.getPathValue?.Invoke();
            var routLinkText =
                "/*\n" +
                "    Сгенерировано автоматически. Не редактировать!\n" +
                "*/\n" +
                $"namespace {scriptData.namespaceValue}\n" +
                $"{{\n    public static class {scriptData.FileName}\n    {{\n<text>\n    }}\n}}";

            // update link dictionary
            var linkDict = new Dictionary<string, object>();

            var valuesDict = scriptData.getDict?.Invoke();

            if (valuesDict != null)
            {
                foreach (var data in valuesDict)
                {
                    CheckLinkSegments(data.Key.Replace(" ", "_").Split('/'), index: 0, linkDict, data.Value);
                }
            }

            // update links text
            var links = !string.IsNullOrEmpty(scriptData.defaultKey)
                ? $"        public static readonly {scriptData.keyTypeName} {scriptData.defaultKey} = new(\"{scriptData.defaultValue}\");"
                : "";

            CreateRoutLinkContent(scriptData, ref links, depth: 0, linkDict);

            routLinkText = routLinkText.Replace("<text>", links);

            // write text to script file
            File.WriteAllText(routLinkPath, routLinkText);
        }

        public static bool CheckKeySegments(string key)
        {
            var segments = key.Split('/');

            foreach (var segment in segments)
            {
                if (segments.Count(x => x == segment) > 1)
                {
                    return false;
                }
            }

            return true;
        }

        public static void CreateScriptFile(KeyScriptData scriptData)
        {
            var fileName = scriptData.FileName;
            var assetPath =
                EditorUtility.SaveFilePanelInProject($"Создание файла скрипта {fileName}.cs", fileName, "cs", "");

            if (!string.IsNullOrEmpty(assetPath))
            {
                CreateScriptFile(assetPath, scriptData);
            }
        }

        public static void CreateScriptFile(string assetPath, KeyScriptData scriptData)
        {
            AssetDatabase.CreateAsset(new TextAsset(), assetPath);

            var scriptPath = assetPath.Replace("Assets", "");

            scriptData.onPathCreated?.Invoke(scriptPath);

            UpdateScriptFile(scriptData);
        }

        private static void CreateRoutLinkContent(KeyScriptData scriptData, ref string links, int depth,
            Dictionary<string, object> linkDict)
        {
            var spaces = $"        {string.Join("", Enumerable.Repeat("    ", depth))}";

            depth++;

            foreach (var link in linkDict)
            {
                if (link.Value is string value)
                {
                    links +=
                        $"\n{spaces}public static readonly {scriptData.keyTypeName} {link.Key} = new(\"{value}\");";
                }
                else if (link.Value is Dictionary<string, object> segmentDict)
                {
                    links += $"\n{spaces}public static class {link.Key}";
                    links += $"\n{spaces}{{";

                    CreateRoutLinkContent(scriptData, ref links, depth, segmentDict);

                    links += $"\n{spaces}}}";
                }
            }
        }

        private static void CheckLinkSegments(string[] linkSegments, int index, Dictionary<string, object> linkDict,
            string value)
        {
            if (index >= linkSegments.Length) return;

            var lastIndex = linkSegments.Length - 1;
            var segment = linkSegments[index];

            if (index != lastIndex)
            {
                if (!linkDict.TryGetValue(segment, out var segmentDict))
                {
                    segmentDict = new Dictionary<string, object>();

                    linkDict.Add(segment, segmentDict);
                }

                CheckLinkSegments(linkSegments, ++index, segmentDict as Dictionary<string, object>, value);
            }
            else
            {
                segment = char.ToLowerInvariant(segment[0]) + segment.Substring(1);

                linkDict[segment] = value;
            }
        }
    }
}