using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkBehaviour : MonoBehaviour
{
    int linkValue = 1;
    public Sprite link1;
    public Sprite link2;
    public Sprite link3;
    public Sprite link4;
    public Sprite link5;
    public Sprite link6;
    // Start is called before the first frame update
    void Start()
    {
        
        if (this.transform.parent.parent.GetComponent<TextBehaviour>().type == "link")
        {
            this.gameObject.SetActive(true);
            linkValue = this.transform.parent.parent.GetComponent<TextBehaviour>().level;
            Sprite sprite = link1;
            switch (linkValue)
            {
                case 1: sprite = link1;
                    break;
                case 2: sprite = link2;
                    break;
                case 3: sprite = link3;
                    break;
                case 4: sprite = link4;
                    break;
                case 5: sprite = link5;
                    break;
                case 6: sprite = link6;
                    break;
                default: sprite = link1; 
                    break;
            }
            this.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().sprite = sprite;
        }
        else  this.gameObject.SetActive(false);
    }
}
