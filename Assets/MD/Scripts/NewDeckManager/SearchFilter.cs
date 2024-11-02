using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class SearchFilter : MonoBehaviour
{
    public UIPanel mainWindow_;
    public UIPanel sortWindow_;
    public UI2DSprite shadow_;
    public UIButton confirm_;
    public UIButton cancel_;
    public UIButton clean_;
    public UIScrollBar bar_;
    public bool isShowed;
    public bool sortIsShowed;

    public List<FilterCategory> filterCategories;

    public UIInput lv_from;
    public UIInput lv_to;
    public UIInput atk_from;
    public UIInput atk_to;
    public UIInput def_from;
    public UIInput def_to;
    public UIInput scale_from;
    public UIInput scale_to;
    public UIInput year_from;
    public UIInput year_to;
    List<UIInput> inputs;

    public SortToggle type_up;
    public SortToggle type_down;
    public SortToggle lv_up;
    public SortToggle lv_down;
    public SortToggle atk_up;
    public SortToggle atk_down;
    public SortToggle def_up;
    public SortToggle def_down;
    List<SortToggle> toggles;
    public UIButton sortCancel_;

    public List<List<string>> settings;

    public enum SortOrder
    {
        type_up,
        type_down,
        lv_up,
        lv_down,
        atk_up,
        atk_down,
        def_up,
        def_down
    }
    private void Start()
    {
        shadow_.alpha = 0;
        mainWindow_.alpha = 1;
        mainWindow_.transform.localPosition = new Vector3(0, -1100, 0);
        sortWindow_.alpha = 1;
        sortWindow_.transform.localPosition = new Vector3(0, -1100, 0);
        EventDelegate.Add(confirm_.onClick, Save);
        EventDelegate.Add(cancel_.onClick, Hide);
        EventDelegate.Add(clean_.onClick, ResetFilter);
        EventDelegate.Add(sortCancel_.onClick, HideSort);
        foreach (FilterCategory category in filterCategories)
            foreach(var toggle in category.toggles)
                toggle.Install();
        inputs = new List<UIInput>
        {
            lv_from,
            lv_to,
            atk_from,
            atk_to,
            def_from,
            def_to,
            scale_from,
            scale_to,
            year_from,
            year_to
        };
        toggles = new List<SortToggle>
        {
            type_up,
            type_down,
            lv_up,
            lv_down,
            atk_up,
            atk_down,
            def_up,
            def_down
        };
        foreach (var toggle in toggles)
            toggle.Install();
        type_up.Preselect();
        gameObject.SetActive(false);
    }

    public void Show()
    {
        isShowed = true;
        SEHandler.PlayInternalAudio("se_sys/SE_MENU_SLIDE_03");
        mainWindow_.transform.DOLocalMove(Vector3.zero, 0.3f);
        DOTween.To(() => shadow_.alpha, x => shadow_.alpha = x, 0.8f, 0.3f);
        bar_.value = 0;
        if (settings == null)
            ResetFilter();
        else
        {
            for (int i = 0; i < settings.Count - 1; i++)
                for (int j = 0; j < settings[i].Count; j++)
                {
                    if (settings[i][j] == "")
                        filterCategories[i].toggles[j].selected = true;
                    else
                        filterCategories[i].toggles[j].selected = false;
                    filterCategories[i].toggles[j].Switch();
                }
        }
        if (Program.I().setting.setting.confirmLeft.value)
        {
            confirm_.transform.localPosition = new Vector3(-200, -470, 0);
            cancel_.transform.localPosition = new Vector3(200, -470, 0);
        }
        else
        {
            confirm_.transform.localPosition = new Vector3(200, -470, 0);
            cancel_.transform.localPosition = new Vector3(-200, -470, 0);
        }
    }
    public void ShowSort()
    {
        sortIsShowed = true;
        SEHandler.PlayInternalAudio("se_sys/SE_MENU_SLIDE_03");
        sortWindow_.transform.DOLocalMove(Vector3.zero, 0.3f);
        DOTween.To(() => shadow_.alpha, x => shadow_.alpha = x, 0.8f, 0.3f);
    }

    public void Save()
    {
        bool empty = true;
        foreach (FilterCategory category in filterCategories)
        {
            if (empty == false) break;
            foreach (var toggle in category.toggles)
            {
                if (toggle.selected)
                {
                    empty = false;
                    break;
                }
            }
        }
        foreach (var input in inputs)
        {
            if(input.value != "")
                empty = false;
        }

        if (empty)
            settings = null;
        else
        {
            settings = new List<List<string>>();
            for (int i = 0; i < filterCategories.Count; i++)
            {
                List<string> toggles = new List<string>();
                for (int j = 0; j < filterCategories[i].toggles.Count; j++)
                {
                    if (filterCategories[i].toggles[j].selected)
                        toggles.Add(filterCategories[i].toggles[j].label_.text);
                    else
                        toggles.Add("");
                }
                settings.Add(toggles);
            }
            List<string> values = new List<string>();
            foreach (var input in inputs)
                values.Add(input.value);
            settings.Add(values);
        }
        Program.I().newDeckManager.DoSearch();
        Hide();
    }

    public void Hide()
    {
        isShowed = false;
        SEHandler.PlayInternalAudio("se_sys/SE_MENU_SLIDE_04");
        DOTween.To(() => shadow_.alpha, x => shadow_.alpha = x, 0, 0.3f);
        mainWindow_.transform.DOLocalMove(new Vector3(0, -1100, 0), 0.3f).OnComplete(() => 
        {
            gameObject.SetActive(false);
        });
        if (settings != null)
            Program.I().newDeckManager.FilterMaker(true);
        else
            Program.I().newDeckManager.FilterMaker(false);
    }

    public void HideSort()
    {
        sortIsShowed = false;
        SEHandler.PlayInternalAudio("se_sys/SE_MENU_SLIDE_04");
        DOTween.To(() => shadow_.alpha, x => shadow_.alpha = x, 0, 0.3f);
        sortWindow_.transform.DOLocalMove(new Vector3(0, -1100, 0), 0.3f).OnComplete(() =>
        {
            gameObject.SetActive(false);
        });
    }

    public void ResetFilter()
    {
        foreach (var fc in filterCategories)
            foreach (var toggle in fc.toggles)
            {
                toggle.selected = true;
                toggle.Switch();
            }
        foreach (var input in inputs)
            input.value = "";
    }
    public void ResetSetting()
    {
        settings = null;
    }

    public void ResetSort(SortToggle toggle)
    {
        foreach(var t in toggles)
            if (t != toggle)
                t.Reset();
    }

    public SortOrder GetSortOrder()
    {
        if (type_up.selected)
            return SortOrder.type_up;
        if (type_down.selected)
            return SortOrder.type_down;
        if(lv_up.selected)
            return SortOrder.lv_up;
        if(lv_down.selected)
            return SortOrder.lv_down;
        if(atk_up.selected)
            return SortOrder.atk_up;
        if(atk_down.selected)
            return SortOrder.atk_down;
        if(def_up.selected)
            return SortOrder.def_up;
        if(def_down.selected)
            return SortOrder.def_down;
        return SortOrder.type_up;
    }
}
