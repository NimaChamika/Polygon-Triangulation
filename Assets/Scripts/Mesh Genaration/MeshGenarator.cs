using System.Collections.Generic;
using UnityEngine;
 
public class MeshGenarator : MonoBehaviour
{
    #region PROPERTIES
    public static MeshGenarator Instance { get; private set; }
    Mesh mesh;
    #endregion

    #region UNITY CALLBACKS
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    #endregion

    #region UTIL
    internal void GenarateMesh()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        mesh.Clear();
        //mesh = GetComponent<MeshFilter>().sharedMesh;

        List<Vector3> vertsList = new List<Vector3>();//VERTICES LIST
        List<int> triList = new List<int>();//TRIANGLE LIST
        List<Vector3> normalList = new List<Vector3>(); //NORMAL LIST (WE NEED NORMALS IF NEED LIGTING TO WORK ON THE MESH)
        List<Vector2> uvList = new List<Vector2>();//MAP TEXTURES

        AddVertices(vertsList, normalList, uvList);
        AddTriangles(triList);

        //mesh.vertices = vertsList.ToArray();//SETTING PROPERTIES MAKE MORE GARBAGE
        //mesh.triangles = triList.ToArray();
        //mesh.normals = normalList.ToArray();

        mesh.SetVertices(vertsList);
        mesh.triangles = triList.ToArray();
        mesh.SetNormals(normalList);
        //mesh.SetUVs(0, uvList);

        //mesh.RecalculateNormals();//THIS IS AN EXPENSIVE OPERATION. SHOULD NOT BE USED WHEN THE MESH IS GENAREATED AGAIN ANF AGAIN.
    }

    private void AddVertices(List<Vector3> vertsList, List<Vector3> normalList, List<Vector2> uvList)
    {
        
        foreach(Vector2 v in EarClippingTriangulation.Instance.vertexVList)
        {
            vertsList.Add(new Vector3(v.x,0,v.y));
            normalList.Add(Vector3.down);
        }

        //uvList.Add(new Vector2(0, 0));
        //uvList.Add(new Vector2(0, 1));
        //uvList.Add(new Vector2(1, 0));
        //uvList.Add(new Vector2(1, 1));
    }

    private void AddTriangles(List<int> triList)
    {
        //LEFT HAND COORDINATE SYSTEM
        //WINDING ORDER IS IMPORATANT. IN A QUAD ONLY ONE SIDE WILL GET RENDRED.

        foreach (int index in EarClippingTriangulation.Instance.trianglePointIndexList)
        {
            Debug.Log(index);
            triList.Add(index);
        }
    }
    #endregion
}