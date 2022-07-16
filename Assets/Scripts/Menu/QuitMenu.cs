using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void BackToMenu()
    {
        Data.Save(GameController.Instance.myDragons, Inventory.Instance, GameController.profileID);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
