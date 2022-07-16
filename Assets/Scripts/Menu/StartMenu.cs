using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI[] profiles;
    [SerializeField] private Button[] deleteButtons;

    private void Start()
    {
        SetButtons();
        for (int i = 0; i < deleteButtons.Length; i++)
        {
            int ID = i;
            deleteButtons[i].onClick.AddListener(delegate { DeleteSave("Profile" + ID); });
        }
    }

    private void SetButtons()
    {
        for (int i = 0; i < profiles.Length; i++)
        {
            if (Data.NewGame("Profile" + i))
            {
                profiles[i].text = "New Game";
                deleteButtons[i].interactable = false;
            }
            else
            {
                profiles[i].text = "Continue Game";
                deleteButtons[i].interactable = true;
            }
            int ID = i;
            profiles[i].GetComponentInParent<Button>().onClick.AddListener(delegate { SetProfileID("Profile" + ID); });
        }
    }

    private void SetProfileID(string profileID)
    {
        GameController.profileID = profileID;
        SceneManager.LoadScene(1);
    }

    public void DeleteSave(string profileID)
    {
        Data.DeleteSave(profileID);
        SetButtons();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
