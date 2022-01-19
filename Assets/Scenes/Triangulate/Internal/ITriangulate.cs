using System.Collections.Generic;
using UnityEngine;

public interface ITriangulate
{
    int[] Triangulate(in List<Vector3> points);
}

public enum TriangulateType
{
    Manage,
    Native,
}
