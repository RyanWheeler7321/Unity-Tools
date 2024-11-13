using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class Util : MonoBehaviour
{

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


    public static IEnumerator Wait(float duration)
    {
        yield return new WaitForSeconds(duration);
    }

    public static IEnumerator WaitRealtime(float duration)
    {
        yield return new WaitForSecondsRealtime(duration);
    }

    public static IEnumerator WaitUntil(Func<bool> condition, Action action)
    {
        yield return new WaitUntil(condition);
        action();
    }


    public static IEnumerator DoNextFrame(Action action)
    {
        yield return null;
        action();
    }

    public static IEnumerator Timeout(Action action, float time)
    {
        yield return new WaitForSeconds(time);
        action();
    }

    public static IEnumerator TimeoutRealtime(Action action, float time)
    {
        yield return new WaitForSecondsRealtime(time);
        action();
    }

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

    public static IEnumerator DoOverTimeWithProgressEasing(Action<float> constantActionWithProgress, Action endAction, float time)
    {
        float timer = 0f;
        while (timer < time)
        {
            float progress = timer / time;
            float easedProgress = EasedProgress(progress); // Apply easing function to progress
            constantActionWithProgress(easedProgress);
            timer += Time.deltaTime;
            yield return null;
        }
        endAction();
    }

    private static float EasedProgress(float x)
    {
        return x < 0.5 ? 4 * x * x * x : 1 - Mathf.Pow(-2 * x + 2, 3) / 2;
    }

    public static Bounds GetBoundsOfChildren(GameObject obj)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
            return new Bounds(obj.transform.position, Vector3.zero);

        Bounds bounds = renderers[0].bounds;
        foreach (Renderer renderer in renderers)
        {
            bounds.Encapsulate(renderer.bounds);
        }
        return bounds;
    }


    public static T GetRandomElement<T>(IList<T> list)
    {
        return list[UnityEngine.Random.Range(0, list.Count)];
    }

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

    public static List<Transform> GetChildren(Transform parent)
    {
        List<Transform> children = new List<Transform>();
        foreach (Transform child in parent)
        {
            children.Add(child);
        }
        return children;
    }

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

    //MyComponent foundComponent = Util.FindComponentInChildren<MyComponent>(parentTransform);


    public static T GetOrAddComponent<T>(GameObject obj) where T : Component
    {
        T component = obj.GetComponent<T>();
        if (component == null)
        {
            component = obj.AddComponent<T>();
        }
        return component;
    }

    //Rigidbody rb = Util.GetOrAddComponent<Rigidbody>(gameObject);

    public static void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }

    //Util.SetLayerRecursively(gameObject, LayerMask.NameToLayer("NewLayer"));


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

    public static float Remap(float value, float inMin, float inMax, float outMin, float outMax)
    {
        return (value - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
    }

    public static bool IsApproximatelyEqual(float a, float b, float tolerance = 0.0001f)
    {
        return Mathf.Abs(a - b) < tolerance;
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360f) angle += 360f;
        if (angle > 360f) angle -= 360f;
        return Mathf.Clamp(angle, min, max);
    }



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
        EXAMPLE CALLS

        StartCoroutine(Util.WaitUntil(() => playerHealth <= 0, () => {
            // player died
        }));



        StartCoroutine(Util.Timeout(() => 
        {

         //This will be the code executed in X seconds
         
        }, X));
        
        
        StartCoroutine(Util.DoNextFrame(() => 
        {

         //This will be the code executed in X seconds
         
        }));


        StartCoroutine(Util.DoOverTime(() =>
        {

            //This will run each rendered frame until the action ends

        }, () =>
        {

            //This will be the code executed at the end in X seconds

        }, X));

    */

}