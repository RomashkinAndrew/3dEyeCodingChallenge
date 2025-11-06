using System.Text;

namespace Sorter.DataStructures;

/// <summary>
/// Represents a line of a data file
/// </summary>
public readonly struct Line : IEquatable<Line>, IComparable<Line>
{
    static readonly byte DOT_CODE = (byte)'.';

    public ulong Number { get; }
    public string Str { get; }
    public readonly int Length => Number.ToString().Length + Str.Length + 1;

    public Line()
    {
        Str = string.Empty;
    }

    private Line(ulong number, string str)
    {
        Number = number;
        Str = str;
    }

    /// <summary>
    /// Parse a line directly from a byte array. Low-level-optimized
    /// </summary>
    public static Line Parse(byte[] data, int start, int length)
    {
        ulong number = 0;
        string str;

        int dotIndex = -1;
        for (int i = start; i < start + length; i++)
        {
            if (data[i] == DOT_CODE)
            {
                dotIndex = i;
                break;
            }
        }

        int digit = 0;
        for (int i = dotIndex - 1; i >= start; i--)
        {
            number += (ulong)(data[i] - 48) * Pow10(digit);
            digit++;
        }

        str = Encoding.ASCII.GetString(data[(dotIndex + 1)..(start + length)]);

        return new Line(number, str);
    }

    public static Line Parse(string line)
    {
        byte[] lineData = Encoding.ASCII.GetBytes(line);
        return Parse(lineData, 0, lineData.Length);
    }

    static ulong Pow10(int power)
    {
        ulong result = 1;
        for (int i = 0; i < power; i++)
        {
            result *= 10;
        }
        return result;
    }

    public override readonly string ToString()
    {
        return $"{Number}.{Str}";
    }

    /// <summary>
    /// Determines if the line is correctly formatted
    /// </summary>
    public bool IsValid
    {
        get
        {
            if (Str.Contains('\n') || Str.Contains('\r') || Str.Contains('\0') || Str.Contains('.') || string.IsNullOrWhiteSpace(Str))
            {
                return false;
            }
            return true;
        }
    }

    public override bool Equals(object? obj)
    {
        return obj is Line line && Equals(line);
    }

    public bool Equals(Line other)
    {
        return Number == other.Number &&
               Str == other.Str;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Number, Str);
    }

    public int CompareTo(Line other)
    {
        int strComparison = Str.CompareTo(other.Str);
        if (strComparison != 0)
        {
            return strComparison;
        }
        return Number.CompareTo(other.Number);
    }

    public static bool operator ==(Line left, Line right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Line left, Line right)
    {
        return !(left == right);
    }

    public static bool operator >(Line left, Line right)
    {
        return left.CompareTo(right) > 0;
    }

    public static bool operator <(Line left, Line right)
    {
        return left.CompareTo(right) < 0;
    }

    public static bool operator >=(Line left, Line right)
    {
        return left.CompareTo(right) >= 0;
    }

    public static bool operator <=(Line left, Line right)
    {
        return left.CompareTo(right) <= 0;
    }
}
