using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RoadController : MonoBehaviour
{
    //��Ϸ����
    private GameController gameController;
    /// <summary>
    /// �����������б�
    /// </summary>
    public Dropdown dpdStart;
    /// <summary>
    /// ����������б�
    /// </summary>
    public Dropdown dpdArrival;
    /// <summary>
    /// ��ť
    /// </summary>
    public SelectButton prefab;
    /// <summary>
    /// ��ť����
    /// </summary>
    public Transform svContent;
    /// <summary>
    /// ��ʾ��Ϣ
    /// </summary>
    public Text info;
    /// <summary>
    /// �ؼ����б�
    /// </summary>
    private List<KeyPoint> keyPoints;
    /// <summary>
    /// ѡ�ж���
    /// </summary>
    private Transform selected;
    /// <summary>
    /// ɾ����ť
    /// </summary>
    public Button btnDelete;

    // Start is called before the first frame update
    void Start()
    {
        gameController = FindObjectOfType<GameController>();
        btnDelete.interactable = false;
        keyPoints = new List<KeyPoint>();
        BindDropdown();
        LoadRoad();
    }

    /// <summary>
    /// ����·��
    /// </summary>
    private void LoadRoad()
    {
        var list = gameController.LoadRoads();
        foreach (var item in list)
        {
            var btn = CreateSelectButton();
            btn.road = JsonUtility.FromJson<Road>(item);
            btn.GetComponentInChildren<Text>().text = btn.road.startName + "<===>" + btn.road.arrivalName;
        }
    }
    /// <summary>
    /// ����·��
    /// </summary>
    public void SaveRoads()
    {
        string[] jsons = new string[svContent.childCount];
        for (int i = 0; i < svContent.childCount; i++)
        {
            jsons[i] = JsonUtility.ToJson(svContent.GetChild(i).GetComponent<SelectButton>().road);
        }
        gameController.SaveRoads(jsons);
        info.text = "����ɹ���";
    }
    /// <summary>
    /// ɾ��·��
    /// </summary>
    public void DeleteRoad()
    {
        if(selected != null)
        {
            Destroy(selected.gameObject);
            info.text = "ɾ���ɹ���";
            btnDelete.interactable = false;
        }else
        {
            info.text = "��ѡ��ɾ������";
        }
    }
    /// <summary>
    /// ��ť���
    /// </summary>
    /// <param name="btnTF"></param>
    private void SelectButtonClicked(Transform btnTF)
    {
        selected = btnTF;
        info.text = btnTF.GetComponentInChildren<Text>().text;
        btnDelete.interactable = true;
    }
    /// <summary>
    /// ���·��
    /// </summary>
    public void AddRoad()
    {
        SelectButton btn = CreateSelectButton();
        
        btn.road.startName = dpdStart.captionText.text;
        btn.road.arrivalName = dpdArrival.captionText.text;
        btn.road.startPosition = GetPositionByName(btn.road.startName);
        btn.road.arrivalPosition = GetPositionByName(btn.road.arrivalName);

        btn.GetComponentInChildren<Text>().text = btn.road.startName + "<===>" + btn.road.arrivalName;

        info.text = "��ӳɹ���";
    }
    /// <summary>
    /// ���ݹؼ������ƻ�ȡ����
    /// </summary>
    /// <param name="pName">����</param>
    /// <returns>����</returns>
    private Vector3 GetPositionByName(string pName)
    {
        foreach (var kp in keyPoints)
        {
            if (kp.name == pName)
            {
                return kp.position;
            }
        }
        return Vector3.zero;
    }
    /// <summary>
    /// �������б�
    /// </summary>
    private void BindDropdown()
    {
        var list = gameController.LoadKeyPoins();

        foreach (var item in list)
        {
            KeyPoint point = JsonUtility.FromJson<KeyPoint>(item);
            keyPoints.Add(point);
            dpdStart.options.Add(new Dropdown.OptionData(point.name));
            dpdArrival.options.Add(new Dropdown.OptionData(point.name));
            dpdStart.captionText.text = dpdStart.options[0].text;
            dpdArrival.captionText.text = dpdArrival.options[0].text;
        }
    }

    public void back()
    {
        SceneManager.LoadScene("demo");
    }

    private SelectButton CreateSelectButton()
    {
        SelectButton btn = Instantiate(prefab, svContent);
        btn.GetComponent<Button>().onClick.AddListener(() =>
        {
            SelectButtonClicked(btn.transform);
        });
        return btn;
    }
}
