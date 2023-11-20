using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class QualityManager : MonoBehaviour
{
    public static QualityManager _instance;

    private void Awake()
    {
        _instance = this;

    }

    public void EnableVSync()
    {
        QualitySettings.vSyncCount = 1;
        Debug.Log("已开启垂直同步！");
    }
    public void DisableVSync()
    {
        QualitySettings.vSyncCount = 0;
        Debug.Log("已关闭垂直同步！");
    }

}
