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

        Debug.Log("关键点加载成功");
    }

    [MenuItem("Map Tools/LoadRoads")]
    public static void LoadRoads()
    {
        string pathRoads = Application.persistentDataPath + "/roads.txt";
        GameObject parents = GameObject.Find("/NavRoot/Roads");

        var list = LoadRoads(pathRoads);
        foreach (var item in list)
        {
            Road road = JsonUtility.FromJson<Road>(item);
            GameObject go = PrefabUtility.InstantiatePrefab(Resources.Load("Road"), parents.transform) as GameObject;
            if (go == null)
            {
                Debug.LogErrorFormat("no GameObject");
                return;
            }
            go.name = road.startName+"_"+road.arrivalName;
            //go.transform.position = keyPoint.position;
            //go.transform.rotation = keyPoint.rotation;
        }

        Debug.Log("关键点加载成功");
    }

    private static List<string> LoadKeyPoins(string pathKeyPoints)
    {
        return LoadStringList(pathKeyPoints);
    }

    /// <summary>
    /// 加载路径
    /// </summary>
    /// <returns>路径json列表</returns>
    private static List<string> LoadRoads(string pathRoads)
    {
        return LoadStringList(pathRoads);
    }

    /// <summary>
    /// 读取文本信息
    /// </summary>
    /// <param name="path">文本路径</param>
    /// <returns>字符串列表</returns>
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
