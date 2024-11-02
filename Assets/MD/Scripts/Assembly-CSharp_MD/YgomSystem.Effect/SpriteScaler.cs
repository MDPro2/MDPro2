using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YgomSystem.Effect
{
    public class SpriteScaler : MonoBehaviour
    {
        private void Update()
        {
            if(name == "Black" && transform.parent.name == "Ef04342(Clone)")
            {
                transform.localScale = new Vector3(9000, 540, 1);
            }
            else if (name == "Black" && transform.parent.name.StartsWith("Ef04678"))
            {
                transform.localScale = new Vector3(50000, 10000, 1);
                transform.localPosition = new Vector3(0, 80, -10);
            }
            else if (name == "BK" && transform.parent.name.StartsWith("Ef05124"))
            {
                transform.localScale = new Vector3(50000, 7000, 1);
            }
        }
    }
}

