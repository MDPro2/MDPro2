using UnityEngine;
using DG.Tweening;

public class BackgroundControl : MonoBehaviour
{
    static GameObject background;
    private void OnEnable()
    {
        background = ABLoader.LoadABFromFile("wallpaper/back/back0007", transform);
        ABLoader.ChangeLayer(background, "UI_background");
        background.AddMissingComponent<AutoScale>();
    }

    public static void Hide()
    {
        background.SetActive(false);
    }

    public static void Show()
    {
        background.SetActive(true);
    }
}
