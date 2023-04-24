using System;

// Important note about HugeNumber, when adding two numbers,where the difference
// in magnitude between them is larger than 1 million, the addition will be ignored.

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
			return $"{value:0.000}{postfixStr}";
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
	public static bool operator >=(HugeNumber a, float b)
	{
		return a.value == b || a.value > b;
	}
	public static bool operator <=(HugeNumber a, float b)
	{
		return a.value == b || a.value < b;
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
	public static HugeNumber operator -(HugeNumber a, float b)
	{
		return new HugeNumber(a.value - b, a.postfix);
	}
	public static HugeNumber operator +(HugeNumber a, float b)
	{
		return new HugeNumber(a.value + b, a.postfix);
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

