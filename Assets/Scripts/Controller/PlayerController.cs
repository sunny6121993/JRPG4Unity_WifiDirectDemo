using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour, IAttackHandler, IDefenceHandler
{
    public enum PlayerState { Idle, Attack, Defense, Hurt, Die }

    public const int MIN_PARTY_MEMBER_NUM = 1;

    private const int DEMO_HP = 10;
    private const int DEMO_MP = 10;
    private const int DEMO_ATK = 2;
    private const int DEMO_DEF = 1;
    private const float HURT_TIME = 1f;

    private int partyMemberNum;
    private PlayerData[] playerDatas;
    private PlayerGui[] playerGuis;
    private PlayerState currentPlayerState;
    private IPLayerAttackHandler playerAttackHandler;
    private ITurnHandler turnHandler;
    private float beginHurtTime;
    private int currentDamage;
    private ICombatSystemController combatSystemController;
    private bool isDefending;

    public int PartyMemberNum
    {
        get { return partyMemberNum; }

        set { partyMemberNum = Mathf.Max(MIN_PARTY_MEMBER_NUM, value); }
    }

    public PlayerState CurrentPlayerState
    {
        get { return currentPlayerState; }
    }

    public int GetPlayerDef(int playerNo)
    {
        return playerDatas[playerNo].Def;
    }

    public IPLayerAttackHandler PlayerAttackHandler
    {
        set { playerAttackHandler = value; }
    }

    public ITurnHandler TurnHandler
    {
        set { turnHandler = value; }
    }

    public ICombatSystemController SystemController
    {
        set { combatSystemController = value; }
    }

	// Use this for initialization
	void Start()
    {
        playerDatas = new PlayerData[PartyMemberNum];
        for (int i = 0; i < playerDatas.Length; i++)
        {
            playerDatas[i] = new PlayerData(DEMO_HP, DEMO_MP, DEMO_ATK, DEMO_DEF);
        }

        playerGuis = new PlayerGui[PartyMemberNum];
        for (int i = 0; i < playerGuis.Length; i++)
        {
            playerGuis[i] = gameObject.AddComponent<PlayerGui>();
            playerGuis[i].AttackHandler = this;
            playerGuis[i].DefenceHandler = this;
        }

        currentPlayerState = PlayerState.Idle;
        isDefending = false;
	}

    // Update is called once per frame
    void Update()
    {
        switch (currentPlayerState)
        {
            case PlayerState.Idle:
                break;
            case PlayerState.Attack:
                playerAttackHandler.OnPlayerAttack(playerDatas[0].Atk);
                turnHandler.OnTurnEnd((int)CombatSystemController.Turn.Player);
                currentPlayerState = PlayerState.Idle;
                break;
            case PlayerState.Defense:
                if (turnHandler.GetCurrentTurn() == (int)CombatSystemController.Turn.Player)
                {
                    reset();
                }
                break;
            case PlayerState.Hurt:
                if (Time.time - beginHurtTime > HURT_TIME)
                {
                    playerGuis[0].IsDamageShown = false;
                    currentPlayerState = isDefending ? PlayerState.Defense : PlayerState.Idle;
                }
                break;
            case PlayerState.Die:
                combatSystemController.OnCombatEnd((int)CombatSystemController.Force.Player);
                break;
        }
	}

    public void OnAttack()
    {
        currentPlayerState = PlayerState.Attack;
        playerGuis[0].IsCommandShown = false;
    }

    public void OnDefence()
    {
        isDefending = true;
        currentPlayerState = PlayerState.Defense;
        playerGuis[0].IsCommandShown = false;
        turnHandler.OnTurnEnd((int)CombatSystemController.Turn.Player);
    }

    public void Hurt(int playerNo, int damage)
    {
        currentPlayerState = PlayerState.Hurt;
        beginHurtTime = Time.time;

        if (isDefending)
        {
            damage = (int)((float)damage * 0.7f);
        }

        playerGuis[playerNo].CurrentDamage = damage;
        playerGuis[playerNo].IsDamageShown = true;
        playerDatas[playerNo].Hp -= damage;
        playerGuis[playerNo].HpBarLengthRatio = (float)playerDatas[playerNo].Hp / DEMO_HP;
        if (playerDatas[playerNo].Hp <= 0)
        {
            currentPlayerState = PlayerState.Die;
        }
    }

    public void ShowCommand()
    {
        if (currentPlayerState == PlayerState.Idle)
        {
            playerGuis[0].IsCommandShown = true;
        }
    }

    public void reset()
    {
        isDefending = false;
        currentPlayerState = PlayerState.Idle;
    }
}
