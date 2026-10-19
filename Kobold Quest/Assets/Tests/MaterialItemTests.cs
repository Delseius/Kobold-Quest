using NUnit.Framework;
using UnityEngine;

public class MaterialItemTests
{
    private GameObject testObject;
    private MaterialItem materialItem;

    [SetUp]
    public void SetUp()
    {
        testObject = new GameObject("MaterialItemTestObject");
        materialItem = testObject.AddComponent<MaterialItem>();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(testObject);
    }

    [Test]
    public void MassIsCalculatedFromVolumeAndDensity()
    {
        materialItem.VolumeCubicMetres = 0.01f;
        materialItem.DensityKgPerCubicMetre = 535f;

        Assert.AreEqual(5.35f, materialItem.MassKg, 0.0001f);
    }

    [Test]
    public void ZeroVolumeProducesZeroMass()
    {
        materialItem.VolumeCubicMetres = 0f;
        materialItem.DensityKgPerCubicMetre = 535f;

        Assert.AreEqual(0f, materialItem.MassKg);
    }

    [Test]
    public void NegativeVolumeIsClampedToZero()
    {
        materialItem.VolumeCubicMetres = -1f;

        Assert.AreEqual(0f, materialItem.VolumeCubicMetres);
        Assert.AreEqual(0f, materialItem.MassKg);
    }

    [Test]
    public void NegativeDensityIsClampedToZero()
    {
        materialItem.VolumeCubicMetres = 1f;
        materialItem.DensityKgPerCubicMetre = -535f;

        Assert.AreEqual(0f, materialItem.DensityKgPerCubicMetre);
        Assert.AreEqual(0f, materialItem.MassKg);
    }
}
