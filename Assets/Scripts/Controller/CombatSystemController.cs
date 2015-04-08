using UnityEngine;
using System.Collections;

public class CombatSystemController : MonoBehaviour, ICombatSystemController, IPLayerAttackHandler, ITurnHandler, IEnemyAttackHandler
{
    public enum Turn { Player, Enemy }
    public enum Force { Player, Enemy }

    public int partyMemberNum = 1;
    public int enemyNum = 1;
    public GameObject enemy;

    private const string GAME_OVER_TEXT = "Game Over";
    private const string PLAYER_WIN_TEXT = "You Win";

    private PlayerController playerController;
    private EnemyController[] enemyControllers;
    private Turn currentTurn;
    private bool isCombatEnd;
    private string resultText;
    private GUIStyle style;
    private Rect resultRect;

	// Use this for initialization
	void Start()
    {
        isCombatEnd = false;

        playerController = gameObject.AddComponent<PlayerController>();
        playerController.PartyMemberNum = partyMemberNum;
        playerController.PlayerAttackHandler = this;
        playerController.TurnHandler = this;
        playerController.SystemController = this;

        enemyControllers = new EnemyController[enemyNum];

        GameObject enemyClone = null;
        for (int i = 0; i < enemyNum; i++)
        {
            enemyClone = Instantiate(enemy) as GameObject;
            enemyControllers[i] = enemyClone.GetComponent<EnemyController>();
            enemyControllers[i].EnemyAttackHandler = this;
            enemyControllers[i].TurnHandler = this;
            enemyControllers[i].SystemController = this;
        }

        currentTurn = Turn.Player;

        style = new GUIStyle();
        style.alignment = TextAnchor.MiddleCenter;
        style.fontStyle = FontStyle.Bold;
        style.fontSize = 30;
        resultRect = new Rect(Screen.width / 2f - 50, Screen.height / 2f - 15, 100, 30);
	}

    // Update is called once per frame
    void Update()
    {
        switch (currentTurn)
        {
            case Turn.Player:
                playerController.ShowCommand();
                break;
            case Turn.Enemy:
                break;
        }
	}

    void OnGUI()
    {
        if (isCombatEnd)
        {
            GUI.Label(resultRect, resultText, style);
        }
    }

    private int calculateDamage(int atk, int def)
    {
        return atk - def + Random.Range(atk, def);
    }

    public void OnPlayerAttack(int playerAtk)
    {
        enemyControllers[0].Hurt(calculateDamage(playerAtk, enemyControllers[0].EnemyDef));
    }

    public int GetCurrentTurn()
    {
        return (int)currentTurn;
    }

    public void OnTurnEnd(int currentTurn)
    {
        switch (currentTurn)
        {
            case (int)Turn.Player:
                this.currentTurn = Turn.Enemy;
                break;
            case (int)Turn.Enemy:
                this.currentTurn = Turn.Player;
                break;
        }
    }

    public void OnEnemyAttack(int enemyAtk)
    {
        playerController.Hurt(0, calculateDamage(enemyAtk, playerController.GetPlayerDef(0)));
    }

    public void OnCombatEnd(int loseForce)
    {
        isCombatEnd = true;
        switch (loseForce)
        {
            case (int)Force.Player:
                resultText = GAME_OVER_TEXT;
                style.normal.textColor = Color.red;
                break;
            case (int)Force.Enemy:
                resultText = PLAYER_WIN_TEXT;
                style.normal.textColor = Color.blue;
                break;
        }
    }
}
