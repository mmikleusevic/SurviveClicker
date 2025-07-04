using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
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
    private int blacksmiths;
    private int quarries;
    private int ironMines;
    private int goldMines;
    
    [Header("Resources")][Space(10)]
    [SerializeField] private TMP_Text daysText;
    [SerializeField] private TMP_Text populationText;
    [SerializeField] private TMP_Text foodText;
    [SerializeField] private TMP_Text woodText;
    [SerializeField] private TMP_Text ironText;
    
    [Header("Buildings")][Space(10)]
    [SerializeField] private TMP_Text farmsText;
    [SerializeField] private TMP_Text woodcuttersText;
    [SerializeField] private TMP_Text housesText;
    
    [SerializeField] private Image dayImage;
    
    private IEnumerator notificationCoroutine;

    private float timer;
    
    private void Awake()
    {
        if (!Instance) Instance = this;
    }

    private void Update()
    {
        dayImage.fillAmount = timer / dayLengthInSeconds;
        
        timer += Time.deltaTime;
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
    }

    public void StopGame()
    {
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
        
        UpdateText();
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
    
    private void FoodConsumption()
    {
        food -= Population();
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

    private void UpdateText()
    {
        daysText.text = $"{days}";
        populationText.text = $"{Population()} / {GetMaxPopulation()} \n Workers: {workers} \n Unemployed: {unemployed}";
        foodText.text = $"{food}";
        woodText.text = $"{wood}";
        ironText.text = $"{iron}";
        farmsText.text = $"Farms: {farms}";
        woodcuttersText.text = $"Woodcutters: {woodcutters}";
        housesText.text = $"Houses: {houses}";
    }

    public void SetVolume(float volume)
    {
        audioSource.volume = volume;    
    }
}