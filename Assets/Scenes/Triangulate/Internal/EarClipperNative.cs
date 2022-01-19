using System;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarClipperNative : ITriangulate
{
    [DllImport("ear_clipping", EntryPoint="triangulate")]
    static extern bool Triangulate(Vector3[] v, int size, int[] f);

    public int[] Triangulate(in List<Vector3> points)
    {
        int cnt = points.Count;
        var indices = new int[(cnt-2)*3];
        if(Triangulate(points.ToArray(), cnt, indices))
            return indices;
        else
            throw new InvalidOperationException();
    }
}
