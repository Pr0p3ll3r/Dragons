using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Globalization;
using UnityEngine.UI;
using System.IO;
using System;
using System.Net;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController Instance;

    public static string profileID = "Profile0";

    private void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = Screen.currentResolution.refreshRate;
        Instance = this;
        Database.database = database;
        foreach (DragonInfo d in Database.database.enemyDragons)
        {
            d.Initialize();
            d.LoadEquipment();
        }
        foreach (Expedition e in Database.database.expeditions)
        {
            e.Initialize();
        }
    }

    [SerializeField] private Button[] mainButtons;
    [SerializeField] private Transform canvas;
    [SerializeField] private GameObject newGameButtonClose;
    [SerializeField] private GameObject quitMenu;

    [SerializeField] private GameObject eggPrefab;
    [SerializeField] private GameObject dragonPrefab;
    [SerializeField] private GameObject dragonListPrefab;
    [SerializeField] private GameObject newDragonListPrefab;
    [SerializeField] private Transform dragonList;

    [SerializeField] private GameObject expeditionListPrefab;
    [SerializeField] private GameObject expeditionPanel;
    [SerializeField] private GameObject expeditionInfoPrefab;
    [SerializeField] private Transform expeditionList;
    [SerializeField] private GameObject expeditionLootPrefab;

    [SerializeField] private GameObject inventoryPanel;

    [SerializeField] private GameObject eggsPanel;
    [SerializeField] private GameObject blueprintListPrefab;
    [SerializeField] private GameObject warningBlueprintsText;
    [SerializeField] private GameObject basicDragonsPanel;
    [SerializeField] private GameObject newDragonsPanel;
    [SerializeField] private Transform listOfBlueprints;

    [SerializeField] private GameObject arenaPanel;
    [SerializeField] private Transform arenaList;

    [SerializeField] private GameObject releasePanel;
    [SerializeField] private Button releaseButton;

    [SerializeField] private TextMeshProUGUI warningText;
    [SerializeField] private DatabaseSO database;

    public DragonInfo[] myDragons = new DragonInfo[6];
    public Dragon currentDragon;

    private Egg currentEgg;
    private GameObject currentExpeditionInfo;
    private int nextIndex = 0;
    private Inventory inventory;
    private List<Coroutine> coroutines = new List<Coroutine>();
    private GameObject newDragonUI;

    private void Start()
    {
        if (Data.NewGame(profileID))
        {
            BasicsDragonsPanel();
            newGameButtonClose.gameObject.SetActive(false);
            mainButtons[0].transform.parent.gameObject.SetActive(false);
        }
        else
        {
            Data.Load(profileID);
            mainButtons[0].transform.parent.gameObject.SetActive(true);
            eggsPanel.SetActive(false);
        }
        CreateDragonList();
        LoadCoroutines();
        releasePanel.SetActive(false);
        quitMenu.SetActive(false);
        arenaPanel.SetActive(false);
        inventoryPanel.SetActive(false);
        expeditionPanel.SetActive(false);
        CreateExpeditionList();
        mainButtons[0].onClick.AddListener(delegate { ShowDragonList(); });
        mainButtons[1].onClick.AddListener(delegate { ShowInventory(); });
        mainButtons[2].onClick.AddListener(delegate { ShowQuitMenu(); });
        inventory = Inventory.Instance;
    }

    public void AddDragon(DragonInfo basicDragon)
    {
        DragonInfo newDragon = basicDragon.GetCopy();
        newDragon.Initialize();
        newDragon.shown = true;
        newDragon.index = nextIndex;
        myDragons[nextIndex] = newDragon;
        GetNextIndex();
        HideDragon();
        mainButtons[0].transform.parent.gameObject.SetActive(true);
        Egg newEgg = Instantiate(eggPrefab).GetComponent<Egg>();
        newEgg.newDragon = newDragon;
        currentEgg = newEgg;
        newDragon.indexUI = dragonList.childCount - 1;
        GameObject dragonUI = Instantiate(dragonListPrefab, dragonList);
        dragonUI.GetComponent<Button>().onClick.AddListener(delegate { ShowDragon(newDragon); });
        dragonUI.transform.Find("ButtonRemove").GetComponent<Button>().onClick.AddListener(delegate { ShowReleaseDragonPanel(newDragon); });
    }

    private void ShowDragonList()
    {
        RefreshDragonList();
        dragonList.gameObject.SetActive(!dragonList.gameObject.activeSelf);
    }

    private void CreateDragonList()
    {
        for (int i = 0; i < myDragons.Length; i++)
        {
            if (myDragons[i] == null)
                continue;

            DragonInfo dragon = myDragons[i];
            GameObject dragonUI = Instantiate(dragonListPrefab, dragonList);
            dragonUI.GetComponent<DragonUI>().SetUp(myDragons[i]);
            dragonUI.GetComponent<Button>().onClick.AddListener(delegate { ShowDragon(dragon); });
            dragonUI.transform.Find("ButtonRemove").GetComponent<Button>().onClick.AddListener(delegate { ShowReleaseDragonPanel(dragon); });
        }

        GetNextIndex();

        if (dragonList.childCount < myDragons.Length)
        {
            newDragonUI = Instantiate(newDragonListPrefab, dragonList);
            newDragonUI.GetComponent<Button>().onClick.AddListener(delegate { EggPanel(); });
        }
    }

    public void RefreshDragonList()
    {
        GetNextIndex();

        for (int i = 0; i < myDragons.Length; i++)
        {
            if (myDragons[i] == null)
                continue;

            dragonList.GetChild(myDragons[i].indexUI).GetComponent<DragonUI>().SetUp(myDragons[i]);
        }

        if (dragonList.childCount < myDragons.Length && newDragonUI == null)
        {
            newDragonUI = Instantiate(newDragonListPrefab, dragonList);
            newDragonUI.GetComponent<Button>().onClick.AddListener(delegate { EggPanel(); });
        }
    }

    private void GetNextIndex()
    {
        nextIndex = myDragons.Length;
        for (int i = 0; i < myDragons.Length; i++)
        {
            if (myDragons[i] == null)
            {
                nextIndex = i;
                break;
            }
        }
    }

    private void ShowDragon(DragonInfo info)
    {
        HideDragon();

        if (info.isEgg)
        {
            GameObject dragonGO = Instantiate(eggPrefab);
            Egg egg = dragonGO.GetComponent<Egg>();
            currentEgg = egg;
            egg.newDragon = info;
            egg.newDragon.shown = true;
        }
        else
        {
            if(info.loot)
            {
                ShowLoot(info);
            }
            else
            {
                GameObject dragonGO = Instantiate(dragonPrefab);
                Dragon dragon = dragonGO.GetComponent<Dragon>();
                currentDragon = dragon;
                dragon.dragon = info;
                dragon.dragon.shown = true;
            }
        }

        dragonList.gameObject.SetActive(false);
    }

    public void ShowReleaseDragonPanel(DragonInfo info)
    {
        HideDragon();
        releasePanel.SetActive(true);
        dragonList.gameObject.SetActive(false);
        releaseButton.onClick.RemoveAllListeners();
        releaseButton.onClick.AddListener(delegate { ReleaseDragon(info); });
    }

    private void ReleaseDragon(DragonInfo info)
    {
        int releasedDragonIndexUI = info.indexUI;
        int releasedDragonIndex = info.index;
        myDragons[releasedDragonIndex] = null;

        //unparent childs of dragonList and destroy them cause destroy will happen next frame which will be too late for a loop
        if(dragonList.childCount < myDragons.Length)
        {
            Transform childNewDragon = dragonList.GetChild(dragonList.childCount - 1);
            childNewDragon.SetParent(null);
            Destroy(childNewDragon.gameObject);
        }
        Transform childReleasedDragon = dragonList.GetChild(releasedDragonIndexUI);
        childReleasedDragon.SetParent(null);
        Destroy(childReleasedDragon.gameObject);

        //set indexUI for all dragons which have higher indexUI than released dragon
        for (int i = 0; i < myDragons.Length; i++)
        {
            if (myDragons[i] != null && myDragons[i].indexUI > releasedDragonIndexUI)
            {                
                myDragons[i].indexUI--;
            }
        }
        releasePanel.SetActive(false);
        RefreshDragonList();
    }

    public void ShowExpeditionPanel(DragonInfo info)
    {
        RefreshExpeditionList(info);
        quitMenu.SetActive(false);
        inventoryPanel.SetActive(false);
        expeditionList.gameObject.SetActive(true);
        expeditionPanel.SetActive(!expeditionPanel.activeSelf);
        if (currentExpeditionInfo != null)
            Destroy(currentExpeditionInfo);
    }

    private void BackToExpeditionList(DragonInfo info)
    {
        RefreshExpeditionList(info);
        expeditionList.gameObject.SetActive(true);
        if (currentExpeditionInfo != null)
            Destroy(currentExpeditionInfo);
    }

    private void RefreshExpeditionList(DragonInfo info)
    {
        for (int i = 0; i < expeditionList.childCount; i++)
        {
            expeditionList.transform.GetChild(i).GetComponent<Button>().onClick.RemoveAllListeners();
            Expedition e = Database.database.expeditions[i];
            float percentage = (float)e.exp / (e.level * 2);
            expeditionList.transform.GetChild(i).Find("ExpBar").GetComponent<Slider>().value = percentage;
            expeditionList.transform.GetChild(i).Find("ExpBar/Text").GetComponent<TextMeshProUGUI>().text = $"Lvl: {e.level} ({e.exp}/{e.level * 2})";
            int index = i;
            expeditionList.transform.GetChild(i).GetComponent<Button>().onClick.AddListener(delegate { ShowExpeditionInfo(e, info); });
        }
    }

    private void ShowExpeditionInfo(Expedition expedition, DragonInfo info)
    {
        expeditionList.gameObject.SetActive(false);
        currentExpeditionInfo = Instantiate(expeditionInfoPrefab, expeditionList.parent);
        int level = expedition.level-1;
        currentExpeditionInfo.transform.GetComponent<ExpeditionInfo>().SetUp(expedition);
        currentExpeditionInfo.transform.Find("ButtonStart").GetComponent<Button>().onClick.AddListener(delegate { StartExpedition(expedition.currentData.expeditionTime, expedition, info); });
        currentExpeditionInfo.transform.Find("ButtonCancel").GetComponent<Button>().onClick.AddListener(delegate { BackToExpeditionList(info); });
    }

    private void CreateExpeditionList()
    {
        for (int i = 0; i < expeditionList.childCount; i++)
        {
            Destroy(expeditionList.GetChild(i).gameObject);
        }

        for (int i = 0; i < Database.database.expeditions.Length; i++)
        {
            GameObject expedition = Instantiate(expeditionListPrefab, expeditionList);
            Expedition e = Database.database.expeditions[i];
            expedition.transform.Find("Background/Name").GetComponent<TextMeshProUGUI>().text = e.placeName;
            expedition.transform.Find("Thumbnail").GetComponent<Image>().sprite = e.thumbnail;
            float percentage = (float)e.exp / (e.level * 2);
            expedition.transform.Find("ExpBar").GetComponent<Slider>().value = percentage;
            expedition.transform.Find("ExpBar/Text").GetComponent<TextMeshProUGUI>().text = $"Lvl: {e.level}";
        }
    }

    private void StartExpedition(float time, Expedition e, DragonInfo info)
    {
        dragonList.GetChild(info.indexUI).GetComponent<DragonUI>().RefreshTimerExpedition(time);
        info.onExpedition = true;
        info.currentExpedition = e.ID;
        int currentCoroutine = coroutines.Count;
        coroutines.Add(StartCoroutine(Expedition(time, currentCoroutine, info.index, e)));
        expeditionPanel.SetActive(false);
        RefreshDragonList();
        info.shown = false;
        if (currentDragon != null) 
            Destroy(currentDragon.gameObject);
    }

    private IEnumerator Expedition(float time, int indexCoro, int indexDragon, Expedition e)
    {
        myDragons[indexDragon].remainingExpeditionTime = time;
        dragonList.GetChild(myDragons[indexDragon].indexUI).GetComponent<DragonUI>().RefreshTimerExpedition(time);

        yield return new WaitForSeconds(1f);

        time -= 1;

        if (time <= 0)
        {
            dragonList.GetChild(myDragons[indexDragon].indexUI).GetComponent<DragonUI>().RefreshTimerExpedition(time);
            coroutines[indexCoro] = null;
            myDragons[indexDragon].onExpedition = false;
            myDragons[indexDragon].remainingExpeditionTime = 0;
            myDragons[indexDragon].loot = true;
            RefreshDragonList();
        }
        else
        {
            dragonList.GetChild(myDragons[indexDragon].indexUI).GetComponent<DragonUI>().RefreshTimerExpedition(time);
            coroutines[indexCoro] = StartCoroutine(Expedition(time, indexCoro, indexDragon, e));
        }
    }

    private void ShowLoot(DragonInfo info)
    {
        Expedition expedition = Database.database.expeditions[info.currentExpedition];
        int neededSlots = 0;
        for (int i = 0; i < expedition.currentData.loot.Length; i++)
        {
            neededSlots++;
        }
        if(!inventory.EnoughSlots(neededSlots))
        {
            ShowText("Not enough space in inventory", new Color32(255, 65, 52, 255));
            return;
        }
        List<Item> items = LootTable.Drop(expedition.currentData.loot);
        List<Resource> resources = LootTable.Drop(expedition.currentData.resources);
        foreach(Item item in items)
        {
            Inventory.Instance.AddItem(item, 1);
        }
        foreach (Resource resource in resources)
        {
            Inventory.Instance.AddResource(resource.name, resource.amount);
        }
        info.loot = false;
        expedition.AddExp();
        GameObject loot = Instantiate(expeditionLootPrefab, canvas);
        loot.GetComponent<ExpeditionInfo>().SetUp(expedition, items, resources);
        RefreshDragonList();
    }

    public void StartEating(float time, DragonInfo info)
    {
        int currentCoroutine = coroutines.Count;
        dragonList.GetChild(info.indexUI).GetComponent<DragonUI>().RefreshTimer("Can eat in", time);
        coroutines.Add(StartCoroutine(Eating(time, currentCoroutine, info.index)));
        RefreshDragonList();
    }

    private IEnumerator Eating(float time, int indexCoro, int indexDragon)
    {
        myDragons[indexDragon].remainingEatingTime = time;
        dragonList.GetChild(myDragons[indexDragon].indexUI).GetComponent<DragonUI>().RefreshTimer("Can eat in", time);

        yield return new WaitForSeconds(1f);

        time -= 1;

        if (time <= 0)
        {
            dragonList.GetChild(myDragons[indexDragon].indexUI).GetComponent<DragonUI>().RefreshTimer("Can eat in", time);
            coroutines[indexCoro] = null;
            myDragons[indexDragon].canEat = true;
            myDragons[indexDragon].remainingEatingTime = time;
            if (currentDragon != null)
                currentDragon.CheckEating();
            RefreshDragonList();
        }
        else
        {
            dragonList.GetChild(myDragons[indexDragon].indexUI).GetComponent<DragonUI>().RefreshTimer("Can eat in", time);
            coroutines[indexCoro] = StartCoroutine(Eating(time, indexCoro, indexDragon));
            myDragons[indexDragon].remainingEatingTime = time;
        }
    }

    private void ShowInventory()
    {
        inventoryPanel.SetActive(!inventoryPanel.activeSelf);
    }

    private void ShowQuitMenu()
    {
        quitMenu.SetActive(!quitMenu.activeSelf);
    }

    public void ShowArena()
    {
        RefreshArenaList();
        quitMenu.SetActive(false);
        inventoryPanel.SetActive(false);
        arenaPanel.SetActive(true);
    }

    private void RefreshArenaList()
    {
        DragonInfo info = currentDragon.GetComponent<Dragon>().dragon;

        for (int i = 0; i < arenaList.childCount; i++)
        {
            arenaList.GetChild(i).GetComponent<ArenaDragon>().SetUp(i);
            int index = i;
            arenaList.GetChild(i).GetComponent<Button>().onClick.AddListener(delegate { StartBattle(info, Database.database.enemyDragons[index]); });
        }
    }

    private void StartBattle(DragonInfo myDragon, DragonInfo enemyDragon)
    {
        GetComponent<BattleSystem>().SetupBattle(myDragon, enemyDragon);
        arenaPanel.SetActive(false);
        myDragon.shown = false;
    }

    public void StartHatching(float time, DragonInfo newDragon)
    {
        int currentCoroutine = coroutines.Count;
        coroutines.Add(StartCoroutine(Hatching(time, currentCoroutine, newDragon.index)));
        dragonList.GetChild(newDragon.indexUI).GetComponent<DragonUI>().RefreshTimer("Hatching", time);
        RefreshDragonList();
    }

    private IEnumerator Hatching(float time, int indexCoro, int indexDragon)
    {
        myDragons[indexDragon].remainingHatchingTime = time;
        dragonList.GetChild(myDragons[indexDragon].indexUI).GetComponent<DragonUI>().RefreshTimer("Hatching", time);
            
        yield return new WaitForSeconds(1f);

        time -= 1;

        if (time <= 0)
        {
            dragonList.GetChild(myDragons[indexDragon].indexUI).GetComponent<DragonUI>().RefreshTimer("Hatching", time);
            coroutines[indexCoro] = null;
            myDragons[indexDragon].hatching = false;
            myDragons[indexDragon].toName = true;
            myDragons[indexDragon].remainingHatchingTime = 0;
            if (currentEgg != null)
                currentEgg.EndHatching();
            RefreshDragonList();
        }
        else
        {
            dragonList.GetChild(myDragons[indexDragon].indexUI).GetComponent<DragonUI>().RefreshTimer("Hatching", time);
            myDragons[indexDragon].remainingHatchingTime = time;
            coroutines[indexCoro] = StartCoroutine(Hatching(time, indexCoro, indexDragon));
        }
    }

    public void EggPanel()
    {
        eggsPanel.SetActive(true);
        warningBlueprintsText.SetActive(false);
        basicDragonsPanel.SetActive(false);
        newDragonsPanel.SetActive(false);
        eggsPanel.transform.GetChild(0).gameObject.SetActive(true);
    }

    public void BasicsDragonsPanel()
    {
        eggsPanel.SetActive(true);
        warningBlueprintsText.SetActive(false);
        basicDragonsPanel.SetActive(true);
        newDragonsPanel.SetActive(false);
        eggsPanel.transform.GetChild(0).gameObject.SetActive(false);
    }

    public void CreateBlueprintsList(int typeInt)
    {
        DragonType type = (DragonType)typeInt;

        int i;
        for (i = 0; i < listOfBlueprints.childCount; i++)
        {
            Destroy(listOfBlueprints.GetChild(i).gameObject);
        }

        i = 0;
        foreach(InventorySlot slot in inventory.slots)
        {
            if (slot.item == null) continue;

            if(slot.item.itemType == ItemType.Blueprint)
            {
                Blueprint b = (Blueprint)slot.item;
                i++;
                if (type == b.type)
                {
                    GameObject newBlueprint = Instantiate(blueprintListPrefab, listOfBlueprints);
                    newBlueprint.GetComponent<BlueprintInfo>().SetUp(b);
                    newBlueprint.GetComponent<Button>().onClick.AddListener(delegate { NewEgg(b.dragon, b); });
                }
            }      
        }

        if(i==0)
        {
            warningBlueprintsText.SetActive(true);
        }

        eggsPanel.transform.GetChild(0).gameObject.SetActive(false);
        newDragonsPanel.SetActive(true);
    }

    public void NewEgg(DragonInfo info)
    {
        Destroy(newDragonUI);
        AddDragon(info);
        dragonList.gameObject.SetActive(false);
        eggsPanel.SetActive(false);
    }

    private void NewEgg(DragonInfo info, Blueprint blueprint)
    {
        if(!inventory.CheckMaterials(blueprint))
        {
            return;
        }

        Destroy(newDragonUI);
        AddDragon(info);  
        eggsPanel.SetActive(false);
        inventory.RemoveItem(blueprint);
    }

    private void OnApplicationQuit()
    {
        Data.Save(myDragons, Inventory.Instance, profileID);
    }

    private void LoadCoroutines()
    {
        float passedTime = Data.PassedTime();
        //Debug.Log("Passed Time: " + passedTime);
        foreach (DragonInfo d in myDragons)
        {
            if (d == null) continue;

            if(d.remainingEatingTime != 0)
            {
                float remainingTime = d.remainingEatingTime - passedTime;

                if (remainingTime <= 0)
                {
                    d.canEat = true;
                    d.remainingEatingTime = 0;
                }
                else
                {
                    StartEating(remainingTime, d);
                }             
            }

            if(d.remainingExpeditionTime != 0)
            {
                //Debug.Log("Expedition Time Left: " + d.remainingExpeditionTime);

                float remainingTime = d.remainingExpeditionTime - passedTime;

                //Debug.Log("Remaining Time: " + remainingTime);

                if (remainingTime <= 0)
                {
                    d.onExpedition = false;
                    d.remainingExpeditionTime = 0;
                }
                else
                {
                    StartExpedition(remainingTime, Database.database.expeditions[d.currentExpedition], d);
                }         
            }

            if(d.remainingHatchingTime != 0)
            {
                float remainingTime = d.remainingHatchingTime - passedTime;

                if (remainingTime <= 0)
                {
                    d.hatching = false;
                    d.toName = true;
                    d.remainingHatchingTime = 0;
                }
                else
                {
                    StartHatching(remainingTime, d);
                }       
            }
        }
    }

    public void HideDragon()
    {
        if(currentEgg != null)
        {
            currentEgg.newDragon.shown = false;
            Destroy(currentEgg.gameObject);
        }
        if(currentDragon != null)
        {
            currentDragon.dragon.shown = false;
            Destroy(currentDragon.gameObject);
        }
        expeditionPanel.SetActive(false);
        arenaPanel.SetActive(false);
    }

    public void ShowText(string text, Color color)
    {
        warningText.text = text;
        warningText.color = color;
        warningText.GetComponent<Animation>().Stop();
        warningText.GetComponent<Animation>().Play();
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene(0);
        Data.Save(Instance.myDragons, Inventory.Instance, profileID);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
