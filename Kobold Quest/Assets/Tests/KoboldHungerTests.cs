using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class KoboldHungerTests
{
    [Test]
    public void CanAccessKoboldHunger()
    {
        GameObject kobold = new GameObject("Test Kobold");

        KoboldHunger hunger =
            kobold.AddComponent<KoboldHunger>();

        Assert.IsNotNull(hunger);

        Object.DestroyImmediate(kobold);
    }

    [Test]
    public void NewKoboldStartsWithFullCalories()
    {
        GameObject kobold = new GameObject("Test Kobold");

        KoboldHunger hunger =
            kobold.AddComponent<KoboldHunger>();

        Assert.AreEqual(
            hunger.maxCalories,
            hunger.currentCalories
        );

        Object.DestroyImmediate(kobold);
    }

    [Test]
    public void KoboldLosesCaloriesWhenCaloriesAreLost()
    {
        GameObject kobold = new GameObject("Test Kobold");

        KoboldHunger hunger =
            kobold.AddComponent<KoboldHunger>();

        hunger.LoseCalories(100f);

        Assert.AreEqual(1500f, hunger.currentCalories);

        Object.DestroyImmediate(kobold);
    }

    [Test]
    public void KoboldLosesCaloriesOverTime()
    {
        GameObject kobold = new GameObject("Test Kobold");

        KoboldHunger hunger =
            kobold.AddComponent<KoboldHunger>();

        hunger.LoseCaloriesOverTime(10f);

        Assert.AreEqual(1590f, hunger.currentCalories);

        Object.DestroyImmediate(kobold);
    }
}