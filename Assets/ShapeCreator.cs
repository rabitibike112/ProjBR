using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeCreator : MonoBehaviour
{
    [SerializeField]
    private GameObject Circle;
    [SerializeField]
    private GameObject Empty;
    private GameObject InstObj;
    private bool Started = false;
    private List<Vector3> Verts = new List<Vector3>();
    private List<GameObject> VertCircles = new List<GameObject>();
    private Vector3 Center;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1) && Started == true)
        {
            if (Verts.Count >= 4)
            {
                GenerateObject();
            }
            Started = false;
            Verts.Clear();
            foreach (GameObject x in VertCircles)
            {
                Destroy(x);
            }
            VertCircles.Clear();
        }

        if (Started == true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                VertCircles.Add(Instantiate(Circle, this.transform.position, Quaternion.identity));
                Verts.Add(transform.position);
            }
        }
    }

    public void StartBuild()
    {
        Started = true;
    }

    private void GenerateObject()
    {
        CalcCenter();
        Mesh mesh;
        MeshFilter meshFilter;
        InstObj = Instantiate(Empty, Center, Quaternion.identity);
        Vector3[] newVertices = new Vector3[Verts.Count + 1];
        newVertices[0] = new Vector3(0, 0, 0);
        for (int x1 = 1; x1 <= Verts.Count; x1++)
        {
            newVertices[x1] = Verts.ToArray()[x1 - 1] - Center;
        }
        newVertices = SortVertsByAngle(newVertices);
        int[] newTriangles = new int[6 + (newVertices.Length - 3) * 3];
        int TriangleIndex = 0;
        for (int x2 = 1; x2 < newVertices.Length; x2++)
        {
            newTriangles[TriangleIndex] = 0;
            TriangleIndex += 1;
            newTriangles[TriangleIndex] = x2;
            TriangleIndex += 1;
            if (x2 == newVertices.Length - 1)
            {
                newTriangles[TriangleIndex] = 1;
                TriangleIndex += 1;
            }
            else
            {
                newTriangles[TriangleIndex] = x2 + 1;
                TriangleIndex += 1;
            }
        }
        mesh = new Mesh();
        meshFilter = InstObj.gameObject.GetComponent<MeshFilter>();
        meshFilter.mesh = mesh;
        mesh.vertices = newVertices;
        mesh.triangles = newTriangles;
        InstObj.transform.SetParent(GameObject.Find("ToGrow").transform);
        InstObj.GetComponent<Grow>().baseVertices = newVertices;
        InstObj.GetComponent<Grow>().Center = Center;
    }

    private void CalcCenter()
    {
        float x = 0, y = 0, z = 0;
        foreach (Vector3 temp in Verts)
        {
            x += temp.x;
            y += temp.y;
            z += temp.z;
        }
        x /= Verts.Count;
        y /= Verts.Count;
        z /= Verts.Count;
        Center = new Vector3(x, y, z);
    }
    private float GetAngleVectors(Vector3 Reference, Vector3 Real)
    {
        if (Real == Vector3.zero)
        {
            return 0f;
        }
        //Get the dot product
        float dot = Vector3.Dot(Reference, Real);
        // Divide the dot by the product of the magnitudes of the vectors
        dot = dot / (Reference.magnitude * Real.magnitude);
        //Get the arc cosin of the angle, you now have your angle in radians 
        var acos = Mathf.Acos(dot);
        //Multiply by 180/Mathf.PI to convert to degrees
        var angle = acos * 180 / Mathf.PI;
        //Congrats, you made it really hard on yourself.
        if (Real.x < 0)
        {
            angle = 360 - angle;
        }
        return angle;
    }
    private Vector3[] SortVertsByAngle(Vector3[]Verts)
    {
        int len = Verts.Length;
        for(int x1 = 0; x1 < len; x1++)
        {
            for(int x2 = 0; x2 < len - x1 - 1; x2++)
            {
                if (GetAngleVectors(Vector3.up, Verts[x2]) > GetAngleVectors(Vector3.up, Verts[x2 + 1]))
                {
                    Vector3 temp = Verts[x2];
                    Verts[x2] = Verts[x2 + 1];
                    Verts[x2 + 1] = temp;
                }
            }
        }
        return Verts;
    }
    /*
    private void GenerateMesh()
    {
        Mesh mesh;
        MeshFilter meshFilter;
        Vector3[] newVertices;
        int[] newTriangles;
        newVertices = new Vector3[Verts.Count];
        for (int x1 = 1; x1 < Verts.Count; x1++)
        {
            newVertices[x1] = Verts.ToArray()[x1];
        }
        int NumberOfTriangles = 6 + (newVertices.Length - 3) * 3;
        newTriangles = new int[NumberOfTriangles];//4->9 5->12 6->15 7->18
        int TriangleIndex = 0;
        for (int x2 = 1; x2 < newVertices.Length; x2++)
        {
            newTriangles[TriangleIndex] = 0;
            TriangleIndex += 1;
            newTriangles[TriangleIndex] = x2;
            TriangleIndex += 1;
            if (x2 == newVertices.Length - 1)
            {
                newTriangles[TriangleIndex] = 1;
                TriangleIndex += 1;
            }
            else
            {
                newTriangles[TriangleIndex] = x2 + 1;
                TriangleIndex += 1;
            }
        }
        mesh = new Mesh();
        meshFilter = InstObj.gameObject.GetComponent<MeshFilter>();
        meshFilter.mesh = mesh;
        mesh.vertices = newVertices;
        mesh.triangles = newTriangles;
        foreach (GameObject x in Helpers)
        {
            Destroy(x);
        }
        Helpers.Clear();
        InstObj.transform.SetParent(Ref.UserBuiltGroup.transform);
        InstObj.transform.tag = "AREA";
        InstObj.AddComponent<Area>();
        InstObj.transform.position = Center;
        InstObj.GetComponent<Renderer>().material.color = GameObject.Find("Canvas").transform.GetComponent<ColorPicker>().CurrectColor;
        InstObj.GetComponent<Area>().Color = GameObject.Find("Canvas").transform.GetComponent<ColorPicker>().CurrentColorId;
        foreach (Vector3 x in Verts)
        {
            InstObj.GetComponent<Area>().Vertexes.Add(new Vector2(x.x, x.y));
        }
        GameObject temp = Instantiate(WorldText, null);
        temp.transform.position = Center;
        temp.transform.SetParent(InstObj.transform);
        InstObj = null;
    }
    */
}
