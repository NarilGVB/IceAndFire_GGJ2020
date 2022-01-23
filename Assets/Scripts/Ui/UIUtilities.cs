using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public static class UIUtilities
{
    public static List<RectTransform> GetAntecesorsRectTranforms (Transform transform)
    {
        List<RectTransform> antecesors = new List<RectTransform>();
        antecesors.Add(transform.GetComponent<RectTransform>());
        for (int i = 0; i < 3; i++)
        {
            if (antecesors[i].parent == null) { break; }
            antecesors.Add(antecesors[i].transform.parent.GetComponent<RectTransform>());
        }
        return antecesors;
    }

    public static void UpdateRectTransforms (List<RectTransform> rectTransforms)
    {
        foreach (RectTransform rect in rectTransforms)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
        }
    }

    public static void UpdateAllRectTransforms(this Transform parent)
    {
        RectTransform[] rectTransforms = parent.GetComponentsInChildren<RectTransform>();
        foreach (RectTransform rect in rectTransforms)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
        }
    }
    
    public static void SetActive(this CanvasGroup target, bool activate)
    {
        if (activate)
            target.Activate();
        else target.Deactivate();
    }

    /// <summary>
    /// Sets alpha to 1, and interactable and blockRaycast to true
    /// </summary>
    /// <param name="target"></param>
    public static void Activate(this CanvasGroup target, bool value = true)
    {
        target.alpha = value? 1 : 0;
        target.interactable = value;
        target.blocksRaycasts = value;
    }

    /// <summary>
    /// Sets alpha to 0, and interactable and blockRaycast to false
    /// </summary>
    /// <param name="target"></param>
    public static void Deactivate(this CanvasGroup target)
    {
        target.alpha = 0;
        target.interactable = false;
        target.blocksRaycasts = false;
    }

    /// <summary>
    /// Sets interactable and blockRaycast to false
    /// </summary>
    /// <param name="target"></param>
    public static void Block (this CanvasGroup target)
    {
        target.interactable = false;
        target.blocksRaycasts = false;
    }

    /// <summary>
    /// Sets interactable and blockRaycast to true
    /// </summary>
    /// <param name="target"></param>
    public static void Unblock (this CanvasGroup target)
    {
        target.interactable = true;
        target.blocksRaycasts = true;
    }

    /// <summary>
    /// Sets alpha to 1
    /// </summary>
    /// <param name="target"></param>
    public static void Show(this CanvasGroup target, bool value = true)
    {
        target.alpha = value ? 1 : 0;
    }

    /// <summary>
    /// Sets alpha to 1
    /// </summary>
    /// <param name="target"></param>
    public static bool IsVisible(this CanvasGroup target)
    {
        return target.alpha != 0;
    }
}
