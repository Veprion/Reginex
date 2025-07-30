using System.Collections;
using UnityEngine;

public class ColorSlotWheel : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public float spinDuration = 2f;
    public float frameTime = 0.1f;
    public Sprite[] cycleSprites;
    public int wheelIndex;

    private Sprite finalSprite;

    public void StartSpin(Sprite final)
    {
        finalSprite = final;
        StartCoroutine(SpinRoutine());
    }

    IEnumerator SpinRoutine()
    {
        float timer = 0f;
        int index = 0;
        while (timer < spinDuration)
        {
            if (cycleSprites != null && cycleSprites.Length > 0 && spriteRenderer != null)
            {
                spriteRenderer.sprite = cycleSprites[index % cycleSprites.Length];
                index++;
            }
            timer += frameTime;
            yield return new WaitForSeconds(frameTime);
        }
        if (spriteRenderer != null)
            spriteRenderer.sprite = finalSprite;
        ColorSlotMachine.Instance.OnWheelStopped();
    }
}
