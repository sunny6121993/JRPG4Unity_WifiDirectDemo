using UnityEngine;
using System.Collections;

public class WifiP2pEnemyController : MonoBehaviour
{
    public enum EnemyState { Idle, Attack, Defense, Hurt, Die }

    private const int DEMO_HP = 10;
    private const int DEMO_MP = 5;
    private const int DEMO_ATK = 2;
    private const int DEMO_DEF = 1;
    private const float HURT_TIME = 1f;

    private EnemyData enemyData;
    private float rotateX;
    private float rotateZ;
    public EnemyState currentEnemyState;
    private int currentDamage;
    private Rect labelDamageRect;
    private float beginHurtTime;
    private GUIStyle guiStyle;
    private IEnemyAttackHandler enemyAttackHandler;
    private ITurnHandler turnHandler;
    private ICombatSystemController combatSystemController;
    public bool isDefending;
    private Quaternion defaultRotation;

    public int EnemyDef
    {
        get { return enemyData.Def; }
    }

    public IEnemyAttackHandler EnemyAttackHandler
    {
        set { enemyAttackHandler = value; }
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
        enemyData = new EnemyData(DEMO_HP, DEMO_MP, DEMO_ATK, DEMO_DEF);

        defaultRotation = gameObject.transform.rotation;
        rotateX = -30 * Time.deltaTime;
        rotateZ = -30 * Time.deltaTime;

        Vector3 screenPos = Camera.main.WorldToScreenPoint(gameObject.transform.position);
        labelDamageRect = new Rect(screenPos.x + 50, screenPos.y - 200, 100, 20);
        guiStyle = new GUIStyle();
        guiStyle.normal.textColor = Color.red;
        guiStyle.fontSize = 40;
        guiStyle.fontStyle = FontStyle.Bold;

        currentEnemyState = EnemyState.Idle;
        isDefending = false;
	}

    private void idleMovement()
    {
        gameObject.transform.Rotate(rotateX, 0f, rotateZ);
    }

    private void nextCommand()
    {
        /*int attackChance = ((float)enemyData.Hp / DEMO_HP > 0.3f) ? 95 : 70;
        int random = Random.Range(0, 100);
        
        if (random < attackChance)
        {
            currentEnemyState = EnemyState.Attack;
        }
        else
        {
            currentEnemyState = EnemyState.Defense;
            isDefending = true;
            turnHandler.OnTurnEnd((int)WifiP2pController.Turn.Player2);
        }
        */
		////////////////
        /*
        currentEnemyState = EnemyState.Defense;
        isDefending = true;
        turnHandler.OnTurnEnd((int)CombatSystemController.Turn.Enemy);
        */
    }


    private void defencePose()
    {
        gameObject.transform.rotation = new Quaternion(0, 0, 0, 0);
    }
	
	// Update is called once per frame
	void Update()
    {
        switch (currentEnemyState)
        {
            case EnemyState.Idle:
                idleMovement();
				if (turnHandler.GetCurrentTurn() == (int)WifiP2pController.Turn.Player2)
                {
                    nextCommand();
                }
                break;
            case EnemyState.Attack:
                enemyAttackHandler.OnEnemyAttack(enemyData.Atk);
				turnHandler.OnTurnEnd((int)WifiP2pController.Turn.Player2);
                currentEnemyState = EnemyState.Idle;
                break;
            case EnemyState.Defense:
                defencePose();
				if (turnHandler.GetCurrentTurn() == (int)WifiP2pController.Turn.Player2)
                {
                    reset();
                }
                break;
            case EnemyState.Hurt:
                if (Time.time - beginHurtTime > HURT_TIME)
                {
                    currentEnemyState = isDefending ? EnemyState.Defense : EnemyState.Idle;
                }
                break;
            case EnemyState.Die:
				combatSystemController.OnCombatEnd((int)WifiP2pController.Force.Player2);
                Destroy(gameObject);
                break;
        }
	}

    public void Hurt(int damage)
    {
        currentEnemyState = EnemyState.Hurt;
        beginHurtTime = Time.time;

        if (isDefending)
        {
            damage = (int)((float)damage * 0.7f);
        }

        currentDamage = damage;
        enemyData.Hp -= damage;
        if (enemyData.Hp <= 0)
        {
            currentEnemyState = EnemyState.Die;
        }
    }

    void OnGUI()
    {
        if (currentEnemyState == EnemyState.Hurt)
        {
            GUI.Label(labelDamageRect, currentDamage.ToString(), guiStyle);
        }
    }

    public void reset()
    {
        isDefending = false;
        currentEnemyState = EnemyState.Idle;
        gameObject.transform.rotation = defaultRotation;
    }
	public void changeTurn(int current){
		turnHandler.OnTurnEnd(current);
	}
}
