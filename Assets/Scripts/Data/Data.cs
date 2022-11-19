using UnityEngine;
using System.IO;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using UnityEngine.Profiling;

public static class Data
{
    public static void Save(DragonInfo[] myDragons, Inventory inventory, string profileID)
    {
        //Save if first dragon was chosen
        if (!StartedGame(myDragons, profileID))
            return;
        PlayerPrefs.SetInt("StartedGame" + profileID, 1);

        string inventoryPath = Path.Combine(Application.persistentDataPath, profileID, "inventory.txt");
        string dragonsPath = Path.Combine(Application.persistentDataPath, profileID, "dragons.txt");
        string expeditionsPath = Path.Combine(Application.persistentDataPath, profileID, "expeditions.txt");
        Directory.CreateDirectory(Path.GetDirectoryName(inventoryPath));
        Directory.CreateDirectory(Path.GetDirectoryName(dragonsPath));
        Directory.CreateDirectory(Path.GetDirectoryName(expeditionsPath));

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
        PlayerPrefs.SetString("offlineTime" + profileID, GetNetTime().ToBinary().ToString());
    }

    public static void Load(string profileID)
    {
        string inventoryPath = Path.Combine(Application.persistentDataPath, profileID, "inventory.txt");
        string dragonsPath = Path.Combine(Application.persistentDataPath, profileID, "dragons.txt");
        string expeditionsPath = Path.Combine(Application.persistentDataPath, profileID, "expeditions.txt");
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
                    newDragon.prefabEgg = Database.database.dragons[newDragon.ID].prefabEgg;
                    newDragon.prefabBaby = Database.database.dragons[newDragon.ID].prefabBaby;
                    newDragon.prefabAdult = Database.database.dragons[newDragon.ID].prefabAdult;
                    newDragon.lookBaby = Database.database.dragons[newDragon.ID].lookBaby;
                    newDragon.lookAdult = Database.database.dragons[newDragon.ID].lookAdult;
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

                    newDragon.Initialize();
                    dragons[i].LoadEquipment();
                }
                else
                {
                    continue;
                }
            }
            file.Close();
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
                e.SetData();
            }
            file.Close();
        }
    }

    public static void DeleteSave(string profileID)
    {
        string path = Path.Combine(Application.persistentDataPath, profileID);
        Directory.Delete(Path.Combine(path), true);
    }

    public static DateTime GetNetTime()
    {
        var client = new TcpClient("time.nist.gov", 13);
        var localDateTime = DateTime.UtcNow;
        using (var streamReader = new StreamReader(client.GetStream()))
        {
            try
            {
                var response = streamReader.ReadToEnd();
                var utcDateTimeString = response.Substring(7, 17);
                localDateTime = DateTime.ParseExact(utcDateTimeString, "yy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
            }
            catch
            {
            }
        }
        return localDateTime;
    }

    public static float PassedTime(string profileID)
    {
        float passedTime;

        long previousTime = Convert.ToInt64(PlayerPrefs.GetString("offlineTime" + profileID));
        DateTime oldTime = DateTime.FromBinary(previousTime);
        DateTime currentDate = GetNetTime();
        TimeSpan diff = currentDate.Subtract(oldTime);
        passedTime = (float)diff.TotalSeconds;

        return passedTime;
    }

    public static bool NewGame(string profileID)
    {
        string dragonsPath = Path.Combine(Application.persistentDataPath, profileID, "dragons.txt");
        if (File.Exists(dragonsPath))
            return false;
        else return true;
    }

    public static bool StartedGame(DragonInfo[] dragons, string profileID)
    {
        if (PlayerPrefs.GetInt("StartedGame" + profileID, 0) == 1)
            return true;

        if (dragons[0] == null)
            return false;

        return true;
    }
}