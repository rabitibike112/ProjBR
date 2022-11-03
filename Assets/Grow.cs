using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grow : MonoBehaviour
{
    [SerializeField]
    private GameObject Empty;
    [SerializeField]
    private GameObject Helper;
    [SerializeField]
    private Color BaseColor;
    [SerializeField]
    private Color ExpandedColor;
    private GameObject InstObj;
    public Vector3[] baseVertices;
    public Vector3 Center;

    public void GrowObject()
    {
        if (Links.UiControls_S.bDoarConvex == true) // doar convexe
        {
            if (Links.UiControls_S.bPastrForm == true) // si se pastreaza forma
            {
                if (Links.UiControls_S.Uniform == true) // si creste uniform pe toate axele
                {
                    GrowCaseConvexPastrareformaUniform();
                }
                else
                {
                    GrowCaseConvexPastrareformaNeuniform();
                }
            }
            else // nu pastreaza forma
            {
                if (Links.UiControls_S.Uniform == true) // si creste uniform pe toate axele
                {
                    // intreaba de colturi
                }
                else
                {

                }
            }
        }
        else
        {

        }
    }

    private void GrowCaseConvexPastrareformaUniform()
    {
        Vector3[] Processed = Convexize(baseVertices);
        Vector3[] Grown = GrowUniform(Processed);
        Vector3[] ExtraCorners = AddSharpCorners(Grown);
        Vector3[] GrownAndCorners = SortVertsByAngle(AddVectorArraysTogether(Grown, ExtraCorners));
        Vector3[] WorkingVerts = AddBackZero(GrownAndCorners);
        GenerateTheMesh(WorkingVerts);
    }
    private void GrowCaseConvexPastrareformaNeuniform()
    {
        Vector3[] Processed = Convexize(baseVertices);
        Vector3[] tempGrown = GrowUniformConstant(Processed, 0.02f);
        Vector3[] ExtraCorners = AddSharpCorners(tempGrown);
        Vector3[] GrownAndCorners = SortVertsByAngle(AddVectorArraysTogether(tempGrown, ExtraCorners));
        Vector3[] finalGrown = GrowNeuniform(GrownAndCorners);
        Vector3[] WorkingVerts = AddBackZero(finalGrown);
        GenerateTheMesh(WorkingVerts);
    }
    private float GetAngleABC(Vector3 a, Vector3 b, Vector3 c)
    {
        Vector3 AB = new Vector3(b.x - a.x, b.y - a.y, b.z - a.z);
        Vector3 BC = new Vector3(c.x - b.x, c.y - b.y, c.z - b.z);

        float ABVec = Mathf.Sqrt(AB.x * AB.x + AB.y * AB.y + AB.z * AB.z);
        float BCVec = Mathf.Sqrt(BC.x * BC.x + BC.y * BC.y + BC.z * BC.z);

        Vector3 ABNorm = new Vector3(AB.x / ABVec, AB.y / ABVec, AB.z / ABVec);
        Vector3 BCNorm = new Vector3(BC.x / BCVec, BC.y / BCVec, BC.z / BCVec);

        float Res = ABNorm.x * BCNorm.x + ABNorm.y * BCNorm.y + ABNorm.z * BCNorm.z;
        return Mathf.Acos((Res * 180) / 3.141592f);
    }
    private float PolygonAreaCalculator(Vector3[] Verts)
    {
        float SideA = 0;
        float SideB = 0;
        float[] VectorX = new float[Verts.Length + 1];
        float[] VectorY = new float[Verts.Length + 1];
        for (int x1 = 0; x1 < Verts.Length; x1++)
        {
            VectorX[x1] = Verts[x1].x;
            VectorY[x1] = Verts[x1].y;
        }
        VectorX[VectorX.Length - 1] = Verts[0].x;
        VectorY[VectorY.Length - 1] = Verts[0].y;
        for (int x2 = 0; x2 < VectorX.Length - 1; x2++)
        {
            SideA += VectorX[x2] * VectorY[x2 + 1];
        }
        for (int x3 = 0; x3 < VectorY.Length - 1; x3++)
        {
            SideB += VectorY[x3] * VectorX[x3 + 1];
        }
        float Final = SideA - SideB;
        Final = Final / 2;
        return Mathf.Abs(Final);
    }
    private List<Vector3> RemoveIndex(Vector3[] Verts, int index)
    {
        List<Vector3> Temp = new List<Vector3>();
        for (int x1 = 0; x1 < Verts.Length; x1++)
        {
            if (x1 != index)
            {
                Temp.Add(Verts[x1]);
            }
        }
        return Temp;
    }
    private Vector3[] AddBackZero(Vector3[] Verts)
    {
        Vector3[] Temp = new Vector3[Verts.Length + 1];
        Temp[0] = Vector3.zero;
        for (int x1 = 0; x1 < Verts.Length; x1++)
        {
            Temp[x1 + 1] = Verts[x1];
        }
        return Temp;
    }
    private float PorportionCalculator(float Max, float Min, float Current)
    {
        Max -= Min;
        Current -= Min;
        return ((Current * 100f) / Max) / 100f;
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
    private Vector3[] SortVertsByAngle(Vector3[] Verts)
    {
        int len = Verts.Length;
        for (int x1 = 0; x1 < len; x1++)
        {
            for (int x2 = 0; x2 < len - x1 - 1; x2++)
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
    private Vector3 GetVectorIntersectTwoLines(Vector3 PointLineA1, Vector3 PointLineA2, Vector3 PointLineB1, Vector3 PointLineB2)
    {
        float SlopeA, SlopeB;
        float InterceptA, InterceptB;
        GetLineEquiationOfTwoPoints(PointLineA1, PointLineA2, out SlopeA, out InterceptA);
        GetLineEquiationOfTwoPoints(PointLineB1, PointLineB2, out SlopeB, out InterceptB); //X = IntB-IntA / SlopeA-SlopeB
        float x = (InterceptB - InterceptA) / (SlopeA - SlopeB);
        float y = SlopeA * x + InterceptA;
        return new Vector3(x, y, 0);
    }
    private void GetLineEquiationOfTwoPoints(Vector3 PointA, Vector3 PointB, out float Slope, out float Intercept)
    {
        float x1, x2, y1, y2;
        x1 = PointA.x;
        x2 = PointB.x;
        y1 = PointA.y;
        y2 = PointB.y;

        Slope = (y2 - y1) / (x2 - x1);
        Intercept = y1 - (x1 * Slope);
    }
    private bool IsPointOnLine(Vector3 LinePointA, Vector3 LinePointB, Vector3 LineToBeChecked)
    {
        float x1, x2, y1, y2;
        x1 = LinePointA.x;
        x2 = LinePointB.x;
        y1 = LinePointA.y;
        y2 = LinePointB.y;

        float Slope = (y2 - y1) / (x2 - x1);
        float Intercept = y1 - (x1 * Slope);

        if (FloatApproxEq(LineToBeChecked.y, Slope * LineToBeChecked.x + Intercept)) //LineToBeChecked.y == Slope * LineToBeChecked.x + Intercept
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    private bool FloatApproxEq(float a, float b)
    {
        if (Mathf.Abs(a - b) < 0.005f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    private Vector3[] AddListTogether(List<Vector3> a, List<Vector3> b)
    {
        Vector3[] temp = new Vector3[a.Count + b.Count];
        for (int x1 = 0; x1 < a.Count + b.Count; x1++)
        {
            if (x1 < a.Count)
            {
                temp[x1] = a.ToArray()[x1];
            }
            else
            {
                temp[x1] = b.ToArray()[x1 - a.Count];
            }
        }
        return temp;
    }
    private Vector3[] Convexize(Vector3[] ToConvexize)
    {
        Vector3[] WorkingVerts = RemoveIndex(ToConvexize, 0).ToArray(); //convexize
        bool OneFound = true;
        while (WorkingVerts.Length > 4 && OneFound == true)
        {
            OneFound = false;
            float Base = PolygonAreaCalculator(WorkingVerts);
            for (int x1 = 0; x1 < WorkingVerts.Length; x1++)
            {
                float New = PolygonAreaCalculator(RemoveIndex(WorkingVerts, x1).ToArray());
                if (New > Base)
                {
                    WorkingVerts = RemoveIndex(WorkingVerts, x1).ToArray();
                    OneFound = true;
                    break;
                }
            }
        }
        return WorkingVerts;
    }
    private Vector3[] GrowUniform(Vector3[] ToGrow)
    {
        float Multiplier;
        List<Vector3> NewVerts = new List<Vector3>();
        if (float.TryParse(Links.UiControls_S.UnifScale.text, out Multiplier) != true)
        {
            Multiplier = 0.2f;
        }
        if (Multiplier > 1f || Multiplier < 0.1f)
        {
            Multiplier = 0.2f;
        }
        for (int x3 = 0; x3 <= ToGrow.Length - 1; x3++)
        {
            if (x3 < ToGrow.Length - 1)
            {
                Vector3 Direction = Quaternion.Euler(0, 0, 90) * (ToGrow[x3 + 1] - ToGrow[x3]);
                Direction = Vector3.Normalize(Direction);
                Vector3 temp1 = ToGrow[x3] + Direction * Multiplier;
                Vector3 temp2 = ToGrow[x3 + 1] + Direction * Multiplier;
                NewVerts.Add(temp1);
                NewVerts.Add(temp2);
            }
            else
            {
                Vector3 Direction = Quaternion.Euler(0, 0, 90) * (ToGrow[0] - ToGrow[ToGrow.Length - 1]);
                Direction = Vector3.Normalize(Direction);
                Vector3 temp1 = ToGrow[x3] + Direction * Multiplier;
                Vector3 temp2 = ToGrow[0] + Direction * Multiplier;
                NewVerts.Add(temp1);
                NewVerts.Add(temp2);
            }
        }
        return NewVerts.ToArray();
    }
    private Vector3[] GrowUniformConstant(Vector3[]ToGrow,float Const)
    {
        float Multiplier = Const;
        List<Vector3> NewVerts = new List<Vector3>();
        for (int x3 = 0; x3 <= ToGrow.Length - 1; x3++)
        {
            if (x3 < ToGrow.Length - 1)
            {
                Vector3 Direction = Quaternion.Euler(0, 0, 90) * (ToGrow[x3 + 1] - ToGrow[x3]);
                Direction = Vector3.Normalize(Direction);
                Vector3 temp1 = ToGrow[x3] + Direction * Multiplier;
                Vector3 temp2 = ToGrow[x3 + 1] + Direction * Multiplier;
                NewVerts.Add(temp1);
                NewVerts.Add(temp2);
            }
            else
            {
                Vector3 Direction = Quaternion.Euler(0, 0, 90) * (ToGrow[0] - ToGrow[ToGrow.Length - 1]);
                Direction = Vector3.Normalize(Direction);
                Vector3 temp1 = ToGrow[x3] + Direction * Multiplier;
                Vector3 temp2 = ToGrow[0] + Direction * Multiplier;
                NewVerts.Add(temp1);
                NewVerts.Add(temp2);
            }
        }
        return NewVerts.ToArray();
    }
    private Vector3[] GrowNeuniform(Vector3[] ToGrow)
    {
        float MultiXp, MultiXm, MultiYp, MultiYm;
        if (float.TryParse(Links.UiControls_S.NeUnifScalePX.text, out MultiXp) != true)
        {
            MultiXp = 0.2f;
        }
        if (float.TryParse(Links.UiControls_S.NeUnifScaleMX.text, out MultiXm) != true)
        {
            MultiXm = 0.2f;
        }
        if (float.TryParse(Links.UiControls_S.NeUnifScalePY.text, out MultiYp) != true)
        {
            MultiYp = 0.2f;
        }
        if (float.TryParse(Links.UiControls_S.NeUnifScaleMY.text, out MultiYm) != true)
        {
            MultiYm = 0.2f;
        }
        if (MultiXp > 1.5f || MultiXp < 0.02f)
        {
            MultiXp = 0.02f;
        }
        if (MultiXm > 1.5f || MultiXm < 0.02f)
        {
            MultiXm = 0.02f;
        }
        if (MultiYp > 1.5f || MultiYp < 0.02f)
        {
            MultiYp = 0.02f;
        }
        if (MultiYm > 1.5f || MultiYm < 0.02f)
        {
            MultiYm = 0.02f;
        }
        List<Vector3> NewVerts = new List<Vector3>();
        for (int x1 = 0; x1 < ToGrow.Length; x1++)
        {
            Vector3 Dir = Vector3.Normalize(ToGrow[x1]);
            NewVerts.Add(GetCorrectMultiplierBasedOnAngle(ToGrow[x1], MultiXp, MultiXm, MultiYp, MultiYm));
        }
        return NewVerts.ToArray();
    }
    private Vector3[] AddSharpCorners(Vector3[] BaseCorners)
    {
        List<Vector3> Corners = new List<Vector3>();
        for (int x4 = 0; x4 < BaseCorners.Length; x4 += 2)
        {
            if (x4 < BaseCorners.Length - 2)
            {
                Vector3 newTemp = GetVectorIntersectTwoLines(BaseCorners[x4], BaseCorners[x4 + 1], BaseCorners[x4 + 2], BaseCorners[x4 + 3]);
                Corners.Add(newTemp);
            }
            else
            {
                Vector3 newTemp = GetVectorIntersectTwoLines(BaseCorners[x4], BaseCorners[x4 + 1], BaseCorners[0], BaseCorners[1]);
                Corners.Add(newTemp);
            }
        }
        return Corners.ToArray();
    }
    private Vector3[] AddVectorArraysTogether(Vector3[] Arr1, Vector3[] Arr2)
    {
        Vector3[] temp = new Vector3[Arr1.Length + Arr2.Length];
        for (int x1 = 0; x1 < Arr1.Length + Arr2.Length; x1++)
        {
            if (x1 < Arr1.Length)
            {
                temp[x1] = Arr1[x1];
            }
            else
            {
                temp[x1] = Arr2[x1 - Arr1.Length];
            }
        }
        return temp;
    }
    private void GenerateTheMesh(Vector3[] WorkingVerts)
    {
        Mesh mesh;
        MeshFilter meshFilter;
        InstObj = Instantiate(Empty, Center, Quaternion.identity);
        int[] newTriangles = new int[6 + (WorkingVerts.Length - 3) * 3];
        int TriangleIndex = 0;
        for (int x2 = 1; x2 < WorkingVerts.Length; x2++)
        {
            newTriangles[TriangleIndex] = 0;
            TriangleIndex += 1;
            newTriangles[TriangleIndex] = x2;
            TriangleIndex += 1;
            if (x2 == WorkingVerts.Length - 1)
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
        mesh.vertices = WorkingVerts;
        mesh.triangles = newTriangles;
        InstObj.GetComponent<Renderer>().material.color = ExpandedColor;
        InstObj.transform.position = InstObj.transform.position + new Vector3(0, 0, 0.01f);
        Instantiate(Helper, InstObj.transform.position - new Vector3(0, 0, 0.05f), Quaternion.identity);
    }
    private float RuleOf3Calculator(float max,float current)
    {
        return current * 100f / max;
    }
    private Vector3 GetCorrectMultiplierBasedOnAngle(Vector3 VectorToGrow,float PX,float MX,float PY,float MY)
    {
        Vector3 TempVec3;
        float Angle = GetAngleVectors(Vector3.up, VectorToGrow);
        if (Angle >= 0f && Angle < 90f) //px py
        {
            float temp = RuleOf3Calculator(90, Angle) / 100f;
            TempVec3 = (temp * PX * Vector3.right) + ((1 - temp) * PY * Vector3.up);
        }
        else if(Angle>=90f && Angle < 180f) //px my
        {
            float temp = RuleOf3Calculator(90, Angle-90)/100f;
            TempVec3 = (temp * MY * Vector3.down) + ((1 - temp) * PX * Vector3.right);
        }
        else if(Angle>=180f && Angle < 270f) //mx my
        {
            float temp = RuleOf3Calculator(90, Angle-180)/100f;
            TempVec3 = (temp * MX * Vector3.left) + ((1 - temp) * MY * Vector3.down);
        }
        else //mx py
        {
            float temp = RuleOf3Calculator(90, Angle-270)/100f;
            TempVec3 = (temp * PY * Vector3.up) + ((1 - temp) * MX * Vector3.left);
        }
        TempVec3 = TempVec3 + VectorToGrow;
        return TempVec3;
    }
    /*List<Vector3> NewVerts = new List<Vector3>();
        for(int x3 = 0; x3 <= WorkingVerts.Length-1; x3++)
        {
            if (x3 < WorkingVerts.Length-1)
            {
                Vector3 Direction = Quaternion.Euler(0, 0, 90) * (WorkingVerts[x3 + 1] - WorkingVerts[x3]);
                Direction = Vector3.Normalize(Direction);
                Vector3 temp1 = WorkingVerts[x3] + Direction * 0.2f;
                Vector3 temp2 = WorkingVerts[x3 + 1] + Direction * 0.2f;
                NewVerts.Add(temp1);
                NewVerts.Add(temp2);
            }
            else
            {
                Vector3 Direction = Quaternion.Euler(0, 0, 90) * (WorkingVerts[0] - WorkingVerts[WorkingVerts.Length-1]);
                Direction = Vector3.Normalize(Direction);
                Vector3 temp1 = WorkingVerts[x3] + Direction * 0.2f;
                Vector3 temp2 = WorkingVerts[0] + Direction * 0.2f;
                NewVerts.Add(temp1);
                NewVerts.Add(temp2);
            }
        }*/

    /*
    Vector3[] tempArray = new Vector3[NewVerts.Count + NewVerts.Count / 2];
    List<Vector3> tempList = new List<Vector3>();
        for (int x4 = 1; x4<NewVerts.Count - 1; x4 += 2)
        {
            Vector3 temp = ((NewVerts.ToArray()[x4] + NewVerts.ToArray()[x4 + 1]) / 2);
    temp = temp + Vector3.Normalize(temp)* ((Vector3.Distance(NewVerts.ToArray()[x4], NewVerts.ToArray()[x4 + 1])) / 2);
            tempList.Add(temp);
        }

tempArray = AddListTogether(tempList, NewVerts);
tempArray = SortVertsByAngle(tempArray);

foreach (Vector3 x in tempArray)
{
    Instantiate(Helper, x, Quaternion.identity);
}

WorkingVerts = AddBackZero(tempArray);*/
}
