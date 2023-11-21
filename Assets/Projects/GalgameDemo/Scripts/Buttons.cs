using Cysharp.Threading.Tasks.Triggers;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;



public class Buttons : MonoBehaviour {
    public GameObject AutoPlayingSignal;//自动播放标志
    public GameObject SaveProfliePanel,LoadProfliePanel;
    public GameObject SettingsPanel,AboutPanel,MainMenuPanel,AboutGame;
    public GameObject TestRead;//剧本控制的脚本所挂载在的物体
    public Toggle VSynctoggle;

    public GameObject qualityManager;
    // Use this for initialization

    private void AddVSynctoggleListener()
    {

        Toggle VSynctoggle = GameObject.Find("VsyncSettings").GetComponent<Toggle>();
        VSynctoggle.onValueChanged.AddListener(OnVSynctoggleToggleValueChanged);
    }
    private void Awake()
    {
        SaveProfliePanel = GameObject.Find("SaveProfliePanel");
        LoadProfliePanel = GameObject.Find("LoadProfliePanel");
        qualityManager = GameObject.Find("QualityManager");
    }


    public void OnVSynctoggleToggleValueChanged(bool isOn)
    {
        if(isOn)
        {
            qualityManager.SendMessage("EnableVSync");

        }
        if(isOn == false) 
        {
            qualityManager.SendMessage("DisableVSync");
            
        }
    }
    private void LoadText()
    {
        GameObject tempTestRead = Instantiate(TestRead, Vector3.zero, transform.rotation);//生成新的TestRead来控制脚本读取剧本
    }

    private void SaveProfileGrid()
    {
        GameObject SceneBgPicture = GameObject.Find("SceneBg");
        GameObject ProfileBgPicture = transform.Find("Bg").transform.Find("ProfilePicture").gameObject;
        GameObject PersonPicture = transform.Find("Bg").transform.Find("PersonPictureMask").transform.Find("PersonPicture").gameObject;
        GameObject Time = transform.Find("Bg").transform.Find("Information").transform.Find("Time").gameObject;
        GameObject TreatmentName = transform.Find("Bg").transform.Find("Information").transform.Find("TreatmentName").gameObject;
        GameObject Treatment = transform.Find("Bg").transform.Find("Information").transform.Find("Treatment").gameObject;
        //其实这上面完全可以拖。。。

        //然后对存档格的文字和图片进行相应的更改
        ProfileBgPicture.GetComponent<Image>().sprite = SceneBgPicture.GetComponent<Image>().sprite;
        PersonPicture.GetComponent<Image>().sprite = GameObject.Find("CenterPerson").GetComponent<Image>().sprite;
        Time.GetComponent<Text>().text = DateTime.Now.ToString("yyyy-MM-dd   ") + DateTime.Now.ToShortTimeString();
        TreatmentName.GetComponent<Text>().text = TreatmentController._instance.treatmentName;
        Treatment.GetComponent<Text>().text = "【" + TreatmentController._instance.currentPersonName + "】 " + TreatmentController._instance.currentTreatmentText;

        //进行存档，存档名为当前存档格在父物体中作为子物体的下标，也就是第几个，从0开始数
        GameController._instance.SaveData(transform.GetSiblingIndex().ToString());
    }

    private void LoadProfileGrid()
    {
        //读取存档
        GameController._instance.LoadData(transform.GetSiblingIndex().ToString());
    }
    private void DialougueEmpty()
    {
        string dialogText = transform.Find("LogBg").transform.Find("Text").GetComponent<Text>().text;
        dialogText = dialogText.Substring(dialogText.LastIndexOf('[') + 1, dialogText.LastIndexOf(']') - dialogText.LastIndexOf('[') - 1);
        Debug.Log("临时文本:" + dialogText);
        int tempIndex = TreatmentController._instance.FindIndexByText(dialogText);//通过当前log的文本来寻找index
        Debug.Log("当前log对应行数为:" + tempIndex);
        TreatmentController._instance.ReadTreatment(TreatmentController._instance.treatmentName, tempIndex);
        UIController._instance.OpenLogs();

    }

    IEnumerator LoadingUICoroutine()
    {
        //打印函数第一次调用的时间。
        Debug.Log("Started Coroutine at timestamp : " + Time.time);

        //生成一个等待5秒的yield指令。
        yield return new WaitForSeconds(UIController._instance.Loadsecond);

        transform.parent.gameObject.SetActive(false);

        LoadText();

        //等待5秒后再次打印时间。
        Debug.Log("Finished Coroutine at timestamp : " + Time.time);
    }

