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
    /// �ؼ��㻭��
    /// </summary>
    public GameObject panel;
    /// <summary>
    /// ��Ϣ�ı���
    /// </summary>
    public Text info;
    /// <summary>
    /// ��Ϸ����
    /// </summary>
    private GameController gameController;
    public ARSession session;
    public SparseSpatialMapWorkerFrameFilter mapWorker;
    public SparseSpatialMapController map;
    /// <summary>
    /// ���ػ�״̬
    /// </summary>
    private bool localized = false;
    /// <summary>
    /// ѡ�еĶ���
    /// </summary>
    private Transform selected;
    /// <summary>
    /// ��Ӱ�ť
    /// </summary>
    public Button btnAdd;
    /// <summary>
    /// Scroll view��content��transform
    /// </summary>
    public Transform svContent;
    /// <summary>
    /// ��������
    /// </summary>
    public InputField inputField;
    /// <summary>
    /// �ؼ��㰴ťԤ�Ƽ�
    /// </summary>
    public SelectButton prefab;
    /// <summary>
    /// ɾ����ť
    /// </summary>
    public Button btnDelete;
    /// <summary>
    /// ���ذ�ť
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
    /// ���ص�ͼ
    /// </summary>
    private void LoadMap()
    {
        //���õ�ͼ
        map.MapManagerSource.ID = PlayerPrefs.GetString("MapID");
        map.MapManagerSource.Name = PlayerPrefs.GetString("MapName");
        //��ͼ��ȡ����
        map.MapLoad += (map, status, error) =>
        {
            if (status)
            {
                localized = true;
                gameController.SendMessage("ShowMessage", "��ͼ���سɹ���");
            }
            else
            {
                gameController.SendMessage("ShowMessage", "��ͼ����ʧ�ܡ�" + error);
            }
        };
        //��λ�ɹ��¼�
        map.MapLocalized += () =>
        {
            gameController.SendMessage("ShowMessage", "����ϡ��ռ䶨λ��");
        };
        //ֹͣ��λ�¼�
        map.MapStopLocalize += () =>
        {
            gameController.SendMessage("ShowMessage", "ֹͣϡ��ռ䶨λ");
        };
        gameController.SendMessage("ShowMessage", "��ʼ���ص�ͼ��");
        mapWorker.Localizer.startLocalization();    //���ػ���ͼ
    }


    #region �ؼ������
    /// <summary>
    /// ���عؼ���
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
    /// ����ؼ���
    /// </summary>
    public void SaveKeyPoints()
    {
        string[] jsons = new string[svContent.childCount];
        for (int i = 0; i < svContent.childCount; i++)
        {
            jsons[i] = JsonUtility.ToJson(svContent.GetChild(i).GetComponent<SelectButton>().keyPoint);
        }
        gameController.SaveKeyPoints(jsons);
        info.text = "������ɡ�";
    }
    /// <summary>
    /// ɾ���ؼ���
    /// </summary>
    public void DeleteKeyPoint()
    {
        Destroy(selected.gameObject);
        info.text = "ɾ���ɹ���";
        btnDelete.interactable = false;
    }
    /// <summary>
    /// ��ť���
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
    /// ��ӹؼ���
    /// </summary>
    public void AddKeyPoint()
    {
        //������ӵ�������
        if (!string.IsNullOrEmpty(inputField.text) && selected != null)
        {
            SelectButton btn = CreateSelectButton();

            btn.keyPoint.name = inputField.text;
            btn.keyPoint.position = selected.localPosition;
            btn.keyPoint.pointType = dropdown.value;//0:Ŀ�ĵ� 1:;����

            btn.GetComponentInChildren<Text>().text = inputField.text;
          
            inputField.text = "";
            selected = null;
            info.text = "��ӳɹ���";
            btnAdd.interactable = false;
        }
    }
    #endregion

    #region  �������
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
    /// ������Ϸ����
    /// </summary>
    private void HitObject()
    {
        var tf = new GameObject().transform;
        tf.position = selected.position;
        tf.parent = map.transform;
        selected = tf;
    }
    #endregion

    #region panel����
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

