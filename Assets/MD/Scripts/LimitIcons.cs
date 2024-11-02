using UnityEngine;

public class LimitIcons : MonoBehaviour
{
    public Sprite null_;
    public Sprite ban;
    public Sprite limit1;
    public Sprite limit2;

    public void ChangeIcon(int i)
    {
        switch (i)
        {
            case 0:
                GetComponent<UI2DSprite>().sprite2D = ban;
                break;
            case 1:
                GetComponent<UI2DSprite>().sprite2D = limit1;
                break;
            case 2:
                GetComponent<UI2DSprite>().sprite2D = limit2;
                break;
            case 3:
                GetComponent<UI2DSprite>().sprite2D = null_;
                break;
        }
    }
}
