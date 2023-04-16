using easyar;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NavigationController : MonoBehaviour
{
    private GameController gameController;
    /// <summary>
    /// 导航画布
    /// </summary>
    public GameObject panel;
    /// <summary>
    /// 导航按钮
    /// </summary>
    public Button btnNav;
    /// <summary>
    /// 导航按钮
    /// </summary>
    public SelectButton prefabButton;
    /// <summary>
    /// 导航按钮容器
    /// </summary>
    public Transform svContent;
    /// <summary>
    /// 导航根节点
    /// </summary>
    public Transform navRoot;
    /// <summary>
    /// 目的地预制件
    /// </summary>
    public Transform prefabArrival;
    /// <summary>
    /// 路径预制件
    /// </summary>
    public Transform prefabRoad;
    /// <summary>
    /// 导航线
    /// </summary>
    public LineRenderer lineRenderer;
    /// <summary>
    /// 导航代理
    /// </summary>
    public NavMeshAgent agent;
    /// <summary>
    /// 导航路径
    /// </summary>
    private NavMeshPath path;
    public NavMeshSurface surface;
    /// <summary>
    /// 导航目标
    /// </summary>
    private Transform arrival;
    /// <summary>
    /// 玩家
    /// </summary>
    public Transform player;

    public ARSession session;
    public SparseSpatialMapWorkerFrameFilter mapWorker;
    public SparseSpatialMapController map;
    // Start is called before the first frame update
    void Start()
    {
        gameController = FindObjectOfType<GameController>();
        btnNav.interactable = false;
        CloseNavUI();
        LoadMap();
    }

    /// <summary>
    /// 加载地图
    /// </summary>
    private void LoadMap()
    {
        //设置地图
        map.MapManagerSource.ID = PlayerPrefs.GetString("MapID");
        map.MapManagerSource.Name = PlayerPrefs.GetString("MapName");
        //地图加载反馈
        map.MapLoad += (map, status, error) =>
        {
            if (status)
            {
                gameController.SendMessage("ShowMessage", "地图加载成功。"+ map.ID);
            }
            else
            {
                gameController.SendMessage("ShowMessage", "地图加载失败。" + error);
            }
        };
        //地图定位反馈
        map.MapLocalized += () =>
        {
            gameController.SendMessage("ShowMessage", "进入稀疏空间定位。");
            //ClearNav();
            //LoadArrivals();
            //LoadRoads();
            //BakePath();
            //btnNav.interactable = true;
            //ShowNavUI();
        };
        //停止定位反馈
        map.MapStopLocalize += () =>
        {
            gameController.SendMessage("ShowMessage", "停止稀疏空间定位");
        };
        gameController.SendMessage("ShowMessage", "开始加载地图。" + map.MapManagerSource.ID);
        mapWorker.Localizer.startLocalization();    //本地化地图
    }
    /// <summary>
    /// 清理导航元素
    /// </summary>
    private void ClearNav()
    {
        gameController.SendMessage("ShowMessage", "ClearNav");
        //删除按钮
        foreach (Transform tf in svContent)
        {
            Destroy(tf.gameObject);
        }
        //删除目的地
        foreach (Transform tf in navRoot.Find("Arrivals"))
        {
            Destroy(tf.gameObject);
        }
        //删除路径
        foreach (Transform tf in navRoot.Find("Roads"))
        {
            Destroy(tf.gameObject);
        }
    }
    /// <summary>
    /// 按钮点击
    /// </summary>
    /// <param name="btnTF"></param>
    public void SelectButtonClicked(Transform btnTF)
    {
        CancelInvoke("DisplayPath");
        arrival = btnTF.GetComponent<SelectButton>().arrival;

        Transform root = navRoot.Find("Arrivals");
        for (int i = 0; i < root.childCount; i++)
        {
            root.GetChild(i).gameObject.SetActive(false);
        }
        arrival.gameObject.SetActive(true);

        InvokeRepeating("DisplayPath", 0, 0.5f); //循环调用DisplayPath，每隔0.5秒调用一次
        CloseNavUI();
    }
    /// <summary>
    /// 显示路径
    /// </summary>
    private void DisplayPath()
    {
        agent.transform.position = player.position;
        agent.enabled = true;
        agent.CalculatePath(arrival.position, path);
        lineRenderer.positionCount = path.corners.Length;
        lineRenderer.SetPositions(path.corners);
        agent.enabled = false;
    }
    /// <summary>
    /// 烘培路径
    /// </summary>
    private void BakePath()
    {
        agent.transform.position = player.position;
        agent.enabled = false;
        surface.BuildNavMesh();
        path = new NavMeshPath();
    }

    /// <summary>
    /// 设置导航线样式
    /// </summary>
    private void SetLine()
    {
        lineRenderer = navRoot.Find("Line").gameObject.AddComponent<LineRenderer>();
        Debug.Log(lineRenderer);
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.positionCount = 0;
        lineRenderer.widthMultiplier = 0.05f;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] {
                new GradientColorKey(Color.blue, 0.0f),
                new GradientColorKey(Color.blue, 1.0f) },
            new GradientAlphaKey[] {
                new GradientAlphaKey(1f, 0.0f),
                new GradientAlphaKey(1f, 1.0f) });
        lineRenderer.colorGradient = gradient;
    }
    /// <summary>
    /// 加载路径
    /// </summary>
    private void LoadRoads()
    {
        gameController.SendMessage("ShowMessage", "LoadRoads");
        var list = gameController.LoadRoads();

        var temp = new GameObject().transform;
        temp.parent = navRoot.Find("Roads");

        foreach (var item in list)
        {
            var road = JsonUtility.FromJson<Road>(item);
            var tfRoad = Instantiate(prefabRoad, navRoot.Find("Roads"));

            tfRoad.localPosition = (road.startPosition + road.arrivalPosition) / 2;
            temp.localPosition = road.arrivalPosition;
            tfRoad.LookAt(temp);
            //为什么是0.1？
            //与tfRoad预制体的scale。z=0.1有关
            //z=0.1的时候，tfRoad预制体的长度刚好是1米
            tfRoad.localScale = new Vector3(0.02f, 1f, (road.arrivalPosition - road.startPosition).magnitude * 0.1f + 0.2f);
        }
        Destroy(temp.gameObject);
    }
    /// <summary>
    /// 加载目标
    /// </summary>
    private void LoadArrivals()
    {
        gameController.SendMessage("ShowMessage", "LoadArrivals");
        var list = gameController.LoadKeyPoins();
        foreach (var item in list)
        {
            KeyPoint point = JsonUtility.FromJson<KeyPoint>(item);
            if (point.pointType == 0)
            {
                var btn = Instantiate(prefabButton, svContent);
                btn.keyPoint = point;
                btn.GetComponentInChildren<Text>().text = point.name;

                var arrivalTemp = Instantiate(prefabArrival, navRoot.Find("Arrivals"));
                arrivalTemp.localPosition = point.position;
                btn.arrival = arrivalTemp;
                arrivalTemp.gameObject.SetActive(false);
            }
        }
    }
    /// <summary>
    /// 显示导航菜单
    /// </summary>
    public void ShowNavUI()
    {
        panel.SetActive(true);
    }
    /// <summary>
    /// 关闭导航菜单
    /// </summary>
    public void CloseNavUI()
    {
        panel.SetActive(false);
    }

    public void back()
    {
        SceneManager.LoadScene("demo");
    }
}
