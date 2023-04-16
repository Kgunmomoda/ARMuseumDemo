using easyar;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class KeyPointController : MonoBehaviour
{
    /// <summary>
    /// 关键点画布
    /// </summary>
    public GameObject panel;
    /// <summary>
    /// 信息文本框
    /// </summary>
    public Text info;
    /// <summary>
    /// 游戏控制
    /// </summary>
    private GameController gameController;
    public ARSession session;
    public SparseSpatialMapWorkerFrameFilter mapWorker;
    public SparseSpatialMapController map;
    /// <summary>
    /// 本地化状态
    /// </summary>
    private bool localized = false;
    /// <summary>
    /// 选中的对象
    /// </summary>
    private Transform selected;
    /// <summary>
    /// 添加按钮
    /// </summary>
    public Button btnAdd;
    /// <summary>
    /// Scroll view的content的transform
    /// </summary>
    public Transform svContent;
    /// <summary>
    /// 名称输入
    /// </summary>
    public InputField inputField;
    /// <summary>
    /// 关键点按钮预制件
    /// </summary>
    public SelectButton prefab;
    /// <summary>
    /// 删除按钮
    /// </summary>
    public Button btnDelete;
    /// <summary>
    /// 返回按钮
    /// </summary>
    public GameObject btnBack;
    public Dropdown dropdown;

    private void Start()
    {
        btnBack.SetActive(true);
        panel.SetActive(false);
        btnAdd.interactable = false;
        gameController = FindObjectOfType<GameController>();
        LoadMap();
        LoadKeyPoints();
    }

    /// <summary>
    /// 加载地图
    /// </summary>
    private void LoadMap()
    {
        //设置地图
        map.MapManagerSource.ID = PlayerPrefs.GetString("MapID");
        map.MapManagerSource.Name = PlayerPrefs.GetString("MapName");
        //地图获取反馈
        map.MapLoad += (map, status, error) =>
        {
            if (status)
            {
                localized = true;
                gameController.SendMessage("ShowMessage", "地图加载成功。");
            }
            else
            {
                gameController.SendMessage("ShowMessage", "地图加载失败。" + error);
            }
        };
        //定位成功事件
        map.MapLocalized += () =>
        {
            gameController.SendMessage("ShowMessage", "进入稀疏空间定位。");
        };
        //停止定位事件
        map.MapStopLocalize += () =>
        {
            gameController.SendMessage("ShowMessage", "停止稀疏空间定位");
        };
        gameController.SendMessage("ShowMessage", "开始加载地图。");
        mapWorker.Localizer.startLocalization();    //本地化地图
    }


    #region 关键点控制
    /// <summary>
    /// 加载关键点
    /// </summary>
    private void LoadKeyPoints()
    {
        var list = gameController.LoadKeyPoins();
        foreach (var item in list)
        {
            SelectButton btn = CreateSelectButton();
            btn.keyPoint = JsonUtility.FromJson<KeyPoint>(item);
            
            btn.GetComponentInChildren<Text>().text = btn.keyPoint.name;
        }
    }
    /// <summary>
    /// 保存关键点
    /// </summary>
    public void SaveKeyPoints()
    {
        string[] jsons = new string[svContent.childCount];
        for (int i = 0; i < svContent.childCount; i++)
        {
            jsons[i] = JsonUtility.ToJson(svContent.GetChild(i).GetComponent<SelectButton>().keyPoint);
        }
        gameController.SaveKeyPoints(jsons);
        info.text = "保存完成。";
    }
    /// <summary>
    /// 删除关键点
    /// </summary>
    public void DeleteKeyPoint()
    {
        Destroy(selected.gameObject);
        info.text = "删除成功。";
        btnDelete.interactable = false;
    }
    /// <summary>
    /// 按钮点击
    /// </summary>
    /// <param name="btnTF"></param>
    public void SelectButtonClicked(Transform btnTF)
    {
        selected = btnTF;
        info.text = btnTF.GetComponentInChildren<Text>().text;
        btnDelete.interactable = true;
        btnAdd.interactable = false;
    }
    /// <summary>
    /// 添加关键点
    /// </summary>
    public void AddKeyPoint()
    {
        //建议添加弹窗反馈
        if (!string.IsNullOrEmpty(inputField.text) && selected != null)
        {
            SelectButton btn = CreateSelectButton();

            btn.keyPoint.name = inputField.text;
            btn.keyPoint.position = selected.localPosition;
            btn.keyPoint.pointType = dropdown.value;//0:目的点 1:途径点

            btn.GetComponentInChildren<Text>().text = inputField.text;
          
            inputField.text = "";
            selected = null;
            info.text = "添加成功。";
            btnAdd.interactable = false;
        }
    }
    #endregion

    #region  点击物体
    void Update()
    {
        if (Input.touchCount == 1 && localized && !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
        {           
            var touch = Input.touches[0];
            var viewPoint = new Vector3(touch.position.x, touch.position.y, 0);
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(viewPoint);
            bool flag = Physics.Raycast(ray, out hit);
            gameController.SendMessage("ShowMessage", string.Format("flag:{0} position:{1}", flag, hit.transform.position)); ;
            if (flag)
            {
                //gameController.SendMessage("ShowMessage", "selected position" + selected.position);
                selected = hit.transform;
                btnAdd.interactable = true;
                HitObject();
                ShowPanel();
            }
        }
    }
    /// <summary>
    /// 点中游戏对象
    /// </summary>
    private void HitObject()
    {
        var tf = new GameObject().transform;
        tf.position = selected.position;
        tf.parent = map.transform;
        selected = tf;
    }
    #endregion

    #region panel控制
    private void ShowPanel()
    {
        btnBack.SetActive(false);
        panel.SetActive(true);
        info.text = "Position:" + selected.localPosition;
    }

    public void HiddenPanel()
    {
        btnBack.SetActive(true);
        panel.SetActive(false);
        info.text = "";
    }
    #endregion


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

