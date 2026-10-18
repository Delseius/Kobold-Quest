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

        hunger.caloriesLostPerSecond = 10f;

        hunger.LoseCaloriesOverTime(1f);

        Assert.AreEqual(1590f, hunger.currentCalories);

        Object.DestroyImmediate(kobold);
    }


    [Test]
    public void KoboldLosesCorrectCaloriesOverElapsedTime()
    {
        GameObject kobold = new GameObject("Test Kobold");

        KoboldHunger hunger =
            kobold.AddComponent<KoboldHunger>();

        hunger.caloriesLostPerSecond = 10f;

        hunger.LoseCaloriesOverTime(5f);

        Assert.AreEqual(1550f, hunger.currentCalories);

        Object.DestroyImmediate(kobold);
    }

    [Test]
    public void CaloriesCannotFallBelowZero()
    {
        GameObject kobold = new GameObject("Test Kobold");

        KoboldHunger hunger =
            kobold.AddComponent<KoboldHunger>();

        hunger.LoseCalories(2000f);

        Assert.AreEqual(0f, hunger.currentCalories);

        Object.DestroyImmediate(kobold);
    }

    [Test]
    public void ConsumingCaloriesRestoresHunger()
    {
        GameObject kobold = new GameObject("Test Kobold");

        KoboldHunger hunger =
            kobold.AddComponent<KoboldHunger>();

        hunger.LoseCalories(100f);
        hunger.ConsumeCalories(50f);

        Assert.AreEqual(1550f, hunger.currentCalories);

        Object.DestroyImmediate(kobold);
    }

    [Test]
    public void ConsumingCaloriesCannotExceedMaximum()
    {
        GameObject kobold = new GameObject("Test Kobold");

        KoboldHunger hunger =
            kobold.AddComponent<KoboldHunger>();

        hunger.ConsumeCalories(500f);

        Assert.AreEqual(1600f, hunger.currentCalories);

        Object.DestroyImmediate(kobold);
    }

    [Test]
    public void EatingFoodRestoresCaloriesToConsumer()
    {
        GameObject kobold = new GameObject("Test Kobold");
        KoboldHunger hunger =
            kobold.AddComponent<KoboldHunger>();

        hunger.LoseCalories(100f);

        GameObject mushroom = new GameObject("Test Mushroom");
        TestFood food = mushroom.AddComponent<TestFood>();
        food.SetCalories(25f);

        LogAssert.Expect(
            LogType.Error,
            "Test Mushroom: Destroy may not be called from edit mode! Use DestroyImmediate instead.\n" +
            "Destroying an object in edit mode destroys it permanently"
        );

        food.Eat(kobold);

        Assert.AreEqual(1525f, hunger.currentCalories);

        Object.DestroyImmediate(kobold);
        Object.DestroyImmediate(mushroom);
    }


    [Test]
    public void EatingFoodOnlyRestoresTheConsumingKobold()
    {
        GameObject koboldA = new GameObject("Kobold A");
        KoboldHunger hungerA =
            koboldA.AddComponent<KoboldHunger>();
        hungerA.LoseCalories(100f);

        GameObject koboldB = new GameObject("Kobold B");
        KoboldHunger hungerB =
            koboldB.AddComponent<KoboldHunger>();
        hungerB.LoseCalories(200f);

        GameObject mushroom = new GameObject("Test Mushroom");
        TestFood food = mushroom.AddComponent<TestFood>();
        food.SetCalories(25f);

        LogAssert.Expect(
            LogType.Error,
            "Test Mushroom: Destroy may not be called from edit mode! Use DestroyImmediate instead.\n" +
            "Destroying an object in edit mode destroys it permanently"
        );

        food.Eat(koboldA);

        Assert.AreEqual(1525f, hungerA.currentCalories);
        Assert.AreEqual(1400f, hungerB.currentCalories);

        Object.DestroyImmediate(koboldA);
        Object.DestroyImmediate(koboldB);
        Object.DestroyImmediate(mushroom);
    }

    [Test]
    public void LosingNegativeCaloriesDoesNotIncreaseHunger()
    {
        GameObject kobold = new GameObject("Test Kobold");

        KoboldHunger hunger =
            kobold.AddComponent<KoboldHunger>();

        hunger.LoseCalories(-100f);

        Assert.AreEqual(1600f, hunger.currentCalories);

        Object.DestroyImmediate(kobold);
    }

    [Test]
    public void ConsumingNegativeCaloriesDoesNotReduceHunger()
    {
        GameObject kobold = new GameObject("Test Kobold");

        KoboldHunger hunger =
            kobold.AddComponent<KoboldHunger>();

        hunger.ConsumeCalories(-100f);

        Assert.AreEqual(1600f, hunger.currentCalories);

        Object.DestroyImmediate(kobold);
    }

    [Test]
    public void FoodUseFailsWhenConsumerIsNull()
    {
        GameObject mushroom = new GameObject("Test Mushroom");
        TestFood food = mushroom.AddComponent<TestFood>();

        bool result = food.Use(null);

        Assert.IsFalse(result);

        Object.DestroyImmediate(mushroom);
    }

    [Test]
    public void FoodUseFailsWhenConsumerHasNoKoboldHunger()
    {
        GameObject consumer = new GameObject("Invalid Consumer");
        GameObject mushroom = new GameObject("Test Mushroom");
        TestFood food = mushroom.AddComponent<TestFood>();

        bool result = food.Use(consumer);

        Assert.IsFalse(result);

        Object.DestroyImmediate(consumer);
        Object.DestroyImmediate(mushroom);
    }

    [Test]
    public void BrownMushroomRestores500Calories()
    {
        GameObject kobold = new GameObject("Test Kobold");

        KoboldHunger hunger =
            kobold.AddComponent<KoboldHunger>();

        hunger.LoseCalories(600f);

        GameObject mushroom =
            new GameObject("Brown Mushroom");

        BrownMushroom food =
            mushroom.AddComponent<BrownMushroom>();

        LogAssert.Expect(
            LogType.Error,
            "Brown Mushroom: Destroy may not be called from edit mode! Use DestroyImmediate instead.\n" +
            "Destroying an object in edit mode destroys it permanently"
        );

        bool result = food.Use(kobold);

        Assert.IsTrue(result);
        Assert.AreEqual(1500f, hunger.currentCalories);

        Object.DestroyImmediate(kobold);
        Object.DestroyImmediate(mushroom);
    }


}

public class TestFood : Food
{
    public void SetCalories(float value)
    {
        calories = value;
    }
}
