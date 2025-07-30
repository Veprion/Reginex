using UnityEngine;
using UnityEngine.UI;

public class SpinSpriteButton : MonoBehaviour
{
    void Awake()
    {
        // If this object has a UI Button component, hook up the onClick event
        Button btn = GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.AddListener(() =>
            {
                if (ColorSlotMachine.Instance != null)
                    ColorSlotMachine.Instance.StartSpin();
            });
        }
        else
        {
            // Ensure there's a collider so OnMouseDown works on sprites
            if (GetComponent<Collider2D>() == null)
            {
                var col = gameObject.AddComponent<BoxCollider2D>();
                var sr = GetComponent<SpriteRenderer>();
                if (sr != null && sr.sprite != null)
                    col.size = sr.sprite.bounds.size;
            }
        }
    }

    public void OnMouseDown()
    {
        // Only respond to mouse clicks if no UI Button is present
        if (GetComponent<Button>() == null && ColorSlotMachine.Instance != null)
            ColorSlotMachine.Instance.StartSpin();
    }
}
