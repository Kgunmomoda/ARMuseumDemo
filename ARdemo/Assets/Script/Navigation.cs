using easyar;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Navigation : MonoBehaviour
{
    public ARSession session;
    public SparseSpatialMapWorkerFrameFilter mapWorker;
    public SparseSpatialMapController map;
    public Transform navRoot;
    /// <summary>
    /// 导航目标
    /// </summary>
    public Transform arrival;
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
    /// <summary>
    /// 玩家
    /// </summary>
    public Transform player;
    /// <summary>
    /// 调试信息
    /// </summary>
    public Text Status;
    public Transform prefabArrival;

    void Start()
    {
        LoadMap(); //加载点云地图    
        path = new NavMeshPath();//初始化导航路径
        HideElement();
        InvokeRepeating("DisplayPath", 0, 0.5f);
        arrival.gameObject.SetActive(true); //显示目标点
    }

    private void Update()
    {
        Status.text = "玩家位置:" + player.position + ":" + "目标位置:" + arrival.position;
    }

    /// <summary>
    /// 加载地图
    /// </summary>
    private void LoadMap()
    {
        //设置地图
        //map.MapManagerSource.ID = PlayerPrefs.GetString("MapID");
        //map.MapManagerSource.Name = PlayerPrefs.GetString("MapName");
        //地图加载反馈
        map.MapLoad += (map, status, error) =>
        {
            if (status)
            {
                Status.text = "地图加载成功。" + map.ID;
            }
            else
            {
                Status.text = "地图加载失败。" + error;
            }
        };
        //地图定位反馈
        map.MapLocalized += () =>
        {
            Status.text = "进入稀疏空间定位。";
        };
        //停止定位反馈
        map.MapStopLocalize += () =>
        {
            Status.text = "停止稀疏空间定位";
        };
        Status.text = "开始加载地图。";
        mapWorker.Localizer.startLocalization();    //本地化地图
    }

    /// <summary>
    /// 隐藏元素
    /// </summary>
    private void HideElement()
    {
        //隐藏目的地
        foreach (Transform tf in navRoot.Find("Arrivals"))
        {
            tf.gameObject.SetActive(false);
        }
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
    
}
