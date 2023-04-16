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
    /// ��������
    /// </summary>
    public GameObject panel;
    /// <summary>
    /// ������ť
    /// </summary>
    public Button btnNav;
    /// <summary>
    /// ������ť
    /// </summary>
    public SelectButton prefabButton;
    /// <summary>
    /// ������ť����
    /// </summary>
    public Transform svContent;
    /// <summary>
    /// �������ڵ�
    /// </summary>
    public Transform navRoot;
    /// <summary>
    /// Ŀ�ĵ�Ԥ�Ƽ�
    /// </summary>
    public Transform prefabArrival;
    /// <summary>
    /// ·��Ԥ�Ƽ�
    /// </summary>
    public Transform prefabRoad;
    /// <summary>
    /// ������
    /// </summary>
    public LineRenderer lineRenderer;
    /// <summary>
    /// ��������
    /// </summary>
    public NavMeshAgent agent;
    /// <summary>
    /// ����·��
    /// </summary>
    private NavMeshPath path;
    public NavMeshSurface surface;
    /// <summary>
    /// ����Ŀ��
    /// </summary>
    private Transform arrival;
    /// <summary>
    /// ���
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
    /// ���ص�ͼ
    /// </summary>
    private void LoadMap()
    {
        //���õ�ͼ
        map.MapManagerSource.ID = PlayerPrefs.GetString("MapID");
        map.MapManagerSource.Name = PlayerPrefs.GetString("MapName");
        //��ͼ���ط���
        map.MapLoad += (map, status, error) =>
        {
            if (status)
            {
                gameController.SendMessage("ShowMessage", "��ͼ���سɹ���"+ map.ID);
            }
            else
            {
                gameController.SendMessage("ShowMessage", "��ͼ����ʧ�ܡ�" + error);
            }
        };
        //��ͼ��λ����
        map.MapLocalized += () =>
        {
            gameController.SendMessage("ShowMessage", "����ϡ��ռ䶨λ��");
            //ClearNav();
            //LoadArrivals();
            //LoadRoads();
            //BakePath();
            //btnNav.interactable = true;
            //ShowNavUI();
        };
        //ֹͣ��λ����
        map.MapStopLocalize += () =>
        {
            gameController.SendMessage("ShowMessage", "ֹͣϡ��ռ䶨λ");
        };
        gameController.SendMessage("ShowMessage", "��ʼ���ص�ͼ��" + map.MapManagerSource.ID);
        mapWorker.Localizer.startLocalization();    //���ػ���ͼ
    }
    /// <summary>
    /// ������Ԫ��
    /// </summary>
    private void ClearNav()
    {
        gameController.SendMessage("ShowMessage", "ClearNav");
        //ɾ����ť
        foreach (Transform tf in svContent)
        {
            Destroy(tf.gameObject);
        }
        //ɾ��Ŀ�ĵ�
        foreach (Transform tf in navRoot.Find("Arrivals"))
        {
            Destroy(tf.gameObject);
        }
        //ɾ��·��
        foreach (Transform tf in navRoot.Find("Roads"))
        {
            Destroy(tf.gameObject);
        }
    }
    /// <summary>
    /// ��ť���
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

        InvokeRepeating("DisplayPath", 0, 0.5f); //ѭ������DisplayPath��ÿ��0.5�����һ��
        CloseNavUI();
    }
    /// <summary>
    /// ��ʾ·��
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
    /// ����·��
    /// </summary>
    private void BakePath()
    {
        agent.transform.position = player.position;
        agent.enabled = false;
        surface.BuildNavMesh();
        path = new NavMeshPath();
    }

    /// <summary>
    /// ���õ�������ʽ
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
    /// ����·��
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
            //Ϊʲô��0.1��
            //��tfRoadԤ�����scale��z=0.1�й�
            //z=0.1��ʱ��tfRoadԤ����ĳ��ȸպ���1��
            tfRoad.localScale = new Vector3(0.02f, 1f, (road.arrivalPosition - road.startPosition).magnitude * 0.1f + 0.2f);
        }
        Destroy(temp.gameObject);
    }
    /// <summary>
    /// ����Ŀ��
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
    /// ��ʾ�����˵�
    /// </summary>
    public void ShowNavUI()
    {
        panel.SetActive(true);
    }
    /// <summary>
    /// �رյ����˵�
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
