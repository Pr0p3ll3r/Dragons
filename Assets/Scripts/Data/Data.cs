using UnityEngine;
using System.IO;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net;
using System.Globalization;

public static class Data
{
    private static string inventoryPath = Application.dataPath + "/inventory.txt";
    private static string dragonsPath = Application.dataPath + "/dragons.txt";
    private static string expeditionsPath = Application.dataPath + "/expeditions.txt";

    public static void Save(DragonInfo[] myDragons, Inventory inventory)
    {
        string data;
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file;
        file = File.Create(dragonsPath);
        for (int i = 0; i < myDragons.Length; i++)
        {
            data = JsonUtility.ToJson(myDragons[i]);
            bf.Serialize(file, data);
            if(myDragons[i] != null)
            {
                for (int j = 0; j < myDragons[i].equipment.Length; j++)
                {
                    SaveItem saveItem;
                    Equipment[] equipment = myDragons[i].equipment;
                    if (equipment[j] == null)
                        saveItem = new SaveItem();
                    else
                        saveItem = new SaveItem(equipment[j].ID, 1, equipment[j].stats);
                    data = JsonUtility.ToJson(saveItem);
                    bf.Serialize(file, data);
                }
            }
        }
        file.Close();

        //Save inventory data
        file = File.Create(inventoryPath);
        data = JsonUtility.ToJson(inventory.resources);
        bf.Serialize(file, data);

        //Save inventory slots
        for (int i = 0; i < inventory.slots.Length; i++)
        {
            SaveItem saveItem;
            if (inventory.slots[i].item == null)
                saveItem = new SaveItem();
            else
                saveItem = new SaveItem(inventory.slots[i].item.ID, inventory.slots[i].amount);
            data = JsonUtility.ToJson(saveItem);
            bf.Serialize(file, data);
        }
        file.Close();

        //Save expedition 
        file = File.Create(expeditionsPath);
        for (int i = 0; i < Database.database.expeditions.Length; i++)
        {
            Expedition e = Database.database.expeditions[i];
            SaveExpedition saveExpedition = new SaveExpedition(e.level, e.exp);
            data = JsonUtility.ToJson(saveExpedition);
            bf.Serialize(file, data);
        }
        file.Close();

        //Save current time
        PlayerPrefs.SetString("offlineTime", GetNetTime().ToBinary().ToString());
    }

    public static void Load()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file;
        if (File.Exists(dragonsPath))
        {
            file = File.Open(dragonsPath, FileMode.Open);
            DragonInfo[] dragons = GameController.Instance.myDragons;
            for (int i = 0; i < dragons.Length; i++)
            {
                DragonInfo newDragon = ScriptableObject.CreateInstance<DragonInfo>();
                JsonUtility.FromJsonOverwrite(bf.Deserialize(file).ToString(), newDragon);
                if (newDragon.dragonName != string.Empty)
                {
                    GameController.Instance.myDragons[i] = newDragon;
                    newDragon.shown = false;
                    newDragon.look = Database.database.dragons[newDragon.ID].look;
                    newDragon.eggLook = Database.database.dragons[newDragon.ID].eggLook;

                    for (int j = 0; j < dragons[i].equipment.Length; j++)
                    {
                        SaveItem saveItem = new SaveItem();
                        JsonUtility.FromJsonOverwrite(bf.Deserialize(file).ToString(), saveItem);
                        if (saveItem.ID == -1)
                        {
                            dragons[i].equipment[j] = null;
                        }
                        else
                        {
                            dragons[i].equipment[j] = (Equipment)Database.database.items[saveItem.ID].GetCopy();
                            dragons[i].equipment[j].stats = saveItem.stats;
                        }
                    }
                }
                else
                {
                    break;
                }
            }
        }

        if (File.Exists(inventoryPath))
        {
            file = File.Open(inventoryPath, FileMode.Open);
            JsonUtility.FromJsonOverwrite(bf.Deserialize(file).ToString(), Inventory.Instance.resources);
            InventorySlot[] slots = Inventory.Instance.slots;
            for (int i = 0; i < slots.Length; i++)
            {
                InventorySlot slot = slots[i];
                SaveItem saveItem = new SaveItem();
                JsonUtility.FromJsonOverwrite(bf.Deserialize(file).ToString(), saveItem);
                if (saveItem.ID == -1)
                {
                    slot.item = null;
                    slot.amount = 0;
                }
                else
                {
                    slot.item = Database.database.items[saveItem.ID].GetCopy();
                    slot.amount = saveItem.amount;
                }
            }
            file.Close();
        }

        if(File.Exists(expeditionsPath))
        {
            file = File.Open(expeditionsPath, FileMode.Open);
            for (int i = 0; i < Database.database.expeditions.Length; i++)
            {
                Expedition e = Database.database.expeditions[i];
                SaveExpedition saveExpedition = new SaveExpedition();
                JsonUtility.FromJsonOverwrite(bf.Deserialize(file).ToString(), saveExpedition);
                e.level = saveExpedition.level;
                e.exp = saveExpedition.exp;
                e.currentData = e.data[e.level - 1];
            }
            file.Close();
        }
    }

    public static DateTime GetNetTime()
    {
        var myHttpWebRequest = (HttpWebRequest)WebRequest.Create("http://www.google.com");
        var response = myHttpWebRequest.GetResponse();
        string todaysDates = response.Headers["date"];
        return DateTime.ParseExact(todaysDates,
        "ddd, dd MMM yyyy HH:mm:ss 'GMT'",
        CultureInfo.InvariantCulture.DateTimeFormat,
        DateTimeStyles.AssumeUniversal);
    }

    public static float PassedTime()
    {
        float passedTime;

        long previousTime = Convert.ToInt64(PlayerPrefs.GetString("offlineTime"));
        DateTime oldTime = DateTime.FromBinary(previousTime);
        DateTime currentDate = GetNetTime();
        TimeSpan diff = currentDate.Subtract(oldTime);
        passedTime = (float)diff.TotalSeconds;

        return passedTime;
    }

    public static bool NewGame()
    {
        if (File.Exists(dragonsPath))
            return false;
        else return true;
    }

    public static int GetIdByName(string _name, object[] array)
    {
        Resource[] resources;
        if (array.GetType() == typeof(Resource))
        {
            resources = (Resource[])array;
            for (int i = 0; i < resources.Length; i++)
            {
                if (resources[i].name == _name)
                    return i;
            }
        }
            
        return -1;
    }
}