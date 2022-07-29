using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshMaker
{
    Mesh mesh;
    Vector3[] vertices;
    LinkedList<GameObject> _meshObjs = new LinkedList<GameObject>();
    GameObject _meshObj;

    public void SetMesh()
    {
        Vector2[] uvs = new Vector2[mesh.vertices.Length];
        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(mesh.vertices[i].x, mesh.vertices[i].z);
        }
        mesh.uv = uvs;

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        _meshObj.AddComponent<MeshCollider>();
        _meshObjs.AddLast(_meshObj);
    }
    
    public void MakeMesh(Vector3[] vertices, int[] indexes, string meshTag, Material material)
    {
        _meshObj = new GameObject();
        mesh = _meshObj.AddComponent<MeshFilter>().mesh;
        _meshObj.AddComponent<MeshRenderer>().material = material;
        _meshObj.layer = LayerMask.NameToLayer("Area");
        _meshObj.tag = meshTag;
        mesh.Clear();
        mesh.vertices = vertices;
        this.vertices = vertices;
        mesh.triangles = indexes;

        _meshObj.AddComponent<AreaLine>().CreateLine(vertices);
        vertices = new Vector3[]
        {
            mesh.vertices[indexes[0]] + Vector3.up,
            mesh.vertices[indexes[1]] + Vector3.up,
            mesh.vertices[indexes[2]] + Vector3.up,
        };
        indexes = new int[]
        {
            0, 4, 3,
            0, 1, 4,
            1, 5, 4,
            1, 2, 5,
            2, 3, 5,
            2, 0, 3
        };
        //SetMesh();
        AddMesh(vertices, indexes);
    }
    public void AddMesh(Vector3[] vertices, int[] indexes)
    {
        List<int> miniIndex = new List<int>();
        List<Vector3> miniVertices = new List<Vector3>();
        miniIndex.AddRange(mesh.triangles);
        miniVertices.AddRange(mesh.vertices);
        foreach (Vector3 vec in vertices)
        {
            if(!miniVertices.Contains(vec))
                miniVertices.Add(vec);
        }
        miniIndex.AddRange(indexes);

        mesh.Clear();
        mesh.vertices = miniVertices.ToArray();
        mesh.triangles = miniIndex.ToArray();
        SetMesh();
    }

    public bool IsHaveVertex(Vector3 vertex)
    {
        foreach (Vector3 vec in vertices)
            if (vec == vertex)
                return true;
        return false;
    }
}
