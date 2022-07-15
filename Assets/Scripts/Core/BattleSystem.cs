using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST, EMPTY }

public class BattleSystem : MonoBehaviour
{
    public BattleState state;

    public TextMeshProUGUI stateText;
    public TextMeshProUGUI battleText;

    public BattleHUD playerHUD;
    public BattleHUD enemyHUD;

    DragonInfo player;
    DragonInfo enemy;

    public GameObject battlePanel;
    public GameObject mainPanel;

    public GameObject reward;
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI silverText;
    public Button endButton;

    private void Start()
    {
        endButton.onClick.AddListener(delegate { CloseBattle(); });
    }

    public void SetupBattle(DragonInfo yourDragon, DragonInfo enemyDragon)
    {
        endButton.gameObject.SetActive(false);
        reward.SetActive(false);
        battlePanel.SetActive(true);
        mainPanel.SetActive(false);
        battleText.text = "";
        state = BattleState.START;
        enemy = enemyDragon;
        player = yourDragon;
        playerHUD.SetHUD(player);
        enemyHUD.SetHUD(enemy);
        state = BattleState.PLAYERTURN;
        StartCoroutine(PlayerTurn());
    }

    IEnumerator PlayerTurn()
    {
        stateText.text = "Your turn";

        yield return new WaitForSeconds(1f);

        state = BattleState.EMPTY;

        int damage = player.stats[(int)StatType.Damage].GetValue();
        bool isDead = enemy.TakeDamage(damage);
        enemyHUD.UpdateHealthBar(enemy);

        battleText.color = Color.green;
        battleText.text = $"You dealt {damage} damage";

        yield return new WaitForSeconds(1f);

        if (isDead)
        {
            state = BattleState.WON;
            StartCoroutine(EndBattle());
        }
        else
        {
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
    }

    IEnumerator EnemyTurn()
    {
        stateText.text = "Enemy turn";

        yield return new WaitForSeconds(1f);

        int enemyDamage = enemy.stats[(int)StatType.Damage].GetValue();
        bool isDead = player.TakeDamage(enemyDamage);
        playerHUD.UpdateHealthBar(player);

        battleText.color = Color.red;
        battleText.text = $"You took {enemyDamage} damage ";

        yield return new WaitForSeconds(1f);

        if (isDead)
        {
            state = BattleState.LOST;
            StartCoroutine(EndBattle());
        }
        else
        {
            state = BattleState.PLAYERTURN;
            StartCoroutine(PlayerTurn());
        }
    }

    public void OnEscapeButton()
    {
        //if (state != BattleState.PLAYERTURN)
        //    return;

        state = BattleState.EMPTY;
        StartCoroutine(EndBattle());
    }

    IEnumerator EndBattle()
    {
        if (state == BattleState.WON)
        {
            stateText.text = "YOU WON!";
            battleText.text = "";

            yield return new WaitForSeconds(2f);

            Rewards();
        }
        else if (state == BattleState.LOST)
        {
            stateText.text = "YOU LOST";
            battleText.text = "";

            yield return new WaitForSeconds(2f);

            ShowEndButton();
        }
        else
        {
            enemy.Heal();
            CloseBattle();
        }
    }

    void Rewards()
    {
        enemy.Heal();
        goldText.text = enemy.rewardGold.ToString();
        silverText.text = enemy.rewardSilver.ToString();

        reward.SetActive(true);
        ShowEndButton();
    }

    void ShowEndButton()
    {
        endButton.gameObject.SetActive(true);
    }

    void CloseBattle()
    {
        battlePanel.SetActive(false);
        mainPanel.SetActive(true);
    }
}

