using System.Collections.Generic;
using UnityEngine;

// original : kaiware007
// https://github.com/IndieVisualLab/UnityGraphicsProgramming4/tree/master/Assets/TriangulationByEarClipping
public class EarClipper : ITriangulate
{
    LinkedList<int> indices = new LinkedList<int>();
    List<int> earTipList = new List<int>();

    int[] resultTriangulation;

    public int[] Triangulate(in List<Vector3> points)
    {
        InitializeVertices(points);
        
        EarClipping(points);

        return resultTriangulation;
    }

    void InitializeVertices(in List<Vector3> points)
    {
        resultTriangulation = new int[(points.Count - 2) * 3];
        indices.Clear();
        earTipList.Clear();

        for (int i = 0; i < points.Count; i++) indices.AddLast(i);

        LinkedListNode<int> node = indices.First;
        while (node != null)
        {
            CheckVertex(points, node);
            node = node.Next;
        }
    }

    void CheckVertex(in List<Vector3> vertices, LinkedListNode<int> node)
    {
        int prevIndex = (node.Previous == null) ? indices.Last.Value : node.Previous.Value;
        int nextIndex = (node.Next == null) ? indices.First.Value : node.Next.Value;
        int nowIndex = node.Value;

        Vector3 prevVertex = vertices[prevIndex];
        Vector3 nextVertex = vertices[nextIndex];
        Vector3 nowVertex = vertices[nowIndex];
        
        bool isEar = false;

        if (IsAngleLessPI(nowVertex, prevVertex, nextVertex))
        {
            isEar = true;
            foreach(int i in indices)
            {
                if ((i == prevIndex) || (i == nowIndex) || (i == nextIndex))
                    continue;

                Vector3 p = vertices[i];

                if (Vector3.Distance(p, prevVertex) <= float.Epsilon) continue;
                if (Vector3.Distance(p, nowVertex) <= float.Epsilon) continue;
                if (Vector3.Distance(p, nextVertex) <= float.Epsilon) continue;

                if(IsInTriangle(p, prevVertex, nowVertex, nextVertex) <= 0)
                {
                    isEar = false;
                    break;
                }
                
            }
            if (isEar)
            {
                if (!earTipList.Contains(nowIndex))
                {
                    earTipList.Add(nowIndex);
                }
            }
            else
            {
                if (earTipList.Contains(nowIndex))
                {
                    earTipList.Remove(nowIndex);
                }
            }
            
        }
    }

    void EarClipping(in List<Vector3> vertices)
    {
        int triangleIndex = 0;

        while (earTipList.Count > 0)
        {
            int nowIndex = earTipList[0];

            LinkedListNode<int> indexNode = indices.Find(nowIndex);
            if (indexNode != null)
            {
                int prevIndex = (indexNode.Previous == null) ? indices.Last.Value : indexNode.Previous.Value;
                int nextIndex = (indexNode.Next == null) ? indices.First.Value : indexNode.Next.Value;

                Vector3 prevVertex = vertices[prevIndex];
                Vector3 nextVertex = vertices[nextIndex];
                Vector3 nowVertex = vertices[nowIndex];

                resultTriangulation[triangleIndex + 0] = prevIndex;
                resultTriangulation[triangleIndex + 1] = nowIndex;
                resultTriangulation[triangleIndex + 2] = nextIndex;

                triangleIndex += 3;

                if (indices.Count == 3) break;

                earTipList.RemoveAt(0);
                indices.Remove(nowIndex);

                int[] bothlist = { prevIndex, nextIndex };
                for (int i = 0; i < bothlist.Length; i++)
                {
                    LinkedListNode<int> node = indices.Find(bothlist[i]);
                    CheckVertex(vertices, node);
                }
            }
            else
            {
                Debug.LogError("index now found");
                break;
            }
        }
    }

    static bool IsAngleLessPI(Vector3 o, Vector3 a, Vector3 b)
    {
        return (a.x - o.x) * (b.y - o.y) - (a.y - o.y) * (b.x - o.x) > 0;
    }

    static int CheckLine(Vector3 o, Vector3 p1, Vector3 p2)
    {
        double x0 = o.x - p1.x;
        double y0 = o.y - p1.y;
        double x1 = p2.x - p1.x;
        double y1 = p2.y - p1.y;

        double x0y1 = x0 * y1;
        double x1y0 = x1 * y0;
        double det = x0y1 - x1y0;

        return (det > 0D ? +1 : (det < 0D ? -1 : 0));
    }

    static int IsInTriangle(Vector3 o, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        int sign1 = CheckLine(o, p2, p3);
        if (sign1 < 0)
        {
            return +1;
        }

        int sign2 = CheckLine(o, p3, p1);
        if (sign2 < 0)
        {
            return +1;
        }

        int sign3 = CheckLine(o, p1, p2);
        if (sign3 < 0)
        {
            return +1;
        }

        return (((sign1 != 0) && (sign2 != 0) && (sign3 != 0)) ? -1 : 0);
    }
}
