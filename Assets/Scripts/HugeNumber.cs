using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[Serializable]
public struct HugeNumber
{
	// All postfix increment are in x1000
	public enum Postfix
	{
		None,
		Thousand,
		Million,
		Billion,
		Trillion,
	}

	public float value;
	public Postfix postfix;

	public HugeNumber(float value)
	{
		postfix = Postfix.None;
		while (Math.Abs(value) >= 1000)
		{
			value /= 1000;
			postfix += 1;
		}
		while (Math.Abs(value) < 1 && postfix > Postfix.None)
		{
			value *= 1000;
			postfix -= 1;
		}
		this.value = value;
	}

	public HugeNumber(float value, Postfix postfix) : this(value)
	{
		this.postfix += (int)postfix;
	}

	public HugeNumber(HugeNumber other)
	{
		value = other.value;
		postfix = other.postfix;
	}

	private static string GetPostfixStr(Postfix p)
	{
		switch (p)
		{
			case Postfix.None:
				return "";
			case Postfix.Thousand:
				return "K";
			case Postfix.Million:
				return "M";
			case Postfix.Billion:
				return "B";
			case Postfix.Trillion:
				return "T";
			default:
				throw new Exception($"Unknown postfix '{p}'");
		}
	}

	public override string ToString()
	{
		if (postfix == Postfix.None)
		{
			return $"{value:0}";
		}
		else
		{
			string postfixStr = GetPostfixStr(postfix);
			return $"{value:0.00}{postfixStr}";
		}
	}

	public bool Equals(HugeNumber obj)
	{
		if (obj == null) return false;
		if (ReferenceEquals(this, obj)) return true;

		return value == obj.value && postfix == obj.postfix;
	}

	public override bool Equals(object obj)
	{
		if (obj is not HugeNumber) return false;
		return Equals((HugeNumber)obj);
	}

	public override int GetHashCode()
	{
		return value.GetHashCode() + postfix.GetHashCode();
	}

	public static bool operator ==(HugeNumber a, HugeNumber b)
	{
		return Equals(a, b);
	}

	public static bool operator !=(HugeNumber a, HugeNumber b)
	{
		return !(a == b);
	}

	public static bool operator >(HugeNumber a, HugeNumber b)
	{
		if (a.postfix == b.postfix)
		{
			return a.value > b.value;
		}
		else
		{
			return a.postfix > b.postfix;
		}
	}

	public static bool operator <(HugeNumber a, HugeNumber b)
	{
		return !(a >= b);
	}

	public static bool operator >=(HugeNumber a, HugeNumber b)
	{
		return a == b || a > b;
	}

	public static bool operator <=(HugeNumber a, HugeNumber b)
	{
		return !(a > b);
	}

	public static HugeNumber operator +(HugeNumber a, HugeNumber b)
	{
		if (a.postfix == b.postfix)
		{
			return new HugeNumber(a.value + b.value, a.postfix);
		}
		else if (Math.Abs(a.postfix - b.postfix) >= 2)
		{
			return new HugeNumber(a > b ? a : b);
		}
		else if (a.postfix > b.postfix)
		{
			return new HugeNumber(a.value + b.value/1000, a.postfix);
		}
		else // a.postfix < b.postfix
		{
			return new HugeNumber(a.value/1000 + b.value, b.postfix);
		}
	}

	public static HugeNumber operator -(HugeNumber a)
	{
		return new HugeNumber(-a.value, a.postfix);
	}

	public static HugeNumber operator -(HugeNumber a, HugeNumber b)
	{
		return a + (-b);
	}

	public static HugeNumber operator *(HugeNumber a, HugeNumber b)
	{
		if (a.postfix == b.postfix)
		{
			return new HugeNumber(a.value * b.value, a.postfix);
		}
		else
		{
			int postfixDiff = a.postfix - b.postfix;
			return new HugeNumber(a.value + b.value * (float)Math.Pow(1000, postfixDiff), a.postfix);
		}
	}

	public static HugeNumber operator *(HugeNumber a, float b)
	{
		return new HugeNumber(a.value * b, a.postfix);
	}

	public static HugeNumber operator /(HugeNumber a, HugeNumber b)
	{
		return a * new HugeNumber(1 / b.value, b.postfix);
	}
}

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