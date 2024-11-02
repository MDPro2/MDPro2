using UnityEngine;
using System.Collections;

public class placeSelector : MonoBehaviour {
    public Transform flag;
    public Collider col;
    public byte[] data;
    public bool selected = false;
    public Transform quad;

    public GameObject selectedGO;
    GameObject selectableGO;

    // Use this for initialization
    void Start () 
    {
        if(transform.position.z == -9.48 || transform.position.z == 9.51)//主怪兽区
            selectableGO = ABLoader.LoadABFromFile("effects/hitghlight/fxp_hl_select/fxp_hl_select_mst_001");
        else if(transform.position.z == -18 || transform.position.z == 18)//魔陷区
        {
            selectableGO = ABLoader.LoadABFromFile("effects/hitghlight/fxp_hl_select/fxp_hl_select_trpmgc_001");
        }
        else if (transform.position.z == -10 || transform.position.z == 10)//场地
        {
            selectableGO = ABLoader.LoadABFromFile("effects/hitghlight/fxp_hl_select/fxp_hl_select_card_001");
            selectableGO.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
        }
        else//额外怪兽区
            selectableGO = ABLoader.LoadABFromFile("effects/hitghlight/fxp_hl_select/fxp_hl_select_mst_001");
        var main = selectableGO.GetComponent<ParticleSystem>().main;
        main.playOnAwake = true;
        selectableGO.transform.parent = transform;
        selectableGO.transform.localPosition = Vector3.zero;
        selectableGO.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
        if (Program.pointedCollider == col)
        {
            if (flag.gameObject.activeInHierarchy == false)
            {
                flag.gameObject.SetActive(true);
            }
            Vector3 worldposition = Input.mousePosition;
            worldposition.z = 18;
            flag.transform.position = Camera.main.ScreenToWorldPoint(worldposition) - new Vector3(0, 0, 1);
            if (Program.InputGetMouseButtonDown_0)
            {
                Program.I().ocgcore.ES_placeSelected(this);
            }
        }
        else
        {
            if (flag.gameObject.activeInHierarchy == true)
            {
                flag.gameObject.SetActive(false);
            }
        }
        if (selectableGO!=null)
        {
            selectableGO.SetActive(!selected);
        }
        if (selectedGO != null)
        {
            selectedGO.SetActive(selected);
        }
    }
}
