using UnityEngine;
using UnityEngine.UI;

public class AchievementModal : MonoBehaviour
{
    public Text messageText;

    public void Show(string message)
    {
        if (messageText != null)
            messageText.text = message;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
