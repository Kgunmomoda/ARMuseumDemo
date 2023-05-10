using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class MapTool
{
    [MenuItem("Map Tools/LoadKeyPoints")]
    public static void LoadKeyPoints()
    {
        string pathKeyPoints = Application.persistentDataPath + "/keypoints.txt";
        GameObject parents = GameObject.Find("/NavRoot/Arrivals");

        Debug.Log(pathKeyPoints);

        var list = LoadKeyPoins(pathKeyPoints);
        foreach (var item in list)
        {
            KeyPoint keyPoint = JsonUtility.FromJson<KeyPoint>(item);
            GameObject go = PrefabUtility.InstantiatePrefab(Resources.Load("ArrivalBall"), parents.transform) as GameObject;
            if (go == null)
            {
                Debug.LogErrorFormat("no GameObject");
                return;
            }
            go.name = keyPoint.name + keyPoint.pointType;
            go.transform.position = keyPoint.position;
            go.transform.rotation = keyPoint.rotation;
        }

        Debug.Log("�ؼ�����سɹ�");
    }

    [MenuItem("Map Tools/LoadRoads")]
    public static void LoadRoads()
    {
        string pathRoads = Application.persistentDataPath + "/roads.txt";
        GameObject parents = GameObject.Find("/NavRoot/Roads");

        var temp = new GameObject().transform;
        temp.parent = parents.transform;
        var list = LoadRoads(pathRoads);
        foreach (var item in list)
        {
            Road road = JsonUtility.FromJson<Road>(item);
            GameObject tfRoad = PrefabUtility.InstantiatePrefab(Resources.Load("Road"), parents.transform) as GameObject;
            if (tfRoad == null)
            {
                Debug.LogErrorFormat("no GameObject");
                return;
            }
            tfRoad.name = road.startName + "_" + road.arrivalName;
            tfRoad.transform.localPosition = (road.startPosition + road.arrivalPosition) / 2;
            temp.localPosition = road.arrivalPosition;
            tfRoad.transform.LookAt(temp);
            tfRoad.transform.localScale = new Vector3(0.04f, 1f, (road.arrivalPosition - road.startPosition).magnitude * 0.1f + 0.2f);
        }
        GameObject.DestroyImmediate(temp.gameObject);
        Debug.Log("·�����سɹ�");
    }

    private static List<string> LoadKeyPoins(string pathKeyPoints)
    {
        return LoadStringList(pathKeyPoints);
    }

    /// <summary>
    /// ����·��
    /// </summary>
    /// <returns>·��json�б�</returns>
    private static List<string> LoadRoads(string pathRoads)
    {
        return LoadStringList(pathRoads);
    }

    /// <summary>
    /// ��ȡ�ı���Ϣ
    /// </summary>
    /// <param name="path">�ı�·��</param>
    /// <returns>�ַ����б�</returns>
    private static List<string> LoadStringList(string path)
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
        catch (System.Exception ex)
        {
            Debug.Log(ex.Message);
        }
        return list;
    }

}
