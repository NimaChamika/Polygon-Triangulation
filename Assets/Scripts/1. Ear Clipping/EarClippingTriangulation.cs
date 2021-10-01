using System.Collections.Generic;
using System.Linq;
using UnityEngine;
 
public class EarClippingTriangulation : MonoBehaviour
{
    #region PROPERTIES
    public static EarClippingTriangulation Instance { get; private set; }
    [SerializeField] private Transform[] vertexTArray;

    internal List<Vector2>  vertexVList { get; private set; }
    private List<Vector2> tempVertexVList;
    internal List<int> trianglePointIndexList { get; private set; }
    #endregion

    #region UNITY CALLBACKS
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        vertexVList = vertexTArray.Select(t => new Vector2(t.position.x,t.position.z)).ToList();//VERTEX COUNT -> n

        tempVertexVList = new List<Vector2>();
        tempVertexVList.AddRange(vertexVList);

        trianglePointIndexList = new List<int>();//TRIANGLE COUNT -> (n-2)

        FindAllTriangles();
        MeshGenarator.Instance.GenarateMesh();
    }
    #endregion

    #region EAR CLIPPING METHODS
    //https://arxiv.org/ftp/arxiv/papers/1212/1212.6038.pdf
    private void FindAllTriangles()
    {

        while(tempVertexVList.Count > 2)
        {
            for(int i=0;i< tempVertexVList.Count;i++)
            {
                Vector2 _a = tempVertexVList[GetNextIndex(i)];
                Vector2 _b = tempVertexVList[i];
                Vector2 _c = tempVertexVList[GetPreviousIndex(i)];

                if(IsConvexVertex(_a,_b,_c))
                {
                    if(IsAValidEar(_a,_b,_c))
                    {
                        tempVertexVList.Remove(_b);
                        trianglePointIndexList.Add(vertexVList.IndexOf(_b));
                        trianglePointIndexList.Add(vertexVList.IndexOf(_a));
                        trianglePointIndexList.Add(vertexVList.IndexOf(_c));
                        break;
                    }
                }
            
            }
        }

        //Debug.Log(trianglePointIndexList.Count);
    }


    //USING DETERMINATE
    //https://stackoverflow.com/questions/40410743/polygon-triangulation-reflex-vertex
    private bool IsConvexVertex(Vector2 a, Vector2 b, Vector2 c)
    {
        //CONVEX VERTEX - VETREX ANGLE IS LESS THAN 180 DEGRESS.

        //The determinate gives us as result greater zero if the vertices form a left turn, which means that three consecutive vertices a,b, and c form a convex angle at b; and less than zero otherwise.
        //(b.x - a.x) * (c.y - b.y) - (c.x - b.x) * (b.y - a.y) > 0

        float d1 = (b.x - a.x) * (c.y - b.y);
        float d2 = (c.x - b.x) * (b.y - a.y);

        if(d1 - d2 > 0)
        {
            return true;
        }
        //EDGE CASE - 180

        return false;
    }

    private int GetNextIndex(int index)
    {
        //return index + 1 < vertexList.Count ? index + 1 : (index + 1) % vertexList.Count; 
        return index + 1 < tempVertexVList.Count ? index + 1 : 0;
    }

    private int GetPreviousIndex(int index)
    {
        return index - 1 >= 0 ? index - 1 : tempVertexVList.Count-1;
    }

   

    private bool IsAValidEar(Vector2 a, Vector2 b, Vector2 c)
    {
        for (int i = 0; i < tempVertexVList.Count; i++)
        {
            if (tempVertexVList[i] != a || tempVertexVList[i] != b || tempVertexVList[i] != c)
            {
                if(IsPointInTriangle(a,b,c,tempVertexVList[i]))
                {
                    return false;
                }
            }
        }

        return true;
    }

    //https://blackpawn.com/texts/pointinpoly/
    private bool IsPointInTriangle(Vector2 a, Vector2 b, Vector2 c,Vector2 p)
    {
        bool bool1 = IsSameSide(a, b, c, p);
        bool bool2 = IsSameSide(b, c, a, p);
        bool bool3 = IsSameSide(c, a, b, p);

        if(bool1 && bool2 && bool3)
        {
            return true;
        }

        return false;
    }

    private bool IsSameSide(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
    {
        float f1 = CrossProductSignedMagnitude(p1 - p2,p3 - p2);
        float f2 = CrossProductSignedMagnitude(p1 - p2, p4 - p2);

        //IF HAS THE SAME SIGN, THEN THEY ARE IN THE SAME SIDE
        if(f1 < 0)
        {
            if(f2 < 0)
            {
                return true;
            }
        }
        else
        {
            if (f2 > 0)
            {
                return true;
            }
        }

        return false;
    }

    private float CrossProductSignedMagnitude(Vector2 a, Vector2 b)
    {
        return (a.x * b.y - a.y * b.x);
    }
    #endregion
}