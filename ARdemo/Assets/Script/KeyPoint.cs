using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class KeyPoint
{
    /// <summary>
    /// 位置
    /// </summary>
    public Vector3 position;
    /// <summary>
    /// 角度
    /// </summary>
    public Quaternion rotation;
    /// <summary>
    /// 名称
    /// </summary>
    public string name;
    /// <summary>
    /// 类型：0=目的地；1=途经点
    /// </summary>
    public int pointType;
}

