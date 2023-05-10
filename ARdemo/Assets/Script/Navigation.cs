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
    /// ����Ŀ��
    /// </summary>
    public Transform arrival;
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
    /// <summary>
    /// ���
    /// </summary>
    public Transform player;
    /// <summary>
    /// ������Ϣ
    /// </summary>
    public Text Status;
    public Transform prefabArrival;

    void Start()
    {
        LoadMap(); //���ص��Ƶ�ͼ    
        path = new NavMeshPath();//��ʼ������·��
        HideElement();
        InvokeRepeating("DisplayPath", 0, 0.5f);
        arrival.gameObject.SetActive(true); //��ʾĿ���
    }

    private void Update()
    {
        Status.text = "���λ��:" + player.position + ":" + "Ŀ��λ��:" + arrival.position;
    }

    /// <summary>
    /// ���ص�ͼ
    /// </summary>
    private void LoadMap()
    {
        //���õ�ͼ
        //map.MapManagerSource.ID = PlayerPrefs.GetString("MapID");
        //map.MapManagerSource.Name = PlayerPrefs.GetString("MapName");
        //��ͼ���ط���
        map.MapLoad += (map, status, error) =>
        {
            if (status)
            {
                Status.text = "��ͼ���سɹ���" + map.ID;
            }
            else
            {
                Status.text = "��ͼ����ʧ�ܡ�" + error;
            }
        };
        //��ͼ��λ����
        map.MapLocalized += () =>
        {
            Status.text = "����ϡ��ռ䶨λ��";
        };
        //ֹͣ��λ����
        map.MapStopLocalize += () =>
        {
            Status.text = "ֹͣϡ��ռ䶨λ";
        };
        Status.text = "��ʼ���ص�ͼ��";
        mapWorker.Localizer.startLocalization();    //���ػ���ͼ
    }

    /// <summary>
    /// ����Ԫ��
    /// </summary>
    private void HideElement()
    {
        //����Ŀ�ĵ�
        foreach (Transform tf in navRoot.Find("Arrivals"))
        {
            tf.gameObject.SetActive(false);
        }
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
    
}
