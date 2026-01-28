/*****************************************************************************
// File Name : ReadOnlyEditor.cs
// Author : Brandon Koederitz
// Creation Date : 10/25/2025
// Last Modified : 10/25/2025
//
// Brief Description : Draws a property within a disabled group so that it cant be assigned to in the inspector.
*****************************************************************************/
using UnityEditor;
using UnityEngine;

namespace CustomAttributes
{
    [CustomPropertyDrawer(typeof(ReadOnly))]
    public class ReadOnlyEditor : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //base.OnGUI(position, property, label);
            // Nests the property in a disabled group.
            EditorGUI.BeginDisabledGroup(true);
            EditorGUI.PropertyField(position, property, label);
            EditorGUI.EndDisabledGroup();
        }
    }
}
