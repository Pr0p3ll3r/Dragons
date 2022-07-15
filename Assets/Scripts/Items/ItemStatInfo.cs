using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemStatInfo : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI statText;

    public void SetUp(string text)
    {
        statText.text = text;
    }

    public void SetUp(string text, Sprite _icon)
    {
        statText.text = text;
        icon.sprite = _icon;
    }

    public void SetColor(Color color)
    {
        statText.color = color;
    }
}
