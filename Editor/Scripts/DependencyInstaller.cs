using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace Module.InteractiveEditor.Editor
{
    /// <summary>
    /// Окно для проверки и установки недостающих зависимостей для InteractiveEditor
    /// </summary>
    public class DependencyInstaller : EditorWindow
    {
        private static readonly List<Dependency> RequiredDependencies = new List<Dependency>
        {
            new Dependency
            {
                Name = "Unity Localization",
                PackageId = "com.unity.localization",
                Description = "Требуется для поддержки мультиязычности в InteractiveEditor",
                IsRequired = true
            },
            new Dependency
            {
                Name = "UniTask",
                PackageId = "com.cysharp.unitask",
                GitUrl = "https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask",
                Description = "Требуется для асинхронных операций в InteractiveEditor",
                IsRequired = true
            }
        };

        private Dictionary<string, bool> dependencyStatus = new Dictionary<string, bool>();
        private Dictionary<string, AddRequest> activeRequests = new Dictionary<string, AddRequest>();
        private bool isChecking = false;
        private bool hasChecked = false;
        private ListRequest listRequest;
        private Vector2 scrollPosition;

        private const string PREF_KEY_DONT_SHOW = "InteractiveEditor.DontShowDependencyWindow";

        [MenuItem("Tools/Interactive Editor/Check Dependencies", priority = 100)]
        public static void ShowWindow()
        {
            var window = GetWindow<DependencyInstaller>("InteractiveEditor Dependencies");
            window.minSize = new Vector2(500, 400);
            window.maxSize = new Vector2(500, 600);
            window.Show();
        }

        /// <summary>
        /// Показывает окно, если зависимости отсутствуют и пользователь не отключил автопоказ
        /// </summary>
        public static void ShowWindowIfNeeded()
        {
            // Проверяем, отключил ли пользователь автопоказ
            if (EditorPrefs.GetBool(PREF_KEY_DONT_SHOW, false))
            {
                return;
            }

            var window = GetWindow<DependencyInstaller>("InteractiveEditor Dependencies");
            window.minSize = new Vector2(500, 400);
            window.maxSize = new Vector2(500, 600);
            window.Show();
        }

        private void OnEnable()
        {
            CheckDependencies();
        }

        private void OnGUI()
        {
            DrawHeader();
            
            EditorGUILayout.Space(10);
            
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            
            if (isChecking)
            {
                DrawCheckingStatus();
            }
            else if (hasChecked)
            {
                DrawDependenciesList();
            }
            
            EditorGUILayout.EndScrollView();
            
            EditorGUILayout.Space(10);
            
            DrawFooter();
        }

        private void DrawHeader()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            
            GUILayout.Label("Зависимости InteractiveEditor", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);
            GUILayout.Label(
                "InteractiveEditor требует следующие пакеты для корректной работы. " +
                "Пожалуйста, установите недостающие зависимости.",
                EditorStyles.wordWrappedLabel
            );
            
            EditorGUILayout.EndVertical();
        }

        private void DrawCheckingStatus()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label("Проверка зависимостей...", EditorStyles.centeredGreyMiniLabel);
            EditorGUILayout.EndVertical();
        }

        private void DrawDependenciesList()
        {
            foreach (var dependency in RequiredDependencies)
            {
                DrawDependencyItem(dependency);
                EditorGUILayout.Space(5);
            }
        }

        private void DrawDependencyItem(Dependency dependency)
        {
            var isInstalled = dependencyStatus.ContainsKey(dependency.PackageId) && 
                              dependencyStatus[dependency.PackageId];
            var isInstalling = activeRequests.ContainsKey(dependency.PackageId);

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            
            EditorGUILayout.BeginHorizontal();
            
            // Иконка статуса
            var iconStyle = new GUIStyle(GUI.skin.label);
            iconStyle.fontSize = 16;
            iconStyle.alignment = TextAnchor.MiddleCenter;
            iconStyle.fixedWidth = 30;
            
            if (isInstalling)
            {
                GUILayout.Label("⏳", iconStyle);
            }
            else if (isInstalled)
            {
                var oldColor = GUI.color;
                GUI.color = Color.green;
                GUILayout.Label("✓", iconStyle);
                GUI.color = oldColor;
            }
            else
            {
                var oldColor = GUI.color;
                GUI.color = new Color(1f, 0.5f, 0f);
                GUILayout.Label("⚠", iconStyle);
                GUI.color = oldColor;
            }
            
            EditorGUILayout.BeginVertical();
            
            // Название зависимости
            GUILayout.Label(dependency.Name, EditorStyles.boldLabel);
            
            // Статус
            var status = isInstalling ? "Установка..." : 
                           isInstalled ? "Установлен" : "Не установлен";
            var statusColor = isInstalling ? Color.yellow : 
                               isInstalled ? Color.green : Color.red;
            
            var oldTextColor = GUI.contentColor;
            GUI.contentColor = statusColor;
            GUILayout.Label($"Статус: {status}", EditorStyles.miniLabel);
            GUI.contentColor = oldTextColor;
            
            // Описание
            if (!string.IsNullOrEmpty(dependency.Description))
            {
                EditorGUILayout.Space(3);
                GUILayout.Label(dependency.Description, EditorStyles.wordWrappedMiniLabel);
            }
            
            // ID пакета
            EditorGUILayout.Space(3);
            GUILayout.Label($"Пакет: {dependency.PackageId}", EditorStyles.miniLabel);
            
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.EndHorizontal();
            
            // Кнопка установки
            if (!isInstalled && !isInstalling)
            {
                EditorGUILayout.Space(5);
                if (GUILayout.Button($"Установить {dependency.Name}", GUILayout.Height(25)))
                {
                    InstallDependency(dependency);
                }
            }
            
            EditorGUILayout.EndVertical();
        }

        private void DrawFooter()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("Обновить", GUILayout.Height(30), GUILayout.Width(100)))
            {
                CheckDependencies();
            }
            
            GUILayout.FlexibleSpace();
            
            var allInstalled = RequiredDependencies.All(d => 
                dependencyStatus.ContainsKey(d.PackageId) && dependencyStatus[d.PackageId]);
            
            if (allInstalled)
            {
                var oldColor = GUI.backgroundColor;
                GUI.backgroundColor = Color.green;
                if (GUILayout.Button("Все зависимости установлены - Закрыть", GUILayout.Height(30), GUILayout.Width(250)))
                {
                    Close();
                }
                GUI.backgroundColor = oldColor;
            }
            else
            {
                if (GUILayout.Button("Установить все недостающие", GUILayout.Height(30), GUILayout.Width(200)))
                {
                    InstallAllMissing();
                }
            }
            
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space(5);
            
            // Чекбокс "Не показывать снова"
            EditorGUILayout.BeginHorizontal();
            var dontShow = EditorPrefs.GetBool(PREF_KEY_DONT_SHOW, false);
            var newDontShow = EditorGUILayout.ToggleLeft(
                "Не показывать это окно автоматически", 
                dontShow
            );
            if (newDontShow != dontShow)
            {
                EditorPrefs.SetBool(PREF_KEY_DONT_SHOW, newDontShow);
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.EndVertical();
        }

        private void CheckDependencies()
        {
            isChecking = true;
            hasChecked = false;
            dependencyStatus.Clear();
            listRequest = Client.List();
            EditorApplication.update += CheckDependenciesProgress;
        }

        private void CheckDependenciesProgress()
        {
            if (listRequest == null || !listRequest.IsCompleted)
            {
                return;
            }

            EditorApplication.update -= CheckDependenciesProgress;
            
            if (listRequest.Status == StatusCode.Success)
            {
                foreach (var package in listRequest.Result)
                {
                    dependencyStatus[package.name] = true;
                }
            }
            else
            {
                Debug.LogError($"Не удалось проверить пакеты: {listRequest.Error.message}");
            }

            isChecking = false;
            hasChecked = true;
            listRequest = null;
            Repaint();
        }

        private void InstallDependency(Dependency dependency)
        {
            var packageToInstall = !string.IsNullOrEmpty(dependency.GitUrl) 
                ? dependency.GitUrl 
                : dependency.PackageId;
            
            Debug.Log($"Установка {dependency.Name} ({packageToInstall})...");
            
            var request = Client.Add(packageToInstall);
            activeRequests[dependency.PackageId] = request;
            
            EditorApplication.update += () => InstallProgress(dependency, request);
        }

        private void InstallProgress(Dependency dependency, AddRequest request)
        {
            if (request == null || !request.IsCompleted)
            {
                return;
            }

            EditorApplication.update -= () => InstallProgress(dependency, request);
            
            if (request.Status == StatusCode.Success)
            {
                Debug.Log($"Успешно установлен {dependency.Name}");
                dependencyStatus[dependency.PackageId] = true;
            }
            else
            {
                Debug.LogError($"Не удалось установить {dependency.Name}: {request.Error.message}");
            }

            activeRequests.Remove(dependency.PackageId);
            Repaint();
        }

        private void InstallAllMissing()
        {
            foreach (var dependency in RequiredDependencies)
            {
                var isInstalled = dependencyStatus.ContainsKey(dependency.PackageId) && 
                                  dependencyStatus[dependency.PackageId];
                var isInstalling = activeRequests.ContainsKey(dependency.PackageId);

                if (!isInstalled && !isInstalling)
                {
                    InstallDependency(dependency);
                }
            }
        }

        private class Dependency
        {
            public string Name { get; set; }
            public string PackageId { get; set; }
            public string GitUrl { get; set; }
            public string Description { get; set; }
            public bool IsRequired { get; set; }
        }
    }
}
