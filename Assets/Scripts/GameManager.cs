using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private const string HIGHSCORE_TIME = "HighscoreTime";
    private const string HIGHSCORE_NAME = "HighscoreName";
    
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private AudioSource audioSource;
    
    [Header("Resources")][Space(10)]
    [SerializeField] private int startingWorkers;
    [SerializeField] private int startingUnemployed;
    [SerializeField] private int startingWood;
    [SerializeField] private int startingGold;
    [SerializeField] private int startingFood;
    [SerializeField] private int startingStone;
    [SerializeField] private int startingIron;
    [SerializeField] private int startingTools;
    
    private int days;
    private int workers;
    private int unemployed;
    private int wood;
    private int gold;
    private int food;
    private int stone;
    private int iron;
    private int tools;
    
    [Header("Time")][Space(10)]
    [SerializeField] private int dayLengthInSeconds;
    [SerializeField] private int notificationLength;
    
    [Header("Text")]
    [SerializeField] private TMP_Text notificationText;
    
    [Header("Building")][Space(10)]
    [SerializeField] private int startingHouses;
    [SerializeField] private int startingFarms;
    [SerializeField] private int startingWoodcutters;
    [SerializeField] private int startingBlacksmiths;
    [SerializeField] private int startingQuarries;
    [SerializeField] private int startingIronMines;
    [SerializeField] private int startingGoldMines;
    
    private int houses; // 1 house takes 4 people
    private int farms;
    private int woodcutters;
    private int quarries;
    private int blacksmiths;
    private int ironMines;
    private int goldMines;
    
    [Header("Resources")][Space(10)]
    [SerializeField] private TMP_Text daysText;
    [SerializeField] private TMP_Text populationText;
    [SerializeField] private TMP_Text foodText;
    [SerializeField] private TMP_Text woodText;
    [SerializeField] private TMP_Text stoneText;
    [SerializeField] private TMP_Text toolsText;
    [SerializeField] private TMP_Text ironText;
    [SerializeField] private TMP_Text goldText;
    
    [Header("Buildings")][Space(10)]
    [SerializeField] private TMP_Text farmsText;
    [SerializeField] private TMP_Text woodcuttersText;
    [SerializeField] private TMP_Text housesText;
    [SerializeField] private TMP_Text quarriesText;
    [SerializeField] private TMP_Text blacksmithsText;
    [SerializeField] private TMP_Text ironMinesText;
    [SerializeField] private TMP_Text goldMinesText;
    
    [SerializeField] private Image dayImage;
    
    private Pause pause;
    
    private IEnumerator notificationCoroutine;

    private float timer;
    private float currentTimeTimer;
    private float highscoreTimeTimer;
    private bool isGameStopped;
    
    private void Awake()
    {
        if (!Instance) Instance = this;

        pause = GetComponent<Pause>();
    }

    private void Start()
    {
        nameInputField.text = SaveSystem.GetStringValue(HIGHSCORE_NAME);
        highscoreTimeTimer = SaveSystem.GetFloatValue(HIGHSCORE_TIME);
        Debug.Log(nameInputField.text);
        Debug.Log(highscoreTimeTimer);
    }

    private void Update()
    {
        dayImage.fillAmount = timer / dayLengthInSeconds;
        
        timer += Time.deltaTime;

        if (!isGameStopped) currentTimeTimer += Time.deltaTime;
    }

    public void PlayGame()
    {
        SetInitialValues();
        
        UpdateText();

        StartCoroutine(DayPassed());
    }

    private void SetInitialValues()
    {
        workers = startingWorkers;
        unemployed = startingUnemployed;
        wood = startingWood;
        gold = startingGold;
        food = startingFood;
        stone = startingStone;
        iron = startingIron;
        tools = startingTools;
        houses = startingGold;
        farms = startingFarms;
        woodcutters = startingWoodcutters;
        blacksmiths = startingBlacksmiths;
        quarries = startingQuarries;
        ironMines = startingIronMines;
        goldMines = startingGoldMines;
        notificationText.text = string.Empty;
        isGameStopped = false;
    }

    public void StopGame()
    {
        isGameStopped = true;
        
        if (currentTimeTimer > highscoreTimeTimer)
        {
            highscoreTimeTimer = currentTimeTimer;
            SaveSystem.SetFloatValue(HIGHSCORE_TIME, highscoreTimeTimer);
            SaveSystem.SetStringValue(HIGHSCORE_NAME, nameInputField.text);
        }
        StopAllCoroutines();
    }

    // TODO: make this method a class.
    // private void BuildCost(int woodCost, int stoneCost, int workerAssign)
    // {
    //     if (wood < woodCost || stone < stoneCost || unemployed < workerAssign) return;
    //     
    //     wood -= woodCost;
    //     stone -= stoneCost;
    //     unemployed -= workerAssign;
    //     workers += workerAssign;
    // }
    
    public void BuildFarm()
    {
        int workerAmount = 2;
        int woodNeeded = 10;
        
        if (wood >= woodNeeded && CanAssignWorker(workerAmount))
        {
            wood -= woodNeeded;
            farms++;
            WorkerAssign(workerAmount);
        
            MakeANotification($"You built a farm!");
            
            UpdateText();
        }
        else
        {
            MakeANotification($"Not enough resources to build farm, you need {woodNeeded - wood} wood, and {workerAmount} unemployed");
        }
    }

    public void BuildWoodcutters()
    {
        int workerAmount = 1;
        int woodNeeded = 5;
        int ironNeeded = 1;

        if (wood >= woodNeeded && iron >= ironNeeded && CanAssignWorker(workerAmount))
        {
            wood -= woodNeeded;
            iron -= ironNeeded;
            woodcutters++;
            WorkerAssign(workerAmount);
            
            MakeANotification($"You built a woodcutter!");

            UpdateText();
        }
        else
        {
            MakeANotification(
                $"Not enough resources to build woodcutter, you need {woodNeeded - wood} wood, and {workerAmount} unemployed, and {ironNeeded} iron");
        }
    }

    public void BuildHouse()
    {
        int woodNeeded = 2;

        if (wood >= woodNeeded)
        {
            wood -= woodNeeded;
            houses++;
            
            MakeANotification($"You built a house!");

            UpdateText();
        }
        else
        {
            MakeANotification($"Not enough resources to build a house, you need {woodNeeded - wood} wood");
        }
    }

    public void BuildQuarry()
    {
        int woodNeeded = 2;
        int stoneNeeded = 2;
        int workerAmount = 1;

        if (wood >= woodNeeded && stone >= stoneNeeded && CanAssignWorker(workerAmount))
        {
            wood -= woodNeeded;
            stone -= stoneNeeded;
            WorkerAssign(workerAmount);
            quarries++;
            
            MakeANotification($"You built a quarry!");

            UpdateText();
        }
        else
        {
            MakeANotification(
                $"Not enough resources to build a quarry, you need {woodNeeded - wood} wood and {stoneNeeded - stone} stone");
        }
    }
    
    public void BuildBlacksmith()
    {
        int woodNeeded = 2;
        int ironNeeded = 2;
        int workerAmount = 1;

        if (wood >= woodNeeded && iron >= ironNeeded && CanAssignWorker(workerAmount))
        {
            wood -= woodNeeded;
            iron  -= ironNeeded;
            WorkerAssign(workerAmount);
            blacksmiths++;
            
            MakeANotification($"You built a blacksmith!");

            UpdateText();
        }
        else
        {
            MakeANotification(
                $"Not enough resources to build a blacksmith, you need {woodNeeded - wood} wood, and {ironNeeded - iron} iron");
        }
    }
    
    public void BuildIronMine()
    {
        int woodNeeded = 2;
        int stoneNeeded = 2;
        int goldNeeded = 2;
        int workerAmount = 2;

        if (wood >= woodNeeded && stone >= stoneNeeded && gold >= goldNeeded && CanAssignWorker(workerAmount))
        {
            wood -= woodNeeded;
            stone -= stoneNeeded;
            gold -= goldNeeded;
            WorkerAssign(workerAmount);
            ironMines++;
            
            MakeANotification($"You built an iron mine!");

            UpdateText();
        }
        else
        {
            MakeANotification(
                $"Not enough resources to build an iron mine, you need {woodNeeded - wood} wood and {stoneNeeded - stone} stone, and {goldNeeded - gold} gold");
        }
    }
    
    public void BuildGoldMine()
    {
        int woodNeeded = 2;
        int stoneNeeded = 2;
        int ironNeeded = 2;
        int workerAmount = 2;

        if (wood >= woodNeeded && stone >= stoneNeeded && iron >= ironNeeded && CanAssignWorker(workerAmount))
        {
            wood -= woodNeeded;
            stone -= stoneNeeded;
            iron  -= ironNeeded;
            WorkerAssign(workerAmount);
            goldMines++;
            
            MakeANotification($"You built a gold mine!");

            UpdateText();
        }
        else
        {
            MakeANotification(
                $"Not enough resources to build a gold mine, you need {woodNeeded - wood} wood and {stoneNeeded - stone} stone, and {ironNeeded - iron} iron");
        }
    }
    
    private void MakeANotification(string text)
    {
        if (notificationCoroutine != null)
        {
            StopCoroutine(notificationCoroutine);
        }
        
        notificationCoroutine = NotificationText(text);
        StartCoroutine(notificationCoroutine);
    }

    private IEnumerator NotificationText(string text)
    {
        notificationText.gameObject.SetActive(true);
        
        notificationText.text = text;
        yield return new WaitForSeconds(5);
        notificationText.text = string.Empty;
        
        notificationText.gameObject.SetActive(false);
    }

    private IEnumerator DayPassed()
    {
        timer = 0;
        
        yield return new WaitForSeconds(dayLengthInSeconds);
        
        CountDays();
        FoodGathering();
        FoodProduction();
        FoodConsumption();
        WoodGathering();
        WoodProduction();
        StoneProduction();
        ToolsProduction();
        IronProduction();
        GoldProduction();
        IncreasePopulation();
        
        UpdateText();
        
        yield return DayPassed();
    }
    
    private void WorkerAssign(int amount)
    {
        unemployed -= amount;
        workers += amount;
    }

    private bool CanAssignWorker(int amount)
    {
        return unemployed >= amount;
    }

    private void CountDays()
    {
        days++;
    }

    private void IncreasePopulation()
    {
        int maxPopulation = GetMaxPopulation();
        int population = Population();
        
        if (days % 2 == 0 && population < maxPopulation)
        {
            unemployed = Mathf.Min(unemployed + houses, maxPopulation);
        }
    }

    private int Population()
    {
        return workers + unemployed;
    }

    // number of max houses * 4
    private int GetMaxPopulation()
    {
        return houses * 4;
    }
    
    // Lose condition
    private void FoodConsumption()
    {
        int population = Population();
        food -= population;

        if (food <= population)
        {
            pause.StopGame("You lost the game. Everyone starved to death!");
            StopGame();
        }
    }

    private void FoodGathering()
    {
        food += unemployed / 2;
    }
    
    private void WoodGathering()
    {
        wood += unemployed / 2;
    }

    private void FoodProduction()
    {
        food += farms * 4;
    }

    private void WoodProduction()
    {
        wood += woodcutters * 2;
    }
    
    private void IronProduction()
    {
        iron += ironMines * 2;
    }

    private void GoldProduction()
    {
        gold += goldMines * 2;

        if (gold >= 100)
        {
            StopGame();
            pause.StopGame("You won the game. You earned 100 gold!");
        }
    }
    
    private void StoneProduction()
    {
        stone += quarries * 5;
    }
    
    private void ToolsProduction()
    {
        tools += blacksmiths * 5;
    }
    
    private void UpdateText()
    {
        daysText.text = $"{days}";
        populationText.text = $"{Population()} / {GetMaxPopulation()} \n Workers: {workers} \n Unemployed: {unemployed}";
        foodText.text = $"{food}";
        woodText.text = $"{wood}";
        ironText.text = $"{iron}";
        stoneText.text = $"{stone}";
        toolsText.text = $"{tools}";
        ironText.text = $"{iron}";
        goldText.text = $"{gold}";
        farmsText.text = $"Farms: {farms}";
        woodcuttersText.text = $"Woodcutters: {woodcutters}";
        housesText.text = $"Houses: {houses}";
        quarriesText.text = $"Quarries: {quarries}";
        blacksmithsText.text = $"Blacksmiths: {blacksmiths}";
        ironMinesText.text = $"Iron Mines: {ironMines}";
        goldMinesText.text = $"Gold Mines: {goldMines}";
    }

    public void SetVolume(float volume)
    {
        audioSource.volume = volume;    
    }
}