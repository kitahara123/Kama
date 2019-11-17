using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StackController : MonoBehaviour
{
    [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private Button moreButton;
    [SerializeField] private Button lessButton;
    private Mesh mesh;

    void Start()
    {
        moreButton.onClick.AddListener(PlusChip);
        lessButton.onClick.AddListener(MinusChip);
        mesh = meshFilter.mesh;
    }

    private void PlusChip()
    {
        if (mesh == null) return;
        var vertices = mesh.vertices;
        var shiftedVertices = new List<Vector3>();

        for (int i = 0; i < vertices.Length; i++)
        {
            var v = vertices[i];
            var newV = new Vector3(v.x, v.y, v.z + 0.0035f);
            shiftedVertices.Add(newV);
        }

        var res = new Vector3[vertices.Length + shiftedVertices.Count];
        vertices.CopyTo(res, 0);
        shiftedVertices.CopyTo(res, vertices.Length);
        var resUV = new Vector2[res.Length];
        mesh.uv.CopyTo(resUV, 0);
        mesh.uv.CopyTo(resUV, mesh.uv.Length);

        var resTri = new int[mesh.triangles.Length * 2];
        mesh.triangles.CopyTo(resTri, 0);

        for (int i = mesh.triangles.Length; i < resTri.Length; i++)
        {
            var vIndex = mesh.triangles[i - mesh.triangles.Length];
            resTri[i] = vIndex + vertices.Length;
        }
       
        mesh.vertices = res;
        mesh.uv = resUV;
        mesh.triangles = resTri;
        
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        
        print(res.Length);
        print(mesh.uv.Length);
        print(mesh.triangles.Length);

//        foreach (var vector2 in mesh.uv)
//        {
//            print(vector2);
//        }

//        ShowVertices();
    }

    private void ShowVertices()
    {
        foreach (var vertex in mesh.vertices)
        {
            var tmp = meshFilter.gameObject.transform.TransformPoint(vertex);
            var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            go.transform.localPosition = tmp;
        }
    }

    private void MinusChip()
    {
        if (mesh == null) return;
        var vertices = mesh.vertices;

        var topVertices = new HashSet<int>();

        for (var i = 0; i < mesh.triangles.Length; i += 3)
        {
            var vIndex1 = mesh.triangles[i];
            var vIndex2 = mesh.triangles[i + 1];
            var vIndex3 = mesh.triangles[i + 2];

            var vertex1 = vertices[vIndex1];
            var vertex2 = vertices[vIndex2];
            var vertex3 = vertices[vIndex3];
//            topVertices.Add(vIndex1);
//            topVertices.Add(vIndex2);
//            topVertices.Add(vIndex3);

            if (Mathf.Abs(vertex1.z - vertex2.z) > 0.001f || Mathf.Abs(vertex2.z - vertex3.z) > 0.001f ||
                Mathf.Abs(vertex1.z - vertex3.z) > 0.001f)
            {
                if (vertex1.z > vertex2.z && vertex1.z > vertex3.z)
                {
                    topVertices.Add(vIndex1);
                }
                else if (vertex2.z > vertex1.z && vertex2.z > vertex3.z)
                {
                    topVertices.Add(vIndex2);
                }
                else if (vertex3.z > vertex1.z && vertex3.z > vertex2.z)
                {
                    topVertices.Add(vIndex3);
                }
            }
        }

        print(topVertices.Count);

        foreach (var vertex in topVertices)
        {
            var lastVertex = vertices[vertex];
            var newVertex = new Vector3(lastVertex.x, lastVertex.y, lastVertex.z - 0.02f);

            var tmp = meshFilter.gameObject.transform.TransformPoint(newVertex);
            var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            go.transform.localPosition = tmp;

            vertices[vertex] = newVertex;
        }

        mesh.vertices = vertices;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
    }
}