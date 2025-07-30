using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlotMachine : MonoBehaviour
{
    [Header("Wheels")]
    public SlotWheel[] wheels;
    
    [Header("Timing")]
    public float firstWheelDelay = 15f;
    public float subsequentWheelDelay = 7f;
    
    [Header("UI")]
    public Button spinButton;
    public Text resultText;
    public Text achievementText;
    
    private bool isSpinning = false;
    private int wheelsStopped = 0;
    private SymbolType[] currentResult = new SymbolType[3];
    
    public static SlotMachine Instance;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        if (spinButton != null)
        {
            spinButton.onClick.AddListener(StartSpin);
        }
        
        UpdateUI();
    }
    
    public void StartSpin()
    {
        if (isSpinning) return;
        
        isSpinning = true;
        wheelsStopped = 0;
        
        // Disable spin button
        if (spinButton != null)
            spinButton.interactable = false;
        
        // Clear result text
        if (resultText != null)
            resultText.text = "Spinning...";
        
        // Start wheels with different delays
        for (int i = 0; i < wheels.Length; i++)
        {
            float delay = (i == 0) ? firstWheelDelay : firstWheelDelay + (i * subsequentWheelDelay);
            wheels[i].StartSpin(i, delay);
        }
    }
    
    public void OnWheelStopped(int wheelIndex, SymbolType symbol)
    {
        currentResult[wheelIndex] = symbol;
        wheelsStopped++;
        
        Debug.Log($"Wheel {wheelIndex + 1} stopped with symbol: {symbol}");
        
        // Check if all wheels have stopped
        if (wheelsStopped >= wheels.Length)
        {
            OnAllWheelsStopped();
        }
    }
    
    void OnAllWheelsStopped()
    {
        isSpinning = false;
        
        // Enable spin button
        if (spinButton != null)
            spinButton.interactable = true;
        
        // Check for achievements
        AchievementManager.Instance.CheckAchievements(currentResult);
        
        // Update wheels to show new available symbols
        foreach (SlotWheel wheel in wheels)
        {
            wheel.RefreshWheel();
        }
        
        // Update UI
        UpdateUI();
        
        // Show result
        ShowResult();
    }
    
    void ShowResult()
    {
        string resultString = $"Result: {currentResult[0]} | {currentResult[1]} | {currentResult[2]}";
        
        // Check for wins
        if (currentResult[0] == currentResult[1] && currentResult[1] == currentResult[2])
        {
            resultString += $"\n🎉 JACKPOT! Three {currentResult[0]}s! 🎉";
        }
        else if (currentResult[0] == currentResult[1] || currentResult[1] == currentResult[2] || currentResult[0] == currentResult[2])
        {
            resultString += "\n✨ Two of a kind! ✨";
        }
        
        if (resultText != null)
            resultText.text = resultString;
        
        Debug.Log(resultString);
    }
    
    void UpdateUI()
    {
        if (achievementText != null)
        {
            string achievementInfo = "Available Symbols: ";
            List<SymbolType> availableSymbols = AchievementManager.Instance.GetAvailableSymbols();
            
            for (int i = 0; i < availableSymbols.Count; i++)
            {
                achievementInfo += availableSymbols[i].ToString();
                if (i < availableSymbols.Count - 1)
                    achievementInfo += ", ";
            }
            
            achievementInfo += "\n\nAchievements:\n";
            
            foreach (Achievement achievement in AchievementManager.Instance.achievements)
            {
                string status = achievement.isUnlocked ? "✅" : "❌";
                achievementInfo += $"{status} {achievement.name}: {achievement.description}\n";
            }
            
            achievementText.text = achievementInfo;
        }
    }
    
    // Method to manually unlock achievement for testing
    [ContextMenu("Test Unlock Achievement")]
    public void TestUnlockAchievement()
    {
        AchievementManager.Instance.UnlockAchievement("First Triple");
        UpdateUI();
        
        // Refresh wheels to show new symbols
        foreach (SlotWheel wheel in wheels)
        {
            wheel.RefreshWheel();
        }
    }
}