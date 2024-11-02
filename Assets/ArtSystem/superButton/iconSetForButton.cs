using UnityEngine;

public enum superButtonType
{
    act,
    attack,
    bp,
    change_atk,
    change_def,
    ep,
    mp,
    no,
    see,
    set,
    set_monster,
    set_spell_trap,
    set_pendulum,
    spsummon,
    summon,
    pendulumSummon,
    yes,
    decide
}

public class iconSetForButton : MonoBehaviour
{
    public UILabel UILabelInButton;
    public Texture2D act;
    public Texture2D attack;
    public Texture2D bp;
    public Texture2D change;
    public Texture2D ep;
    public Texture2D mp;
    public Texture2D no;
    public Texture2D see;
    public Texture2D set;
    public Texture2D spsummon;
    public Texture2D summon;
    public Texture2D yes;

    public Sprite[] act_;
    public Sprite[] attack_;
    public Sprite[] yes_;
    public Sprite[] no_;
    public Sprite[] spsummon_;
    public Sprite[] summon_;
    public Sprite[] pendulumSummon_;
    public Sprite[] set_monster;
    public Sprite[] set_spell_trap;
    public Sprite[] set_pendulum;
    public Sprite[] change_atk;
    public Sprite[] change_def;
    public Sprite[] decide_;

    void SetSpriteSet(Sprite[] sprites)
    {
        GetComponent<UIButton>().normalSprite2D = sprites[0];
        GetComponent<UIButton>().hoverSprite2D = sprites[1];
        GetComponent<UIButton>().pressedSprite2D = sprites[2];
        GetComponent<UIButton>().disabledSprite2D = sprites[3];
    }

    public void setTexture(superButtonType type)
    {
        switch (type)
        {
            case superButtonType.act:
                SetSpriteSet(act_);
                break;
            case superButtonType.attack:
                SetSpriteSet (attack_);
                break;
            //case superButtonType.bp:
            //    UITextureInButton.mainTexture = bp;
            //    break;
            case superButtonType.change_atk:
                SetSpriteSet(change_atk);
                break;
            case superButtonType.change_def:
                SetSpriteSet(change_def);
                break;
            //case superButtonType.ep:
            //    UITextureInButton.mainTexture = ep;
            //    break;
            //case superButtonType.mp:
            //    UITextureInButton.mainTexture = mp;
            //    break;
            case superButtonType.no:
                SetSpriteSet(no_);
                break;
            //case superButtonType.see:
            //    UITextureInButton.mainTexture = see;
            //    break;
            case superButtonType.set:
                SetSpriteSet(set_spell_trap);
                break;
            case superButtonType.set_monster:
                SetSpriteSet(set_monster);
                break;
            case superButtonType.set_spell_trap:
                SetSpriteSet(set_spell_trap);
                break;
            case superButtonType.set_pendulum:
                SetSpriteSet(set_pendulum);
                break;
            case superButtonType.spsummon:
                SetSpriteSet(spsummon_);
                break;
            case superButtonType.summon:
                SetSpriteSet(summon_);
                break;
            case superButtonType.pendulumSummon:
                SetSpriteSet(pendulumSummon_);
                break;
            case superButtonType.yes:
                SetSpriteSet(yes_);
                break;
            case superButtonType.decide:
                SetSpriteSet(decide_);
                break;
        }

        //Color c;
        //ColorUtility.TryParseHtmlString(Config.Getui("gameButtonSign.color"), out c);
        //UITextureInButton.color = c;
    }

    public void setText(string hint)
    {
        UILabelInButton.text = hint;
    }
}