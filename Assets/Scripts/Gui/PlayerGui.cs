using UnityEngine;
using System.Collections;

public class PlayerGui : MonoBehaviour
{
    private const int PARTY_RECT_WIDTH = 300;
    private const int PARTY_RECT_HEIGHT = 90;
    private const string LABEL_HP = "HP";
    private const string LABEL_MP = "MP";
    private const int PLAYER_LABEL_WIDTH = 50;
    private const int PLAYER_LABEL_HEIGHT = 20;
    private const int PLAYER_BAR_HEIGHT = 13;
    private const int BUTTON_RECT_WIDTH = 200;
    private const int BUTTON_RECT_HEIGHT = 80;
    private const string LABEL_ATTACK = "Attack";
    private const string LABEL_DEFENSE = "Defense";

    private Texture2D playerIcon;

    private Texture2D partyBgTexture,
                      hpNormalTexture,
                      hpDangerTexture,
                      mpTexture;
    private Rect partyBgRect,
                 playerIconRect,
                 playerHpLabelRect,
                 playerHpBarRect,
                 playerMpLabelRect,
                 playerMpBarRect,
                 buttonAttackRect,
                 buttonDefenceRect,
                 labelDamageRect;
    private float playerIconWidth,
                  hpBarLengthRatio,
                  barWidth;
    private bool isCommandShown,
                 isDamageShown;
    private IAttackHandler attackHandler;
    private IDefenceHandler defenceHandler;
    private int currentDamage;
    private GUIStyle guiStyle, commandStyle;

    public bool IsCommandShown
    {
        get { return isCommandShown; }

        set
        {
            if (value != isCommandShown)
            {
                isCommandShown = value;
            }
        }
    }

    public IAttackHandler AttackHandler
    {
        set { attackHandler = value; }
    }

    public IDefenceHandler DefenceHandler
    {
        set { defenceHandler = value; }
    }

    public int CurrentDamage
    {
        set { currentDamage = value; }
    }

    public bool IsDamageShown
    {
        get { return isDamageShown; }

        set
        {
            if (value != isDamageShown)
            {
                isDamageShown = value;
            }
        }
    }

    public float HpBarLengthRatio
    {
        set { hpBarLengthRatio = Mathf.Max(0, value); }
    }

    // Use this for initialization
    void Start()
    {
        playerIcon = Resources.Load("Textures/playerIcon") as Texture2D;

        partyBgRect = new Rect(Screen.width / 2f - PARTY_RECT_WIDTH / 2f, Screen.height * 8 / 10f, PARTY_RECT_WIDTH, PARTY_RECT_HEIGHT);
        partyBgTexture = new Texture2D((int)partyBgRect.width, (int)partyBgRect.height);
        setTextureColor(ref partyBgTexture, new Color32(127, 127, 255, 127));
        partyBgTexture.Apply();

        playerIconWidth = partyBgRect.height - 20;
        playerIconRect = new Rect(partyBgRect.x + 10, partyBgRect.y + 10, playerIconWidth, playerIconWidth);

        playerHpLabelRect = new Rect(playerIconRect.x + playerIconWidth + 10, playerIconRect.y, PLAYER_LABEL_WIDTH, PLAYER_LABEL_HEIGHT);
        playerHpBarRect = new Rect(playerHpLabelRect.x, playerHpLabelRect.y + PLAYER_LABEL_HEIGHT, partyBgTexture.width - 30 - playerIconRect.width, PLAYER_BAR_HEIGHT);
        playerMpBarRect = new Rect(playerHpLabelRect.x, partyBgRect.y + PARTY_RECT_HEIGHT - 10 - PLAYER_BAR_HEIGHT, partyBgTexture.width - 30 - playerIconRect.width, PLAYER_BAR_HEIGHT);
        playerMpLabelRect = new Rect(playerHpLabelRect.x, playerMpBarRect.y - PLAYER_LABEL_HEIGHT, PLAYER_LABEL_WIDTH, PLAYER_LABEL_HEIGHT);

        barWidth = playerHpBarRect.width;

        hpNormalTexture = new Texture2D((int)playerHpBarRect.width, (int)playerHpBarRect.height);
        setTextureColor(ref hpNormalTexture, Color.green);
        hpNormalTexture.Apply();

        hpDangerTexture = new Texture2D((int)playerHpBarRect.width, (int)playerHpBarRect.height);
        setTextureColor(ref hpDangerTexture, Color.red);
        hpDangerTexture.Apply();

        mpTexture = new Texture2D((int)playerMpBarRect.width, (int)playerMpBarRect.height);
        setTextureColor(ref mpTexture, Color.blue);
        mpTexture.Apply();

        buttonAttackRect = new Rect(30, Screen.height / 2f - 5 - BUTTON_RECT_HEIGHT, BUTTON_RECT_WIDTH, BUTTON_RECT_HEIGHT);
        buttonDefenceRect = new Rect(buttonAttackRect.x, Screen.height / 2f + 5, BUTTON_RECT_WIDTH, BUTTON_RECT_HEIGHT);

        labelDamageRect = new Rect(partyBgRect.xMax - 50, playerHpLabelRect.y - 10, 100, 20);
        guiStyle = new GUIStyle();
        guiStyle.normal.textColor = Color.red;
        guiStyle.fontSize = 25;
        guiStyle.fontStyle = FontStyle.Bold;

        hpBarLengthRatio = 1f;

        commandStyle = new GUIStyle();
        commandStyle.fontStyle = FontStyle.Bold;
        commandStyle.fontSize = 50;
    }

    private void setTextureColor(ref Texture2D texture, Color color)
    {
        for (int i = 0; i < texture.width; i++)
        {
            for (int j = 0; j < texture.height; j++)
            {
                texture.SetPixel(i, j, color);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        playerHpBarRect.width = barWidth * hpBarLengthRatio;
    }

    void OnGUI()
    {
        GUI.DrawTexture(partyBgRect, partyBgTexture);
        GUI.DrawTexture(playerIconRect, playerIcon);
        GUI.Label(playerHpLabelRect, LABEL_HP);
        GUI.DrawTexture(playerHpBarRect, hpNormalTexture);
        GUI.Label(playerMpLabelRect, LABEL_MP);
        GUI.DrawTexture(playerMpBarRect, mpTexture);

        if (isCommandShown)
        {
            if (GUI.Button(buttonAttackRect, LABEL_ATTACK))
            {
                attackHandler.OnAttack();
            }

            if (GUI.Button(buttonDefenceRect, LABEL_DEFENSE))
            {
                defenceHandler.OnDefence();
            }
        }

        if (isDamageShown)
        {
            GUI.Label(labelDamageRect, currentDamage.ToString(), guiStyle);
        }
    }
}
