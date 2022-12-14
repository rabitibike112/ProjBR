using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiControls : MonoBehaviour
{
    //scale Settings
    private GameObject Panel;
    private Button CrestereUnif, CrestereNeUnif;
    private Toggle UnifTogg, NeUnifTogg;
    public InputField UnifScale, NeUnifScalePX, NeUnifScaleMX, NeUnifScalePY, NeUnifScaleMY;
    public bool Uniform = true;
    //Concavity Settings
    private Button DoarConvex, SiConcav;
    private Toggle DoarConvexTogg, SiConcavTogg;
    public bool bDoarConvex = true;
    //Corner Settings
    private Button ColtRot, ColtDrept, ColtAscutit;
    private Toggle ColtRotTogg, ColtDreptTogg, ColtAscutitTogg;
    public int Care=1;
    //Other Buttons
    private Button StartDraw;
    private Button StartGrow;

    void Start()
    {
        Links.MouseFollower = GameObject.Find("MouseFollower").gameObject;
        Links.UiControls_S = transform.GetComponent<UiControls>();
        
        Panel = transform.Find("Panel").gameObject;
        Links.CrestereUniform_Inp = Panel.transform.Find("Scale").GetComponent<InputField>();
        AssignScaleUi();
        AssignConcavityUi();
        AssignCornerUi();
        AssignOtherButtonsUi();
    }
    private void AssignScaleUi()
    {
        CrestereUnif = Panel.transform.Find("Uniform").GetComponent<Button>();
        CrestereNeUnif = Panel.transform.Find("Neuniform").GetComponent<Button>();
        UnifTogg = Panel.transform.Find("UniformT").GetComponent<Toggle>();
        NeUnifTogg = Panel.transform.Find("NeuniformT").GetComponent<Toggle>();
        UnifScale = Panel.transform.Find("Scale").GetComponent<InputField>();
        NeUnifScalePX = Panel.transform.Find("InpX+").GetComponent<InputField>();
        NeUnifScaleMX = Panel.transform.Find("InpX-").GetComponent<InputField>();
        NeUnifScalePY = Panel.transform.Find("InpY+").GetComponent<InputField>();
        NeUnifScaleMY = Panel.transform.Find("InpY-").GetComponent<InputField>();
        CrestereUnif.onClick.AddListener(ButCrestereUniforma);
        CrestereNeUnif.onClick.AddListener(ButCrestereNeUniforma);
    }
    private void AssignConcavityUi()
    {
        DoarConvex = Panel.transform.Find("DoarConvex").GetComponent<Button>();
        SiConcav = Panel.transform.Find("SiConcav").GetComponent<Button>();
        DoarConvexTogg = Panel.transform.Find("DoarConvexT").GetComponent<Toggle>();
        SiConcavTogg = Panel.transform.Find("SiConcavT").GetComponent<Toggle>();
        DoarConvex.onClick.AddListener(ButDoarConvex);
        SiConcav.onClick.AddListener(ButSiConcave);
    }

    private void AssignCornerUi()
    {
        ColtRot = Panel.transform.Find("ColtRotund").GetComponent<Button>();
        ColtDrept = Panel.transform.Find("ColtDrept").GetComponent<Button>();
        ColtAscutit = Panel.transform.Find("PasForma").GetComponent<Button>();
        ColtRotTogg = Panel.transform.Find("ColtRotundT").GetComponent<Toggle>();
        ColtDreptTogg = Panel.transform.Find("ColtDreptT").GetComponent<Toggle>();
        ColtAscutitTogg = Panel.transform.Find("PasFormaT").GetComponent<Toggle>();
        ColtRot.onClick.AddListener(ButColtRotund);
        ColtDrept.onClick.AddListener(ButColtDrept);
        ColtAscutit.onClick.AddListener(ButColtAscutit);
    }
    private void AssignOtherButtonsUi()
    {
        StartDraw = Panel.transform.Find("Desenare").GetComponent<Button>();
        StartDraw.onClick.AddListener(ButStartDraw);
        StartGrow = Panel.transform.Find("Expandare").GetComponent<Button>();
        StartGrow.onClick.AddListener(ButStartGrow);
    }
    private void ButCrestereUniforma()
    {
        Uniform = true;
        UnifTogg.isOn = true;
        NeUnifTogg.isOn = false;
        NeUnifScalePX.interactable = false;
        NeUnifScaleMX.interactable = false;
        NeUnifScalePY.interactable = false;
        NeUnifScaleMY.interactable = false;
        UnifScale.interactable = true;
    }
    private void ButCrestereNeUniforma()
    {
        Uniform = false;
        UnifTogg.isOn = false;
        NeUnifTogg.isOn = true;
        NeUnifScalePX.interactable = true;
        NeUnifScaleMX.interactable = true;
        NeUnifScalePY.interactable = true;
        NeUnifScaleMY.interactable = true;
        UnifScale.interactable = false;
    }
    private void ButDoarConvex()
    {
        bDoarConvex = true;
        DoarConvexTogg.isOn = true;
        SiConcavTogg.isOn = false;
    }
    private void ButSiConcave()
    {
        bDoarConvex = false;
        DoarConvexTogg.isOn = false;
        SiConcavTogg.isOn = true;
    }

    private void ButColtAscutit()
    {
        Care = 1;
        ColtAscutitTogg.isOn = true;
        ColtDreptTogg.isOn = false;
        ColtRotTogg.isOn = false;
    }

    private void ButColtDrept()
    {
        Care = 2;
        ColtAscutitTogg.isOn = false;
        ColtDreptTogg.isOn = true;
        ColtRotTogg.isOn = false;
    }
    private void ButColtRotund()
    {
        Care = 3;
        ColtAscutitTogg.isOn = false;
        ColtDreptTogg.isOn = false;
        ColtRotTogg.isOn = true;
    }

    private void ButStartDraw()
    {
        Links.MouseFollower.GetComponent<ShapeCreator>().StartBuild();
    }
    private void ButStartGrow()
    {
        GameObject Container = GameObject.Find("ToGrow").gameObject;
        if (Container.transform.childCount > 0)
        {
            for(int x1 = 0; x1 < Container.transform.childCount; x1++)
            {
                Container.transform.GetChild(x1).GetComponent<Grow>().GrowObject();
            }
        }
    }
}

public static class Links
{
    public static GameObject MouseFollower { get; set; }
    public static UiControls UiControls_S { get; set; }
    public static InputField CrestereUniform_Inp { get; set; }
}