using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class KoboldHungerTests
{
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
}