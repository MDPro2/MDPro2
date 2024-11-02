using UnityEngine;
using Spine.Unity;
using DG.Tweening;

public class P63288574_Handler : MonoBehaviour
{
    SkeletonGraphic sg;
    public UIClickHandler interPart;
    public Material mat2;
    public Material mat3;

    float waitTime;
    float time;
    bool playing;
    private void Start()
    {
        sg = GetComponent<SkeletonGraphic>();
        interPart.action = InterClick;
        waitTime = 1;
        Idle();
    }

    void Update()
    {
        time += Time.deltaTime;
        if (playing && time > 0.3f)
        {
            playing = false;
        }
        if (time > waitTime && playing == false)
        {
            time = 0;
            waitTime = Random.Range(2, 5);
            sg.AnimationState.SetAnimation(2, "blink", false);
        }
    }

    bool wingClosed;
    public void InterClick()
    {
        if (playing)
            return;
        if (wingClosed)
        {
            sg.AnimationState.SetAnimation(1, "recover", false);
            mat2.DOFloat(1f, "Vector1_5cc3d821a68e4acb9ac135067244a058", 0.5f);
            mat3.DOFloat(1f, "Vector1_5cc3d821a68e4acb9ac135067244a058", 0.5f);
            OpenWing(/*0.25f*/);
        }
        else
        {
            sg.AnimationState.SetEmptyAnimation(2, 0.2f);
            sg.AnimationState.SetAnimation(1, "interaction", false);
            mat2.DOFloat(0f, "Vector1_5cc3d821a68e4acb9ac135067244a058", 0.15f);
            mat3.DOFloat(0f, "Vector1_5cc3d821a68e4acb9ac135067244a058", 0.15f);
        }
        wingClosed = !wingClosed;
        playing = true;
        time = 0;
    }

    bool closed = false;
    public void WingClick()
    {
        if (closed)
        {
            OpenWing();
        }
        else
        {
            CloseWing();
        }
    }

    void CloseWing()
    {
        sg.AnimationState.SetAnimation(3, "closeLeftA", false);
        sg.AnimationState.SetAnimation(4, "closeLeftB", false);
        sg.AnimationState.SetAnimation(5, "closeLeftC", false);
        sg.AnimationState.SetAnimation(6, "closeLeftD", false);
        sg.AnimationState.SetAnimation(7, "closeRightA", false);
        sg.AnimationState.SetAnimation(8, "closeRightB", false);
        sg.AnimationState.SetAnimation(9, "closeRightC", false);
        sg.AnimationState.SetAnimation(10, "closeRightD", false);
        closed = true;
    }

    void OpenWing(float delay = 0f)
    {
        DOTween.To(v => { }, 0, 0, delay).OnComplete(() =>
        {
            for (int i = 3; i < 11; i++)
                sg.AnimationState.SetEmptyAnimation(i, 0.16f);
            closed = false;
        });
    }


    void Idle()
    {
        sg.AnimationState.SetAnimation(0, "idle", true);
    }
}
