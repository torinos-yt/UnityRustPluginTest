using System;
using System.Collections.Generic;
using UnityEngine;

public class Trianglurate : MonoBehaviour
{
    [SerializeField] TriangulateType _type = TriangulateType.Manage;
    [SerializeField, Range(.0001f, .01f)] float _pointThreshold = .003f;
    [SerializeField] bool _compare = false;

    List<Vector3> _points = new();
    Vector3 _prevPos;

    MeshFilter _mesh;
    Camera _mainCam;

    EarClipper _earClipper = new();
    EarClipperNative _earClipperNative = new();

    void Start()
    {
        _mesh = this.GetComponent<MeshFilter>();
        _mesh.sharedMesh = new Mesh();

        _mainCam = Camera.main;
    }

    void Update()
    {
        var aspect = Screen.width / (float)Screen.height;
        Vector3 mousePos = _mainCam.ScreenToViewportPoint(Input.mousePosition) * 2 + Vector3.left;
        mousePos.x *= aspect;

        if((_points.Count <= 0 || Vector3.Distance(_prevPos, mousePos) >= _pointThreshold)
            && Input.GetMouseButton(0))
        {
            _points.Add(mousePos);
            _prevPos = mousePos;
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            _points.Clear();
            _mesh.sharedMesh = new Mesh();
        }

        if(_points.Count > 3)
        {
            BuildMesh();
        }
    }

    void BuildMesh()
    {
        int[] indices;

        if(_compare)
        {
            float start = Time.realtimeSinceStartup;
            indices = _earClipper.Triangulate(_points);
            float end = Time.realtimeSinceStartup;
            float processTime = end - start;


            start = Time.realtimeSinceStartup;
            var indicesNative = _earClipperNative.Triangulate(_points);
            end = Time.realtimeSinceStartup;
            float processTime2 = end - start;

            if(_type == TriangulateType.Native) indices = indicesNative;

            Debug.Log($"Manage: {processTime.ToString("F6")}, Native: {processTime2.ToString("F6")}, {(processTime/processTime2).ToString("F2")} times fast");
        }
        else
        {
            switch(_type)
                {
                    case TriangulateType.Manage:
                        indices = _earClipper.Triangulate(_points);
                        break;
                    case TriangulateType.Native:
                        indices = _earClipperNative.Triangulate(_points);
                        break;
                    default:
                        indices = _earClipper.Triangulate(_points);
                        break;
                }
        }

        var uv = new Vector2[_points.Count];
        var normals = new Vector3[_points.Count];

        _mesh.sharedMesh.vertices = _points.ToArray();
        _mesh.sharedMesh.SetIndices(indices, MeshTopology.Triangles, 0);
        _mesh.sharedMesh.SetNormals(normals);
        _mesh.sharedMesh.SetUVs(0, uv);
        _mesh.sharedMesh.bounds = new Bounds(Camera.main.transform.localPosition, new Vector3(10f, 10f, 10f));

#if UNITY_EDITOR
        // string data = $"#[cfg(test)] pub const POINTS: [f32;{_points.Count*3}] = [";
        // foreach(var v in _points)
        // {
        //     data += v.x;
        //     data += "f32,";
        //     data += v.y;
        //     data += "f32,";
        //     data += v.z;
        //     data += "f32,";
        // }
        // data = data.Substring(0, data.Length-1);
        // data += "];";
        
        // data += $"#[cfg(test)] pub const INDICES: [i32;{(_points.Count-2)*3}] = [";
        // foreach(var i in indices)
        // {
        //     data += i;
        //     data += ",";
        // }
        // data = data.Substring(0, data.Length-1);
        // data += "];";

        // string path = Application.dataPath;
        // path = System.IO.Path.Join(path.Substring(0, path.Length - 7), "/Plugin/ear_clipping/src/test_data.rs");
        // System.IO.File.WriteAllText(path, data);

        // _earClipperNative.Triangulate(_points);
#endif
    }
}
