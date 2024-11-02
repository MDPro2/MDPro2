using System.Collections;
using UnityEngine;
using YGOSharp.OCGWrapper.Enums;

public class CardAnimation : MonoBehaviour
{
    public int activateType = 0;
    public bool activated = false;
    public bool bigLanding = false;
    Material cardfaceMaterial;
    public int position;

    public bool cutin;
    public bool summon;
    public float landTime;
    public string type;

    public gameCard card;

    Animation ani_;
    void Start()
    {
        cardfaceMaterial = transform.Find("card/face").GetComponent<Renderer>().material;
        ani_ = transform.Find("card").GetComponent<Animation>();
    }

    IEnumerator DelayActivateObjectWithSound(GameObject go, float delayTime, string name)
    {
        yield return new WaitForSeconds(delayTime);
        go.SetActive(true);
        UIHelper.playSound(name, 1);
    }
    IEnumerator DelayActivateQuad(float delayTime)
    {
        yield return new WaitForSeconds(0.1f);
        if (transform.Find("mod_simple_quad(Clone)") != null)
        {
            CloseUpControl cc = transform.Find("mod_simple_quad(Clone)").GetChild(0).GetComponent<CloseUpControl>();
            cc.FadeOut(0f);
        }
        yield return new WaitForSeconds(delayTime - 0.1f);
        cardfaceMaterial.renderQueue = 3000;
        BlackBehaviour.FadeOut(0.167f);
        if (transform.Find("mod_simple_quad(Clone)") != null)
        {
            CloseUpControl cc = transform.Find("mod_simple_quad(Clone)").GetChild(0).GetComponent<CloseUpControl>();
            cc.FadeIn(0.25f);
        }
    }

    public void PlayDelayActivateQuad(float delayTime)
    {
        StartCoroutine(DelayActivateQuad(delayTime));
    }
    public void PlayDelayedAnimation(string name, float time)
    {
        StartCoroutine(DelayedAnimation(name, time));
    }

    IEnumerator DelayedAnimation(string name, float time)
    {
        yield return new WaitForSeconds(time);
        if (name == "negate")
            PlayNegateAnimation();
        else if (name == "activate")
            PlayActivateAnimation();
        else
            ani_.Play(name);
    }

    void PlayNegateAnimation()
    {
        var negateEff = ABLoader.LoadABFromFile("effects/buff/fxp_bff_disable_001");
        negateEff.transform.parent = ani_.transform;
        negateEff.transform.localPosition = new Vector3(0f, 0f, -0.2f);
        negateEff.transform.localEulerAngles = new Vector3(90, 0, 0);
        negateEff.transform.localScale = Vector3.one;
        negateEff.SetActive(false);
        StartCoroutine(DelayActivateObjectWithSound(negateEff, 0.167f, "SE_DUEL/SE_EFFECT_INVALID"));
        if ((card.p.location & (uint)CardLocation.Grave) >0 || (card.p.location & (uint)CardLocation.Removed) > 0)
            ani_.Play("card_bff_disable_grave_exclude");
        else
            ani_.Play("card_bff_disable");
        Destroy(negateEff, 1f);
        if (transform.Find("mod_simple_quad(Clone)") != null)
        {
            CloseUpControl cc = transform.Find("mod_simple_quad(Clone)").GetChild(0).GetComponent<CloseUpControl>();
            cc.FadeOut(0.167f);
        }
        StartCoroutine(DelayActivateQuad(0.5f + 0.167f));
    }
    void PlayActivateAnimation()
    {
        var activateEff = ABLoader.LoadABFromFile("effects/buff/fxp_bff_active_001");
        foreach (var renderer in activateEff.transform.GetComponentsInChildren<Renderer>())
        {
            renderer.sharedMaterial.renderQueue = 4004;
        }
        activateEff.transform.parent = ani_.transform;
        activateEff.transform.localPosition = new Vector3(0f, 0f, -0.2f);
        activateEff.transform.localEulerAngles = new Vector3(90, 0, 0);
        activateEff.transform.localScale = Vector3.one;
        activateEff.SetActive(false);
        StartCoroutine(DelayActivateObjectWithSound(activateEff, 0.167f, "SE_DUEL/SE_CARDVIEW_01"));
        if ((card.p.location & (uint)CardLocation.Hand) > 0)
            ani_.Play("card_bff_activate_hand");
        else if ((card.p.location & (uint)CardLocation.Grave) > 0 || (card.p.location & (uint)CardLocation.Removed) > 0)
            ani_.Play("card_bff_activate_grave_exclude");
        else
            ani_.Play("card_bff_activate");
        Destroy(activateEff, 1f);

        cardfaceMaterial.renderQueue = 4004;
        BlackBehaviour.FadeIn(0.167f);
        if (transform.Find("mod_simple_quad(Clone)") != null)
        {
            CloseUpControl cc = transform.Find("mod_simple_quad(Clone)").GetChild(0).GetComponent<CloseUpControl>();
            cc.FadeOut(0.167f);
        }
        StartCoroutine(DelayActivateQuad(0.75f + 0.167f));
    }
}
