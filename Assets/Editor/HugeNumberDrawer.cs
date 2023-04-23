using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomPropertyDrawer(typeof(HugeNumber))]
public class HugeNumberDrawer : PropertyDrawer
{
	// Draw the property inside the given rect
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		// Using BeginProperty / EndProperty on the parent property means that
		// prefab override logic works on the entire property.
		EditorGUI.BeginProperty(position, label, property);

		// Draw label
		position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

		var indent = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;

		// Calculate rects
		var valueRect = new Rect(position.x, position.y, 60, position.height);
		var postfixRect = new Rect(position.x + 65, position.y, 100, position.height);

		// Draw fields - pass GUIContent.none to each so they are drawn without labels
		EditorGUI.PropertyField(valueRect, property.FindPropertyRelative("value"), GUIContent.none);
		EditorGUI.PropertyField(postfixRect, property.FindPropertyRelative("postfix"), GUIContent.none);

		// Set indent back to what it was
		EditorGUI.indentLevel = indent;

		EditorGUI.EndProperty();
	}
}