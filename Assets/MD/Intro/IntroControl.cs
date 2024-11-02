using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;




public class IntroControl : MonoBehaviour
{
    float time;
    bool yes = true;
    bool yes2 = true;
    bool yes3 = true;
    bool yes4 = true;
    bool yes5 = true;
    bool yes6 = true;
    bool yes7 = true;
    bool yes8 = true;
    bool yes9 = true;
    Transform duelEntry;
    Transform BK_hole;
    Transform mate1;
    Transform mate2;
    Transform nameFrame1;
    Transform nameFrame2;
    Transform vs;

    void Ins()
    {
        duelEntry = GameObject.Find("DuelEntry(Clone)").transform;
        BK_hole = duelEntry.Find("BK_hole").transform;
        BK_hole.localScale = new Vector3(30, 30, 1);
        duelEntry.gameObject.SetActive(false);
        duelEntry.gameObject.SetActive(true);
        
        mate1 = GameObject.Find("M07701_Model(Clone)").transform;
        mate2 = GameObject.Find("M12950_Model(Clone)").transform;
        foreach(Transform t in mate1.gameObject.GetComponentsInChildren<Transform>(true))
        {
            t.gameObject.layer = LayerMask.NameToLayer("layer_12");
        }
        foreach (Transform t in mate2.gameObject.GetComponentsInChildren<Transform>(true))
        {
            t.gameObject.layer = LayerMask.NameToLayer("layer_12");
        }

        mate1.eulerAngles = new Vector3(0, 160, 0);
        mate2.eulerAngles = new Vector3(0, -160, 0);
        mate1.position = new Vector3(-45, -10, -65);
        mate2.position = new Vector3(45, -10, -65);
        mate1.GetChild(0).gameObject.AddComponent<EventSEPlayer>();
        mate2.GetChild(0).gameObject.AddComponent<EventSEPlayer>();

        nameFrame1 = GameObject.Find("NameFrame1").transform;
        nameFrame2 = GameObject.Find("NameFrame2").transform;
        nameFrame1.position = new Vector3(-45, -15, -65);
        nameFrame2.position = new Vector3(45, -15, -65);

        vs = GameObject.Find("VS").transform;
        vs.gameObject.SetActive(false);
    }
    void Ins2()
    {
        mate1.DOMoveX(mate1.position.x + 20, 0.5f).SetEase(Ease.OutCubic);
        mate2.DOMoveX(mate2.position.x - 20, 0.5f).SetEase(Ease.OutCubic);
        nameFrame1.DOMoveX(nameFrame1.position.x + 20, 0.5f).SetEase(Ease.OutCubic);
        nameFrame2.DOMoveX(nameFrame2.position.x - 20, 0.5f).SetEase(Ease.OutCubic);
    }
    void Ins3()
    {
        mate1.DOMoveX(mate1.position.x - 20, 0.25f).SetEase(Ease.InCubic);
        mate2.DOMoveX(mate2.position.x + 20, 0.25f).SetEase(Ease.InCubic);
        nameFrame1.DOMoveX(nameFrame1.position.x - 20, 0.5f).SetEase(Ease.OutCubic);
        nameFrame2.DOMoveX(nameFrame2.position.x + 20, 0.5f).SetEase(Ease.OutCubic);
        vs.DOScale(0,0.5f);
        vs.GetComponent<SpriteRenderer>().DOFade(0, 0.5f);
    }
    void Ins4()
    {
        AnimationControl.PlayAnimation(mate1, "Matching");
        AnimationControl.PlayAnimation(mate2, "Matching");
        //foreach (Transform t in mate1.gameObject.GetComponentsInChildren<Transform>(true))
        //{
        //    t.gameObject.layer = LayerMask.NameToLayer("layer_12");
        //}
        //foreach (Transform t in mate2.gameObject.GetComponentsInChildren<Transform>(true))
        //{
        //    t.gameObject.layer = LayerMask.NameToLayer("layer_12");
        //}
    }

    void Ins5()
    {
        vs.gameObject.SetActive(true);
        vs.DOScale(new Vector3(0.15f,0.15f,1),0.16f);
        vs.GetComponent<SpriteRenderer>().DOFade(0.8f, 0.16f);
    }

    void Update()
    {
        time += Time.deltaTime;
        if(time > 2 && yes)
        {
            Ins();
            yes = false;
        }
        if (time > 2.5f && yes2)
        {
            Ins2();
            yes2 = false;
        }
        if (time > 8f && yes3)
        {
            Ins3();
            yes3 = false;
        }
        if (time > 5f && yes4)
        {
            Ins4();
            yes4 = false;
        }
        if (time > 0.16f && yes4)
        {
            Ins5();
            yes5 = false;
        }
        //Debug.Log(time);
    }
}
