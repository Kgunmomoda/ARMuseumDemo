using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RoadController : MonoBehaviour
{
    //游戏控制
    private GameController gameController;
    /// <summary>
    /// 出发点下拉列表
    /// </summary>
    public Dropdown dpdStart;
    /// <summary>
    /// 到达点下拉列表
    /// </summary>
    public Dropdown dpdArrival;
    /// <summary>
    /// 按钮
    /// </summary>
    public SelectButton prefab;
    /// <summary>
    /// 按钮容器
    /// </summary>
    public Transform svContent;
    /// <summary>
    /// 显示信息
    /// </summary>
    public Text info;
    /// <summary>
    /// 关键点列表
    /// </summary>
    private List<KeyPoint> keyPoints;
    /// <summary>
    /// 选中对象
    /// </summary>
    private Transform selected;
    /// <summary>
    /// 删除按钮
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
    /// 加载路径
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
    /// 保存路径
    /// </summary>
    public void SaveRoads()
    {
        string[] jsons = new string[svContent.childCount];
        for (int i = 0; i < svContent.childCount; i++)
        {
            jsons[i] = JsonUtility.ToJson(svContent.GetChild(i).GetComponent<SelectButton>().road);
        }
        gameController.SaveRoads(jsons);
        info.text = "保存成功。";
    }
    /// <summary>
    /// 删除路径
    /// </summary>
    public void DeleteRoad()
    {
        if(selected != null)
        {
            Destroy(selected.gameObject);
            info.text = "删除成功。";
            btnDelete.interactable = false;
        }else
        {
            info.text = "请选择删除对象。";
        }
    }
    /// <summary>
    /// 按钮点击
    /// </summary>
    /// <param name="btnTF"></param>
    private void SelectButtonClicked(Transform btnTF)
    {
        selected = btnTF;
        info.text = btnTF.GetComponentInChildren<Text>().text;
        btnDelete.interactable = true;
    }
    /// <summary>
    /// 添加路径
    /// </summary>
    public void AddRoad()
    {
        SelectButton btn = CreateSelectButton();
        
        btn.road.startName = dpdStart.captionText.text;
        btn.road.arrivalName = dpdArrival.captionText.text;
        btn.road.startPosition = GetPositionByName(btn.road.startName);
        btn.road.arrivalPosition = GetPositionByName(btn.road.arrivalName);

        btn.GetComponentInChildren<Text>().text = btn.road.startName + "<===>" + btn.road.arrivalName;

        info.text = "添加成功。";
    }
    /// <summary>
    /// 根据关键点名称获取坐标
    /// </summary>
    /// <param name="pName">名称</param>
    /// <returns>坐标</returns>
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
    /// 绑定下拉列表
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
