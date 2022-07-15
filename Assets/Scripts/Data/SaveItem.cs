[System.Serializable]
public class SaveItem
{
    public int ID;
    public int amount;
    public ItemStat[] stats;
    public SaveItem()
    {
        ID = -1;
        amount = 0;
        stats = null;
    }
    public SaveItem(int _ID, int _amount)
    {
        ID = _ID;
        amount = _amount;
        stats = null;
    }
    public SaveItem(int _ID, int _amount, ItemStat[] _stats)
    {
        ID = _ID;
        amount = _amount;
        stats = new ItemStat[_stats.Length];
        stats = _stats;
    }
}
