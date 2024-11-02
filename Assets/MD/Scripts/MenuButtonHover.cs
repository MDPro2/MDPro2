using DG.Tweening;
using UnityEngine;

public class MenuButtonHover : MonoBehaviour
{
    Transform label;
    Transform label2;
    Transform description;
    Transform accordion;
    Collider collider_;
    bool hover;
    void Start()
    {
        label = transform.Find("!label");
        label2 = transform.Find("!label2");
        description = transform.Find("!description");
        accordion = transform.Find("!accordion");
        collider_ = GetComponent<Collider>();
    }

    void Update()
    {
        if (Program.pointedCollider == collider_ && !hover)
        {
            description.DOLocalMoveX(500, 0.15f);
            label.gameObject.SetActive(false);
            label2.gameObject.SetActive(true);
            accordion.localScale = new Vector3(1f, 2f, 1f);
            hover = true;
        }
        else if(Program.pointedCollider != collider_ && hover)
        {
            description.DOLocalMoveX(0, 0.15f);
            label.gameObject.SetActive(true);
            label2.gameObject.SetActive(false);
            accordion.localScale = new Vector3(0.5f, 1f, 1f);
            hover = false;
        }
    }
}
