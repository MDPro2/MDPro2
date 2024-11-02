using DG.Tweening;
using Spine.Unity;
using System;
using System.IO;
using UnityEngine;

public class barPngLoader : MonoBehaviour
{
    public UILabel api_name;
    public UILabel api_healthHint;
    public UI2DSprite api_face;
    public UI2DSprite api_frame;
    public SpineCharacterHandler spine;
    public bool dynamic;

    int lp;
    float flp;
    public void SetLP(int LP)
    {
        if(lp != LP)
        {
            flp = lp;
            DOTween.To(() => flp, x => flp = x, LP, 1.2f);
            Color color = Color.red;
            if (name.StartsWith("new_mod_healthBar_me"))
            {
                if (LP > lp)
                {
                    color = Color.green;
                    UIHelper.playSound("SE_DUEL/SE_LP_RECOVERY_PLAYER", 1);
                }
                else
                    UIHelper.playSound("SE_DUEL/SE_LP_COUNT_PLAYER", 1);
            }
            else
            {
                if (LP > lp)
                {
                    color = Color.green;
                    UIHelper.playSound("SE_DUEL/SE_LP_RECOVERY_RIVAL", 1);
                }
                else
                    UIHelper.playSound("SE_DUEL/SE_LP_COUNT_RIVAL", 1);
            }

            Sequence quence = DOTween.Sequence();
            quence.Append(api_healthHint.transform.DOScale(1.2f, 0.2f).OnComplete(()=> api_healthHint.color = color));
            quence.AppendInterval(0.8f);
            quence.Append(api_healthHint.transform.DOScale(1f, 0.2f));
            quence.AppendCallback(() => api_healthHint.color = Color.white);

            lp = LP;
        }
    }

    private void Update()
    {
        api_healthHint.text = ((int)flp).ToString();
    }

}