using UnityEngine;

/// <summary>
/// Utility component that assigns a generated symbol prefab to all SlotWheel
/// instances. The prefab contains all shoe sprites stacked vertically so the
/// wheels can display them while spinning.
/// </summary>
public class SlotWheelPrefabGenerator : MonoBehaviour
{
    public SlotWheel[] wheels;
    public float spacing = 1f;

    void Awake()
    {
        if (wheels == null || wheels.Length == 0)
            return;

        GameObject prefab = CreatePrefab();
        foreach (var wheel in wheels)
        {
            if (wheel != null && wheel.symbolPrefab == null)
                wheel.symbolPrefab = prefab;
        }
    }

    GameObject CreatePrefab()
    {
        GameObject root = new GameObject("AutoSymbolPrefab");
        string[] names = { "Shoes_B", "Shoes_P", "Shoes_R", "Shoes_W" };
        float offset = 0f;
        foreach (string n in names)
        {
            Sprite sprite = Resources.Load<Sprite>(n);
            GameObject child = new GameObject(n);
            child.transform.SetParent(root.transform, false);
            child.transform.localPosition = new Vector3(0f, offset, 0f);
            offset -= spacing;
            var sr = child.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;
        }
        root.SetActive(false);
        return root;
    }
}
