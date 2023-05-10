using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using System.IO;

/// <summary>
/// ��Ϸ����
/// </summary>
public class GameController : MonoBehaviour
{
    private static GameController instance = null;
    /// <summary>
    /// ����ĵ�ͼ����
    /// </summary>
    public string inputName;
    /// <summary>
    /// ��ʾ��Ϣ�ı���
    /// </summary>
    private Text txtShow;
    /// <summary>
    /// �ؼ���洢·��
    /// </summary>
    private string pathKeyPoints;
    /// <summary>
    /// ����·���洢·��
    /// </summary>
    private string pathRoads;

    void Awake()
    {
        //ʵ�ֵ�ʵ��
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (this != instance)
        {
            Destroy(gameObject);
        }

        pathKeyPoints = Application.persistentDataPath + "/keypoints.txt";
        pathRoads = Application.persistentDataPath + "/roads.txt";
    }

    void Start()
    {
        txtShow = transform.GetComponentInChildren<Text>();
        txtShow.gameObject.SetActive(false);
    }

    #region ��ʾ��Ϣ
    /// <summary>
    /// /// ��ʾ��Ϣ
    /// </summary>
    /// <param name="message">��Ϣ</param>
    public void ShowMessage(string message)
    {
        StopCoroutine("EndShowMessage");
        txtShow.gameObject.SetActive(true);
        txtShow.text = message;
        StartCoroutine("EndShowMessage");
    }
    /// <summary>
    /// ������Ϣ
    /// </summary>
    /// <returns></returns>
    private IEnumerator EndShowMessage()
    {
        yield return new WaitForSeconds(4f);
        txtShow.text = "";
        txtShow.gameObject.SetActive(false);
    }

    #endregion

    #region ��ȡ�ؼ����·��
    /// <summary>
    /// ����ؼ���
    /// </summary>
    /// <param name="jsons">json�ַ�������</param>
    public void SaveKeyPoints(string[] jsons)
    {
        SaveStringArray(jsons, pathKeyPoints);
    }
    /// <summary>
    /// ���عؼ���
    /// </summary>
    /// <returns>�ؼ���json�б�</returns>
    public List<string> LoadKeyPoins()
    {
        return LoadStringList(pathKeyPoints);
    }
    /// <summary>
    /// ����·��
    /// </summary>
    /// <param name="jsons">json�ַ�������</param>
    public void SaveRoads(string[] jsons)
    {
        SaveStringArray(jsons, pathRoads);
    }
    /// <summary>
    /// ����·��
    /// </summary>
    /// <returns>·��json�б�</returns>
    public List<string> LoadRoads()
    {
        return LoadStringList(pathRoads);
    }
    /// <summary>
    /// �����ַ�������
    /// </summary>
    /// <param name="stringArray">�ַ�������</param>
    /// <param name="path">����·��</param>
    private void SaveStringArray(string[] stringArray, string path)
    {
        try
        {
            using (StreamWriter writer = new StreamWriter(path))
            {
                foreach (var s in stringArray)
                {
                    writer.WriteLine(s);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }
    /// <summary>
    /// ��ȡ�ı���Ϣ
    /// </summary>
    /// <param name="path">�ı�·��</param>
    /// <returns>�ַ����б�</returns>
    private List<string> LoadStringList(string path)
    {
        List<string> list = new List<string>();
        try
        {
            using (StreamReader reader = new StreamReader(path))
            {
                while (!reader.EndOfStream)
                {
                    list.Add(reader.ReadLine());
                }
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
        return list;
    }

    #endregion
}


