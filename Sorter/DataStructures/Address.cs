using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sorter.DataStructures;

public struct Address
{
    public long Position { get; private set; }
    public ushort Length { get; private set; }
    public readonly ulong Raw => (ulong)Position << 16 | Length;

    public Address(ulong raw)
    {
        Position = (long)(raw >> 16);
        Length = (ushort)(raw & 0xFFFF);
    }

    public Address(long position, ushort length)
    {
        Position = position;
        Length = length;
    }

    public override bool Equals(object? obj)
    {
        return obj is Address address &&
               Position == address.Position;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Position);
    }

    internal static ulong ToRaw(long position, ushort length)
    {
        return (ulong)position << 16 | length;
    }

    public static bool operator ==(Address left, Address right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Address left, Address right)
    {
        return !(left == right);
    }
}