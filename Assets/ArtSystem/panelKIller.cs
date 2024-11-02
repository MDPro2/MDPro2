using UnityEngine;

public class panelKIller : MonoBehaviour
{
    private bool on;

    private UIPanel pan;

    UIWidget mainWindow;
    private void Start()
    {
        if(name.StartsWith("trans_room"))
        mainWindow = UIHelper.getByName<UIWidget>(gameObject, "mainWindow");
    }

    // Update is called once per frame
    private void Update()
    {
        if (pan != null)
        {
            float to = 0;
            if (on) to = 1;
            if (Mathf.Abs(to - pan.alpha) > 0.1f)
            {
                pan.alpha += (to - pan.alpha) * Program.deltaTime * 18;
            }
            else
            {
                if (pan.alpha != to) pan.alpha = to;
            }
        }
        if (mainWindow != null)
            mainWindow.width = Utils.UIWidth() + 4;
    }

    public void ini()
    {
        pan = GetComponentInChildren<UIPanel>();
    }

    public void set(bool r)
    {
        on = r;
        if (pan != null)
            if (r)
                pan.alpha = 0;
    }
}