using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Util : MonoBehaviour
{
    #region GameObject Utilities

    /// <summary>
    /// Finds a child GameObject by name, recursively.
    /// </summary>
    public static GameObject FindChildByNameRecursively(Transform parent, string childName, bool starter)
    {
        foreach (Transform child in parent)
        {
            if (child.name == childName)
            {
                return child.gameObject;
            }

            GameObject foundInChildren = FindChildByNameRecursively(child, childName, false);
            if (foundInChildren != null)
            {
                return foundInChildren;
            }
        }

        if (starter)
            Debug.Log("Didn't find child with the name: " + childName);
        return null;
    }
    /*
        GameObject child = Util.FindChildByNameRecursively(parentTransform, "ChildName", true);
    */

    /// <summary>
    /// Sets the layer of a GameObject and all its children recursively.
    /// </summary>
    public static void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }
    /*
        Util.SetLayerRecursively(gameObject, LayerMask.NameToLayer("NewLayer"));
    */

    #endregion

    #region Component Utilities

    /// <summary>
    /// Gets a component from a GameObject, or adds it if it doesn't exist.
    /// </summary>
    public static T GetOrAddComponent<T>(GameObject obj) where T : Component
    {
        T component = obj.GetComponent<T>();
        if (component == null)
        {
            component = obj.AddComponent<T>();
        }
        return component;
    }
    /*
        Rigidbody rb = Util.GetOrAddComponent<Rigidbody>(gameObject);
    */

    /// <summary>
    /// Finds a specific component type in children of the given Transform, recursively.
    /// </summary>
    public static T FindComponentInChildren<T>(Transform parent) where T : Component
    {
        foreach (Transform child in parent)
        {
            T component = child.GetComponent<T>();
            if (component != null)
                return component;

            component = FindComponentInChildren<T>(child);
            if (component != null)
                return component;
        }
        return null;
    }
    /*
        MyComponent foundComponent = Util.FindComponentInChildren<MyComponent>(parentTransform);
    */

    #endregion

    #region Coroutine Utilities

    /// <summary>
    /// Waits for a specified duration in seconds.
    /// </summary>
    public static IEnumerator Wait(float duration)
    {
        yield return new WaitForSeconds(duration);
    }
    /*
        StartCoroutine(Util.Wait(3f));
    */

    /// <summary>
    /// Waits for a specified duration in real-time seconds.
    /// </summary>
    public static IEnumerator WaitRealtime(float duration)
    {
        yield return new WaitForSecondsRealtime(duration);
    }
    /*
        StartCoroutine(Util.WaitRealtime(3f));
    */

    /// <summary>
    /// Waits until the specified condition is met, then performs an action.
    /// </summary>
    public static IEnumerator WaitUntil(Func<bool> condition, Action action)
    {
        yield return new WaitUntil(condition);
        action();
    }
    /*
        StartCoroutine(Util.WaitUntil(() => playerHealth <= 0, () => {
        // Player died
    }));
    */

    /// <summary>
    /// Performs an action in the next frame.
    /// </summary>
    public static IEnumerator DoNextFrame(Action action)
    {
        yield return null;
        action();
    }
    /*
        StartCoroutine(Util.DoNextFrame(() => {
        // This will be the code executed in the next frame
    }));
    */

    /// <summary>
    /// Waits for a specific duration, then performs an action.
    /// </summary>
    public static IEnumerator Timeout(Action action, float time)
    {
        yield return new WaitForSeconds(time);
        action();
    }
    /*
        StartCoroutine(Util.Timeout(() => {
        // This will be the code executed in X seconds
    }, X));
    */

    /// <summary>
    /// Waits for a specific duration in real-time, then performs an action.
    /// </summary>
    public static IEnumerator TimeoutRealtime(Action action, float time)
    {
        yield return new WaitForSecondsRealtime(time);
        action();
    }
    /*
        StartCoroutine(Util.TimeoutRealtime(() => {
        // This will be the code executed in X real-time seconds
    }, X));
    */

    /// <summary>
    /// Performs an action repeatedly over a duration, then performs an end action.
    /// </summary>
    public static IEnumerator DoOverTime(Action constantAction, Action endAction, float time)
    {
        float timer = 0f;
        while (timer < time)
        {
            constantAction();
            timer += Time.deltaTime;
            yield return null;
        }
        endAction();
    }
    /*
        StartCoroutine(Util.DoOverTime(() => {
        // This will run each rendered frame until the action ends
    }, () => {
        // This will be the code executed at the end in X seconds
    }, X));
    */

    /// <summary>
    /// Performs an action with eased progress over time, then performs an end action.
    /// </summary>
    public static IEnumerator DoOverTimeWithProgressEasing(Action<float> constantActionWithProgress, Action endAction, float time)
    {
        float timer = 0f;
        while (timer < time)
        {
            float progress = timer / time;
            float easedProgress = EasedProgress(progress);
            constantActionWithProgress(easedProgress);
            timer += Time.deltaTime;
            yield return null;
        }
        endAction();
    }
    /*
        StartCoroutine(Util.DoOverTimeWithProgressEasing((progress) => {
        // This will run each rendered frame with eased progress
    }, () => {
        // This will be the code executed at the end in X seconds
    }, X));
    */

    #endregion

    #region Math Utilities

    /// <summary>
    /// Eases the progress value using a custom cubic function.
    /// </summary>
    private static float EasedProgress(float x)
    {
        return x < 0.5 ? 4 * x * x * x : 1 - Mathf.Pow(-2 * x + 2, 3) / 2;
    }

    /// <summary>
    /// Remaps a value from one range to another.
    /// </summary>
    public static float Remap(float value, float inMin, float inMax, float outMin, float outMax)
    {
        return (value - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
    }
    /*
        float remappedValue = Util.Remap(value, 0f, 1f, 10f, 20f);
    */

    /// <summary>
    /// Checks if two float values are approximately equal, within a given tolerance.
    /// </summary>
    public static bool IsApproximatelyEqual(float a, float b, float tolerance = 0.0001f)
    {
        return Mathf.Abs(a - b) < tolerance;
    }
    /*
        bool isEqual = Util.IsApproximatelyEqual(0.5f, 0.50001f);
    */

    /// <summary>
    /// Clamps an angle between a minimum and maximum value.
    /// </summary>
    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360f) angle += 360f;
        if (angle > 360f) angle -= 360f;
        return Mathf.Clamp(angle, min, max);
    }
    /*
        float clampedAngle = Util.ClampAngle(angle, -45f, 45f);
    */

    #endregion

    #region Random Utilities

    /// <summary>
    /// Gets a random element from a list.
    /// </summary>
    public static T GetRandomElement<T>(IList<T> list)
    {
        return list[UnityEngine.Random.Range(0, list.Count)];
    }
    /*
        int randomElement = Util.GetRandomElement(myList);
    */

    /// <summary>
    /// Shuffles a list randomly.
    /// </summary>
    public static void ShuffleList<T>(IList<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }
    /*
        Util.ShuffleList(myList);
    */

    /// <summary>
    /// Generates a random value, with optional center bias.
    /// </summary>
    public static float Random(float min, float max, bool centerBiased)
    {
        if (!centerBiased)
        {
            return UnityEngine.Random.Range(min, max);
        }
        else
        {
            float mean = (min + max) / 2f;
            float stdDev = (max - min) / 6f;
            float randomValue;
            do
            {
                float u1 = 1.0f - UnityEngine.Random.value;
                float u2 = 1.0f - UnityEngine.Random.value;
                float gaussian = Mathf.Sqrt(-2.0f * Mathf.Log(u1)) * Mathf.Sin(2.0f * Mathf.PI * u2);
                randomValue = mean + stdDev * gaussian;
            } while (randomValue < min || randomValue > max);
            return randomValue;
        }
    }
    /*
        float randomValue = Util.Random(0f, 10f, true);
    */

    #endregion

    #region Scene Management

    /// <summary>
    /// Loads a scene asynchronously, with optional pre and post actions.
    /// </summary>
    public static IEnumerator LoadSceneAsync(string sceneName, Action preAction = null, Action postAction = null)
    {
        preAction?.Invoke();

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        postAction?.Invoke();
    }
    /*
        StartCoroutine(Util.LoadSceneAsync("GameScene", () => {
        // Pre-loading actions
    }, () => {
        // Post-loading actions
    }));
    */

    #endregion
}
