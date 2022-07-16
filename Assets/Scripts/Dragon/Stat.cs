using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatType
{
    Damage,
    Defense,
    Strength,
    Vitality,
    Luck 
}

[System.Serializable]
public class Stat
{
    [SerializeField] private int baseValue;
    public StatType statType;

    private readonly List<int> modifiers = new List<int>();

    public Stat(int _baseValue, StatType _statType)
    {
        baseValue = _baseValue;
        statType = _statType;
        modifiers = new List<int>();
    }

    public int GetValue()
    {
        int finalValue = baseValue;
        if(modifiers != null)
        {
            for (int i = 0; i < modifiers.Count; i++)
            {
                finalValue += modifiers[i];
            }
        }
        return finalValue;
    }

    public void AddPoint()
    {
        baseValue++;
    }

    public void AddModifier(int modifier)
    {
        modifiers.Add(modifier);
    }

    public void RemoveModifier(int modifier)
    {
        modifiers.Remove(modifier);
    }
}
