using Studio23.SS2.InteractionSystem.Abstract;
using Studio23.SS2.InteractionSystem.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Studio23.SS2.InteractionSystem.Editor
{
    public class InteractionSystemEditorUtility : EditorWindow
    {
        private InteractableHoverSpriteTable spriteTable;
        private Vector2 scrollPosition;

        [MenuItem("Studio-23/InteractionSystem/Interactable Hover Sprite Table")]
        private static void Init()
        {
            var window = (InteractionSystemEditorUtility)GetWindow(typeof(InteractionSystemEditorUtility));
            window.titleContent = new GUIContent("Hover Sprite Table Map");
            window.Show();
        }

        private void OnEnable()
        {
            LoadOrCreateTable();
        }

        private void LoadOrCreateTable()
        {
            spriteTable = Resources.Load<InteractableHoverSpriteTable>("InteractionSystem/InteractableHoverSpriteTable");
            if (spriteTable == null)
            {
                if (EditorUtility.DisplayDialog("Sprite Table Not Found",
                    "No Interactable Hover Sprite Table found in Resources/InteractionSystem. Would you like to create one?", "Yes", "No"))
                {
                    CreateNewTable();
                }
            }
        }

        private void CreateNewTable()
        {
            spriteTable = CreateInstance<InteractableHoverSpriteTable>();
            string assetPath = "Assets/Resources/InteractionSystem";
            if (!Directory.Exists(assetPath))
            {
                Directory.CreateDirectory(assetPath);
            }
            AssetDatabase.CreateAsset(spriteTable, assetPath + "/InteractableHoverSpriteTable.asset");
            spriteTable.Icons = new List<HoverSpriteData>();
            AssetDatabase.SaveAssets();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = spriteTable;
        }

        private void OnGUI()
        {
            if (spriteTable == null)
            {
                EditorGUILayout.HelpBox("No Sprite Table Loaded", MessageType.Warning);
                if (GUILayout.Button("Create New Sprite Table"))
                {
                    CreateNewTable();
                }
                return;
            }

            // Button to automatically find and add interactable classes.
            if (GUILayout.Button("Auto Find New Interactables"))
            {
                var interactables = FindInteractables();
                foreach (var interactable in interactables)
                {
                    if(spriteTable.Icons.FirstOrDefault(r=>r.Name==interactable)!=null)continue;
                    var newIcon = new HoverSpriteData { Name = interactable };
                    spriteTable.Icons.Add(newIcon);
                }
                EditorUtility.SetDirty(spriteTable);
                AssetDatabase.SaveAssets();
            }

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            // Adjusting styles for a more fitting content
            GUIStyle boldBorderStyle = new GUIStyle(GUI.skin.box);
            boldBorderStyle.border = new RectOffset(2, 2, 2, 2);
            boldBorderStyle.margin = new RectOffset(5, 5, 5, 5);

            // Table with 2 columns: Label and Sprite
            foreach (var hoverSpriteData in spriteTable.Icons)
            {
                EditorGUILayout.BeginHorizontal("box"); // Start a horizontal row with box style

                // First Column: Label
                EditorGUILayout.BeginVertical(boldBorderStyle, GUILayout.ExpandWidth(true)); // Flexible width
                EditorGUILayout.LabelField(hoverSpriteData.Name);
                EditorGUILayout.EndVertical();

                // Second Column: Sprite
                hoverSpriteData.Sprite = (Sprite)EditorGUILayout.ObjectField(
                    hoverSpriteData.Sprite, typeof(Sprite), false, GUILayout.Width(70), GUILayout.Height(70));

                EditorGUILayout.EndHorizontal(); // End horizontal row
            }

            EditorGUILayout.EndScrollView(); // End the scroll view

            if (GUI.changed)
            {
                EditorUtility.SetDirty(spriteTable);
                AssetDatabase.SaveAssets();
            }
        }




        public IEnumerable<string> FindInteractables()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var interactables = new List<string>();

            foreach (var assembly in assemblies)
            {
                Type[] types;
                try
                {
                    types = assembly.GetTypes();
                }
                catch (ReflectionTypeLoadException ex)
                {
                    types = ex.Types.Where(t => t != null).ToArray();
                }

                interactables.AddRange(
                    types.Where(type => !type.IsAbstract && type.IsClass && typeof(InteractableBase).IsAssignableFrom(type))
                        .Select(type => type.Name));
            }

            return interactables;
        }




    }
}
