using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST, EMPTY }

public class BattleSystem : MonoBehaviour
{
    [SerializeField] private BattleState state;

    [SerializeField] private TextMeshProUGUI stateText;
    [SerializeField] private TextMeshProUGUI battleText;

    [SerializeField] private BattleHUD playerHUD;
    [SerializeField] private BattleHUD enemyHUD;

    private DragonInfo player;
    private DragonInfo enemy;

    [SerializeField] private GameObject battlePanel;
    [SerializeField] private GameObject mainPanel;

    [SerializeField] private GameObject reward;
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private TextMeshProUGUI silverText;
    [SerializeField] private Button endButton;
    [SerializeField] private Button escapeButton;

    private int playerSpecialSkill = 0;
    private int enemySpecialSkill = 0;

    private void Start()
    {
        endButton.onClick.AddListener(delegate { CloseBattle(); });
        escapeButton.onClick.AddListener(delegate { Escape(); });
    }

    public void SetupBattle(DragonInfo yourDragon, DragonInfo enemyDragon)
    {
        endButton.gameObject.SetActive(false);
        reward.SetActive(false);
        battlePanel.SetActive(true);
        mainPanel.SetActive(false);
        battleText.text = "";
        state = BattleState.START;
        enemy = enemyDragon.GetCopy();
        player = yourDragon;
        playerSpecialSkill = 0;
        enemySpecialSkill = 0;
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
        if (playerSpecialSkill == 100)
        {
            damage *= 2;
            playerSpecialSkill = 0;
        }
        else
        {
            playerSpecialSkill += player.stats[(int)StatType.Luck].GetValue();
            playerSpecialSkill = Mathf.Clamp(playerSpecialSkill, 0, 100);
        }
        playerHUD.UpdateSkillBar(playerSpecialSkill);

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
        if (enemySpecialSkill == 100)
        {
            enemyDamage *= 2;
            enemySpecialSkill = 0;
        }
        else
        {
            enemySpecialSkill += enemy.stats[(int)StatType.Luck].GetValue();
            enemySpecialSkill = Mathf.Clamp(enemySpecialSkill, 0, 100);
        }
        enemyHUD.UpdateSkillBar(enemySpecialSkill);

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

    private void Escape()
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
            escapeButton.gameObject.SetActive(false);

            yield return new WaitForSeconds(2f);

            Rewards();
        }
        else if (state == BattleState.LOST)
        {
            stateText.text = "YOU LOST";
            battleText.text = "";
            escapeButton.gameObject.SetActive(false);

            yield return new WaitForSeconds(2f);

            endButton.gameObject.SetActive(true);
        }
        else
        {          
            CloseBattle();
            StopAllCoroutines();
        }
    }

    void Rewards()
    {
        goldText.text = enemy.rewardGold.ToString();
        silverText.text = enemy.rewardSilver.ToString();

        reward.SetActive(true);
        endButton.gameObject.SetActive(true);
    }

    void CloseBattle()
    {
        battlePanel.SetActive(false);
        mainPanel.SetActive(true);
    }
}
