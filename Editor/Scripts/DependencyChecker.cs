using System.IO;
using UnityEditor;
using UnityEngine;

namespace Module.InteractiveEditor.Editor
{
    /// <summary>
    /// Автоматически проверяет зависимости при загрузке InteractiveEditor
    /// </summary>
    [InitializeOnLoad]
    public static class DependencyChecker
    {
        private const string PREF_KEY_FIRST_IMPORT = "InteractiveEditor.FirstImportCheck";
        
        static DependencyChecker()
        {
            // Задержка для проверки, чтобы Unity полностью инициализировался
            EditorApplication.delayCall += CheckOnLoad;
        }

        /// <summary>
        /// Динамически находит путь к субмодулю InteractiveEditor по расположению этого скрипта
        /// </summary>
        private static string GetSubmodulePath()
        {
            // Находим этот скрипт через GUID или по имени класса
            var script = UnityEditor.MonoScript.FromScriptableObject(ScriptableObject.CreateInstance<DependencyCheckerMarker>());
            if (script != null)
            {
                var scriptPath = AssetDatabase.GetAssetPath(script);
                if (!string.IsNullOrEmpty(scriptPath))
                {
                    // Поднимаемся на 3 уровня вверх: Scripts -> Editor -> InteractiveEditor
                    var dir = new DirectoryInfo(Path.GetDirectoryName(scriptPath));
                    if (dir.Parent?.Parent != null)
                    {
                        return dir.Parent.Parent.FullName.Replace(Application.dataPath, "Assets").Replace("\\", "/");
                    }
                }
            }
            
            // Альтернативный метод: поиск через GUID файла asmdef
            var asmdefGuids = AssetDatabase.FindAssets("t:asmdef InterctiveEditorEditor");
            if (asmdefGuids.Length > 0)
            {
                var asmdefPath = AssetDatabase.GUIDToAssetPath(asmdefGuids[0]);
                var dir = new DirectoryInfo(Path.GetDirectoryName(asmdefPath));
                if (dir.Parent != null)
                {
                    return dir.Parent.FullName.Replace(Application.dataPath, "Assets").Replace("\\", "/");
                }
            }
            
            return null;
        }

        private static void CheckOnLoad()
        {
            // Динамически определяем путь к субмодулю
            var submodulePath = GetSubmodulePath();
            
            // Проверяем, существует ли InteractiveEditor
            if (string.IsNullOrEmpty(submodulePath) || !Directory.Exists(submodulePath))
            {
                return;
            }

            // Проверяем, первый ли раз после импорта
            var hasCheckedBefore = EditorPrefs.GetBool(PREF_KEY_FIRST_IMPORT, false);
            
            if (!hasCheckedBefore)
            {
                // Отмечаем как проверенное
                EditorPrefs.SetBool(PREF_KEY_FIRST_IMPORT, true);
                
                // Показываем окно зависимостей
                Debug.Log("InteractiveEditor обнаружен. Открываем установщик зависимостей...");
                EditorApplication.delayCall += () =>
                {
                    DependencyInstaller.ShowWindowIfNeeded();
                };
            }
        }

        /// <summary>
        /// Сбрасывает флаг первого импорта (полезно для тестирования или повторного вызова окна)
        /// </summary>
        [MenuItem("Tools/Interactive Editor/Reset First Import Flag", priority = 200)]
        private static void ResetFirstImportFlag()
        {
            EditorPrefs.DeleteKey(PREF_KEY_FIRST_IMPORT);
            EditorPrefs.DeleteKey("InteractiveEditor.DontShowDependencyWindow");
            Debug.Log("Флаг первого импорта сброшен. Окно зависимостей покажется при следующем перезапуске Unity или перекомпиляции скриптов.");
        }
    }
    
    /// <summary>
    /// Маркерный класс для определения пути к скрипту через MonoScript
    /// </summary>
    internal class DependencyCheckerMarker : ScriptableObject { }
}
