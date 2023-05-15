using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Move : MonoBehaviour
{
    private bool isNextMove = false;
    private Vector3 point;
    public NavMeshAgent agent;
    private NavMeshPath path;
    public Transform arrival;
    public LineRenderer lineRenderer;

    private void Start()
    {
        path = new NavMeshPath();
        InvokeRepeating("DisplayPath", 0, 0.5f);
        //DisplayPath();
    }

    private void Update()
    {     
        //当鼠标点击时，才触发射线检测
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            //当检测到地面
            if (Physics.Raycast(ray, out hitInfo))
            {
                isNextMove = true;
                point = hitInfo.point;
                //将isNextMove设为true，然后保存当前撞击点位置
            }
        }

        if (isNextMove == true)
        //当isNextMove为真，则不停调用Move
        {
            MoveToPoint(point);
        }
    }

    private void MoveToPoint(Vector3 pos)
    {
        //使用Vector3的插值函数来移动位置
        transform.position = Vector3.MoveTowards(transform.position, pos, Time.deltaTime * 4.0f);

         //当目标抵达位置的时候，将isNextMove置为false，等待下一次移动指令
        if (transform.position == pos)
            isNextMove = false;
    }

    /// <summary>
    /// 显示路径
    /// </summary>
    private void DisplayPath()
    {
        //agent.enabled = true;
        agent.CalculatePath(arrival.position, path);
        lineRenderer.positionCount = path.corners.Length;
        lineRenderer.SetPositions(path.corners);
        //agent.enabled = false;
    }
}
