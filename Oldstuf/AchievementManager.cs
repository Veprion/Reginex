using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Achievement
{
    public string name;
    public string description;
    public bool isUnlocked;
    public SymbolType unlockedSymbol;
}

public enum SymbolType
{
    Shoes,
    Jewels,
    Coconuts,
    Stars,
    Hearts,
    Diamonds
}

public class AchievementManager : MonoBehaviour
{
    [Header("Achievements")]
    public List<Achievement> achievements = new List<Achievement>();
    
    [Header("Available Symbols")]
    public List<SymbolType> availableSymbols = new List<SymbolType>();
    
    public static AchievementManager Instance;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAchievements();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void InitializeAchievements()
    {
        // Initialize starting symbols (shoes and jewels are available from start)
        availableSymbols.Add(SymbolType.Shoes);
        availableSymbols.Add(SymbolType.Jewels);
        
        // Set up achievements
        achievements.Add(new Achievement
        {
            name = "First Triple",
            description = "Get 3 of the same symbol",
            isUnlocked = false,
            unlockedSymbol = SymbolType.Coconuts
        });
        
        achievements.Add(new Achievement
        {
            name = "Lucky Stars",
            description = "Get 3 coconuts in a row",
            isUnlocked = false,
            unlockedSymbol = SymbolType.Stars
        });
        
        achievements.Add(new Achievement
        {
            name = "Heart Collector",
            description = "Get 3 stars in a row",
            isUnlocked = false,
            unlockedSymbol = SymbolType.Hearts
        });
        
        achievements.Add(new Achievement
        {
            name = "Diamond Hunter",
            description = "Get 3 hearts in a row",
            isUnlocked = false,
            unlockedSymbol = SymbolType.Diamonds
        });
    }
    
    public void CheckAchievements(SymbolType[] result)
    {
        // Check for three of a kind
        if (result[0] == result[1] && result[1] == result[2])
        {
            UnlockAchievement("First Triple");
            
            // Check specific symbol achievements
            switch (result[0])
            {
                case SymbolType.Coconuts:
                    UnlockAchievement("Lucky Stars");
                    break;
                case SymbolType.Stars:
                    UnlockAchievement("Heart Collector");
                    break;
                case SymbolType.Hearts:
                    UnlockAchievement("Diamond Hunter");
                    break;
            }
        }
    }
    
    public void UnlockAchievement(string achievementName)
    {
        Achievement achievement = achievements.Find(a => a.name == achievementName);
        if (achievement != null && !achievement.isUnlocked)
        {
            achievement.isUnlocked = true;
            
            // Add the unlocked symbol to available symbols
            if (!availableSymbols.Contains(achievement.unlockedSymbol))
            {
                availableSymbols.Add(achievement.unlockedSymbol);
            }
            
            Debug.Log($"Achievement Unlocked: {achievement.name} - {achievement.description}");
            // Here you can add UI notifications, sound effects, etc.
        }
    }
    
    public List<SymbolType> GetAvailableSymbols()
    {
        return new List<SymbolType>(availableSymbols);
    }
}