    public void OnClickButton()
    {
        Debug.Log("点击了" + gameObject.name);
        switch (gameObject.name)
        {
            case "StartGame":
                //点击开始游戏
                UIController._instance.ActiveLoadingWindow();

                //启动我们在下面定义的名为 LoadingUICoroutine 的协程。
                StartCoroutine(LoadingUICoroutine());

                break;
            case "Continue":
                //继续游戏
                UIController._instance.ActiveLoadingWindow();

                //启动我们在下面定义的名为 LoadingUICoroutine 的协程。
                StartCoroutine(LoadingUICoroutine());
                GameController._instance.LoadData("0");//读取第一个剧本

                break;
            case "SaveButton":
                //SaveData
                SaveProfliePanel.GetComponent<Image>().color = new Color(1,1,1,0.5f);
                UIController._instance.SetRaycastTargetOn(SaveProfliePanel.transform);
                UIController._instance.SetRaycastTargetOff(LoadProfliePanel.transform);
                
                break;
            case "CloseSaveButton":
                //SaveData
                Debug.Log("关闭存档");
                SaveProfliePanel.GetComponent<Image>().color = new Color(1, 1, 1, 0);//透明度设为0而不是SetActive
                UIController._instance.SetRaycastTargetOff(SaveProfliePanel.transform);
                UIController._instance.SetRaycastTargetOff(LoadProfliePanel.transform);
                break;
            case "LoadButton"://打开存档面板
                //LoadData
                LoadProfliePanel.GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
                UIController._instance.SetRaycastTargetOn(LoadProfliePanel.transform);
                UIController._instance.SetRaycastTargetOff(SaveProfliePanel.transform);
                GameController._instance.InitProfileData();
                break;
            case "CloseLoadButton":
                //LoadData
                LoadProfliePanel.GetComponent<Image>().color = new Color(1, 1, 1, 0);
                UIController._instance.SetRaycastTargetOff(SaveProfliePanel.transform);
                UIController._instance.SetRaycastTargetOff(LoadProfliePanel.transform);
                break;
            case "AutoButton"://自动播放
                TreatmentController._instance.treatmentAutoPlay = !TreatmentController._instance.treatmentAutoPlay;
                AutoPlayingSignal.SetActive(!AutoPlayingSignal.activeInHierarchy);//自动播放标志
                break;
            case "TalkBg1"://在文本栏单机
                //读取下一行剧本
                if (UIController._instance.showTalkTexting)
                {
                    UIController._instance.StopCoroutine("ShowTalkTextCoroutine");
                    UIController._instance.showTalkTexting = false;
                    UIController._instance.TalkText.text = UIController._instance.text;
                }
                else
                {
                    TreatmentController._instance.ReadTreatmentLine(TreatmentController._instance.index);
                }
                break;
            case "Settings"://打开设置面板
                SettingsPanel.SetActive(true);
                UIController._instance.InitSettingsPanel();
                AddVSynctoggleListener();
                break;
            case "MainMenuButton":
                //回到主界面
                MainMenuPanel.SetActive(true);
                Destroy(GameObject.Find("TestRead"));//销毁TestRead，避免下次再进入游戏时出现两个TestRead
                transform.parent.gameObject.SetActive(false);//关闭设置面板
                break;


            case "OpenLogs"://打开文字回想面板
                UIController._instance.OpenLogs();
                break;
            case "CloseLogs"://关闭文字回想面板
                UIController._instance.CloseLogs();
                break;
           
            case "CloseSettings"://关闭设置面板
                SettingsPanel.SetActive(false);
                break;
            case "CloseAbout"://关闭设置面板
                SettingsPanel.SetActive(false);
                break;
            case "AboutGame"://开启关于游戏
                AboutGame.SetActive(true);
                break;
            case "CloseAboutGame"://关闭关于游戏
                AboutGame.SetActive(false);
                break;
            case "ExitGame"://退出游戏的按钮
                Application.Quit();
                break;


        }
        if (gameObject.name.StartsWith("SaveProfileGrid"))//点击存档按钮,因为存档按钮是实例化在gridLayout中的，所以名字都有规律，所以以这个前缀开头的就是存档按钮
        {
            SaveProfileGrid();

        }
        if (gameObject.name.StartsWith("LoadProfileGrid"))
        {
            LoadProfileGrid();
        }
        if(gameObject.name.StartsWith("DialougueEmpty"))//文本回想
        {
            DialougueEmpty();

        }
    }
}
