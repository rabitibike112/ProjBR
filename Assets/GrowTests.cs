using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class GrowTests : Grow
{
    [Test]
    [Category("Pass")]
    public void ConcaveSquareToSquare1()
    {
        Vector3[] InitialVerts = { new Vector3(0, 0), new Vector3(1, 1), new Vector3(0.1f, 0)/*in interior*/, new Vector3(1, -1), new Vector3(-1, -1), new Vector3(-1, 1) };
        Vector3[] SortedVerts = SortVertsByAngle(InitialVerts);
        Vector3[] toCheck = Convexize(SortedVerts);
        Assert.AreEqual(toCheck.Length, 4);
    }

    [Test]
    [Category("Pass")]
    public void ConcaveSquareToSquare2()
    {
        Vector3[] InitialVerts = { new Vector3(0, 0), new Vector3(1, 1), new Vector3(2.1f, 0)/*in exterior*/, new Vector3(1, -1), new Vector3(-1, -1), new Vector3(-1, 1) };
        Vector3[] SortedVerts = SortVertsByAngle(InitialVerts);
        Vector3[] toCheck = Convexize(SortedVerts);
        Assert.AreEqual(toCheck.Length, 5);
    }

    [Test]
    [Category("Pass")]
    public void ConcaveSquareToSquare3()
    {
        Vector3[] InitialVerts = { new Vector3(0, 0), new Vector3(1, 1), new Vector3(0.1f, 0)/*in interior*/, new Vector3(-0.1f,0)/*in interior*/, new Vector3(1, -1), new Vector3(-1, -1), new Vector3(-1, 1) };
        Vector3[] SortedVerts = SortVertsByAngle(InitialVerts);
        Vector3[] toCheck = Convexize(SortedVerts);
        Assert.AreEqual(toCheck.Length, 4);
    }

    [Test]
    [Category("Pass")]
    public void Star8ToSquare()
    {
        Vector3[] InitialVerts = { new Vector3(0, 0), new Vector3(0, 2), new Vector3(0.25f, 0.25f)/*colt intern*/, new Vector3(2, 0), new Vector3(0.25f, -0.25f)/*colt intern*/, new Vector3(0, -2),
            new Vector3(-0.25f, -0.25f)/*colt intern*/, new Vector3(-2, 0), new Vector3(-0.25f, 0.25f)/*colt intern*/ };
        Vector3[] SortedVerts = SortVertsByAngle(InitialVerts);
        Vector3[] toCheck = Convexize(SortedVerts);
        Assert.AreEqual(toCheck.Length, 4);
    }

    [Test]
    [Category("Pass")]
    public void Star8ToNotSquare()
    {
        Vector3[] InitialVerts = { new Vector3(0, 0), new Vector3(0, 2), new Vector3(1.25f, 1.25f)/*colt intern*/, new Vector3(2, 0), new Vector3(1.25f, -1.25f)/*colt intern*/, new Vector3(0, -2),
            new Vector3(-1.25f, -1.25f)/*colt intern*/, new Vector3(-2, 0), new Vector3(-1.25f, 1.25f)/*colt intern*/ };
        Vector3[] SortedVerts = SortVertsByAngle(InitialVerts);
        Vector3[] toCheck = Convexize(SortedVerts);
        Assert.AreEqual(toCheck.Length, 8);
    }

    [Test]
    [Category("Pass")]
    [Combinatorial]
    public void GetAngleVectorTest([Values(-2, -1, 0, 1, 2)] int x, [Values(-2, -1, 0, 1, 2)] int y) //must return a float number equal or larger than 0 but smaller than 360
    {
        Vector3 ReferenceVector = Vector3.up;
        Vector3 TestVector = new Vector3(x, y, 0);
        float Angle = GetAngleVectors(ReferenceVector, TestVector);
        Assert.GreaterOrEqual(Angle, 0);
        Assert.LessOrEqual(Angle, 359.999f);
    }

    [Test]
    [Category("Pass")]
    [Combinatorial]
    public void GetAngleVectorBorderTest([Values(-0.075f, -0.05f, -0.025f, 0, 0.025f, 0.05f, 0.075f)] float x, [Values(1)] float y) //must return a float number equal or larger than 0 but smaller than 360
    {
        Vector3 ReferenceVector = Vector3.up;
        Vector3 TestVector = new Vector3(x, y, 0);
        float Angle = GetAngleVectors(ReferenceVector, TestVector);
        Assert.GreaterOrEqual(Angle, 0);
        Assert.LessOrEqual(Angle, 359.999f);
    }
}
