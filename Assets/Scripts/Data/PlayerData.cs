using UnityEngine;

public class PlayerData
{
    public const int MIN_HP = 0;
    public const int MIN_MP = 0;
    public const int MIN_ATK = 1;
    public const int MIN_DEF = 1;

    private int hp;
    private int mp;
    private int atk;
    private int def;

    public int Hp
    {
        get { return hp; }

        set { hp = Mathf.Max(MIN_HP, value); }
    }

    public int Mp
    {
        get { return mp; }

        set { mp = Mathf.Max(MIN_MP, value); }
    }

    public int Atk
    {
        get { return atk; }

        set { atk = Mathf.Max(MIN_ATK, value); }
    }

    public int Def
    {
        get { return def; }

        set { def = Mathf.Max(MIN_DEF, value); }
    }

    public PlayerData(int hp, int mp, int atk, int def)
    {
        Hp = hp;
        Mp = mp;
        Atk = atk;
        Def = def;
    }
}
