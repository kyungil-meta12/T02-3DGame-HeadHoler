using System;
using System.Security.Cryptography;
using UnityEngine;

// 특정 값이 특정 범위 안에 포함되는지 확인하는 모듈
// inclusive min - inclusive max 범위이다. 

public static class St_Range
{
    public static bool IsInRange(int val, int rangeMinInclusive, int rangeMaxInclusive)
    {
        return rangeMinInclusive <= val && val <= rangeMaxInclusive;
    }

    public static bool IsInRange(Vector2 val, Vector2 rangeMinInclusive, Vector2 rangeMaxInclusive)
    {
        return 
        rangeMinInclusive.x <= val.x && val.x <= rangeMaxInclusive.x && 
        rangeMinInclusive.y <= val.y && val.y <= rangeMaxInclusive.y;
    }

    public static bool IsInRange(Vector3 val, Vector3 rangeMinInclusive, Vector3 rangeMaxInclusive)
    {
        return 
        rangeMinInclusive.x <= val.x && val.x <= rangeMaxInclusive.x && 
        rangeMinInclusive.y <= val.y && val.y <= rangeMaxInclusive.y && 
        rangeMinInclusive.z <= val.z && val.z <= rangeMaxInclusive.z;
    }

    public static bool Probability(int percentage)
    {
        int max = 100;
        int min = 1;
        byte[] bytes = new byte[4];
        RandomNumberGenerator.Fill(bytes);
        int rawValue = BitConverter.ToInt32(bytes, 0);
        uint uValue = unchecked((uint)rawValue);
        uint range = (uint)(max - min + 1);
        int result = (int)(uValue % range) + min;
        return IsInRange(result, min, percentage);
    }

    public static int GenInt(int minInclusive, int maxInclusive)
    {
        byte[] bytes = new byte[4];
        RandomNumberGenerator.Fill(bytes);
        int rawValue = BitConverter.ToInt32(bytes, 0);
        uint uValue = unchecked((uint)rawValue);
        uint range = (uint)(maxInclusive - minInclusive + 1);
        int result = (int)(uValue % range) + minInclusive;
        return result;
    }
}
