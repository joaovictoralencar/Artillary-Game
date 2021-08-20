using System;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(Outliner), menuName = nameof(Outliner))]
public class Outliner : ScriptableObject
{
    [Tooltip("How much difference in pixels in a straight line is considered a gap. This can help smooth out the outline a bit.")] [Min(1)] public uint gapLength = 3;
    [Tooltip("Product for optimizing the outline based on angle. 1 means no optimization. This value should be kept pretty high if you want to maintain round shapes. Note that some points (e.g. outer angles) are never optimized.")] [Range(0f, 1f)] public float product = 0.99f;
}
