using Studio23.SS2.InteractionSystem.Abstract;
using Studio23.SS2.InteractionSystem.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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

            if (GUILayout.Button("Add New Icon"))
            {
                spriteTable.Icons.Add(new HoverSpriteData());
                EditorUtility.SetDirty(spriteTable);
                AssetDatabase.SaveAssets();
            }

            // Button to automatically find and add interactable classes.
            if (GUILayout.Button("Auto Find Interactables"))
            {
                var interactables = FindInteractableClasses();
                foreach (var interactable in interactables)
                {
                    var newIcon = new HoverSpriteData { Name = interactable.Name };
                    spriteTable.Icons.Add(newIcon);
                }
                EditorUtility.SetDirty(spriteTable);
                AssetDatabase.SaveAssets();
            }

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            for (int i = 0; i < spriteTable.Icons.Count; i++)
            {
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.BeginHorizontal();

                spriteTable.Icons[i].Sprite = (Sprite)EditorGUILayout.ObjectField(
                    spriteTable.Icons[i].Sprite, typeof(Sprite), false, GUILayout.Width(70), GUILayout.Height(70));

                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField("Key", GUILayout.Width(50));
                spriteTable.Icons[i].Name = EditorGUILayout.TextField(
                    spriteTable.Icons[i].Name, GUILayout.Width(135), GUILayout.MaxWidth(135));
                EditorGUILayout.EndVertical();

                GUILayout.FlexibleSpace();

                if (GUILayout.Button("X", GUILayout.Width(30), GUILayout.Height(70)))
                {
                    spriteTable.Icons.RemoveAt(i);
                    EditorUtility.SetDirty(spriteTable);
                    AssetDatabase.SaveAssets();
                    continue;
                }

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndScrollView();

            if (GUI.changed)
            {
                EditorUtility.SetDirty(spriteTable);
                AssetDatabase.SaveAssets();
            }
        }

        // Helper method to find all non-abstract classes inheriting from InteractableBase.
        private IEnumerable<InteractableBase> FindInteractableClasses()
        {
            var interactables = new List<InteractableBase>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(InteractableBase)))
                    {
                        var instance = (InteractableBase)Activator.CreateInstance(type);
                        interactables.Add(instance);
                    }
                }
            }
            return interactables;
        }


    }
}
