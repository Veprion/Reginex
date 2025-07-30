using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotWheel : MonoBehaviour
{
    [Header("Wheel Settings")]
    public float spinSpeed = 1000f;
    public float slowdownTime = 2f;
    public int symbolsOnWheel = 10;
    public float symbolSpacing = 100f;
    
    [Header("Symbol Prefabs")]
    public GameObject symbolPrefab;
    public Sprite[] symbolSprites;
    
    private List<GameObject> symbols = new List<GameObject>();
    private bool isSpinning = false;
    private float currentSpeed = 0f;
    private SymbolType finalSymbol;
    private int wheelIndex;
    
    void Start()
    {
        CreateWheel();
    }
    
    void CreateWheel()
    {
        // Clear existing symbols
        foreach (GameObject symbol in symbols)
        {
            if (symbol != null)
                DestroyImmediate(symbol);
        }
        symbols.Clear();
        
        // Get available symbols from achievement manager
        List<SymbolType> availableSymbols = AchievementManager.Instance.GetAvailableSymbols();
        
        // Create symbols on the wheel
        for (int i = 0; i < symbolsOnWheel; i++)
        {
            GameObject newSymbol = Instantiate(symbolPrefab, transform);
            newSymbol.transform.localPosition = new Vector3(0, i * symbolSpacing, 0);
            
            // Assign random symbol from available symbols
            SymbolType randomSymbol = availableSymbols[Random.Range(0, availableSymbols.Count)];
            SpriteRenderer symbolRenderer = newSymbol.GetComponent<SpriteRenderer>();
            symbolRenderer.sprite = GetSymbolSprite(randomSymbol);
            
            // Store the symbol type
            SymbolComponent symbolComp = newSymbol.GetComponent<SymbolComponent>();
            if (symbolComp == null)
                symbolComp = newSymbol.AddComponent<SymbolComponent>();
            symbolComp.symbolType = randomSymbol;
            
            symbols.Add(newSymbol);
        }
    }
    
    Sprite GetSymbolSprite(SymbolType symbolType)
    {
        // Map symbol types to sprites
        // You'll need to assign these in the inspector or create a mapping
        int index = (int)symbolType;
        if (index < symbolSprites.Length)
            return symbolSprites[index];
        return symbolSprites[0]; // Default sprite
    }
    
    public void StartSpin(int wheelIndex, float stopDelay)
    {
        if (isSpinning) return;
        
        this.wheelIndex = wheelIndex;
        StartCoroutine(SpinRoutine(stopDelay));
    }
    
    IEnumerator SpinRoutine(float stopDelay)
    {
        isSpinning = true;
        currentSpeed = spinSpeed;
        
        // Spin at full speed
        yield return new WaitForSeconds(stopDelay);
        
        // Choose final symbol
        List<SymbolType> availableSymbols = AchievementManager.Instance.GetAvailableSymbols();
        finalSymbol = availableSymbols[Random.Range(0, availableSymbols.Count)];
        
        // Slow down gradually
        float slowdownTimer = 0f;
        while (slowdownTimer < slowdownTime)
        {
            slowdownTimer += Time.deltaTime;
            float progress = slowdownTimer / slowdownTime;
            currentSpeed = Mathf.Lerp(spinSpeed, 0f, progress);
            yield return null;
        }
        
        // Snap to final position
        SnapToFinalSymbol();
        
        isSpinning = false;
        currentSpeed = 0f;
        
        // Notify slot machine that this wheel has stopped
        SlotMachine.Instance.OnWheelStopped(wheelIndex, finalSymbol);
    }
    
    void Update()
    {
        if (isSpinning)
        {
            // Move all symbols down
            foreach (GameObject symbol in symbols)
            {
                symbol.transform.localPosition += Vector3.down * currentSpeed * Time.deltaTime;
                
                // Wrap around if symbol goes too far down
                if (symbol.transform.localPosition.y < -symbolSpacing)
                {
                    symbol.transform.localPosition += Vector3.up * (symbolsOnWheel * symbolSpacing);
                }
            }
        }
    }
    
    void SnapToFinalSymbol()
    {
        // Find the symbol closest to the center and snap it exactly to center
        GameObject closestSymbol = null;
        float closestDistance = float.MaxValue;
        
        foreach (GameObject symbol in symbols)
        {
            float distance = Mathf.Abs(symbol.transform.localPosition.y);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestSymbol = symbol;
            }
        }
        
        if (closestSymbol != null)
        {
            // Adjust all symbols so the closest one is at center
            float offset = -closestSymbol.transform.localPosition.y;
            foreach (GameObject symbol in symbols)
            {
                symbol.transform.localPosition += Vector3.up * offset;
            }
            
            // Update the symbol to match our desired final symbol
            SymbolComponent symbolComp = closestSymbol.GetComponent<SymbolComponent>();
            if (symbolComp != null)
            {
                symbolComp.symbolType = finalSymbol;
                closestSymbol.GetComponent<SpriteRenderer>().sprite = GetSymbolSprite(finalSymbol);
            }
        }
    }
    
    public SymbolType GetCurrentSymbol()
    {
        // Return the symbol at the center position
        GameObject centerSymbol = GetCenterSymbol();
        if (centerSymbol != null)
        {
            SymbolComponent symbolComp = centerSymbol.GetComponent<SymbolComponent>();
            if (symbolComp != null)
                return symbolComp.symbolType;
        }
        return SymbolType.Shoes; // Default
    }
    
    GameObject GetCenterSymbol()
    {
        GameObject closestSymbol = null;
        float closestDistance = float.MaxValue;
        
        foreach (GameObject symbol in symbols)
        {
            float distance = Mathf.Abs(symbol.transform.localPosition.y);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestSymbol = symbol;
            }
        }
        
        return closestSymbol;
    }
    
    public void RefreshWheel()
    {
        // Recreate wheel with new available symbols
        CreateWheel();
    }
}

// Component to store symbol type on GameObjects
public class SymbolComponent : MonoBehaviour
{
    public SymbolType symbolType;
}