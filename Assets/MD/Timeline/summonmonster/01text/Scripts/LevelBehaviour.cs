using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBehaviour : MonoBehaviour
{
    int levelValue = 1;
    void Start()
    {
        if (this.transform.parent.parent.GetComponent<TextBehaviour>().type == "level")
        {
            levelValue = this.transform.parent.parent.GetComponent<TextBehaviour>().level;
            List<GameObject> gos = new List<GameObject>();
            foreach(Transform child in this.transform.GetChild(0).GetComponentsInChildren<Transform>())
            {
                if(child.name != "Icon_Level")
                {
                    gos.Add(child.gameObject);
                }
            }
            foreach(GameObject child in gos)
            {
                if(int.Parse(child.name.Substring(child.name.Length - 2)) > levelValue)
                {
                    child.SetActive(false);
                }
            }
        }
        else this.gameObject.SetActive(false);
    }
}
