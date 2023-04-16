using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Move : MonoBehaviour
{
    private bool isNextMove = false;
    private Vector3 point;
    public NavMeshAgent agent;
   
    private void Update()
    {     
        //�������ʱ���Ŵ������߼��
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            //����⵽����
            if (Physics.Raycast(ray, out hitInfo))
            {
                isNextMove = true;
                point = hitInfo.point;
                //��isNextMove��Ϊtrue��Ȼ�󱣴浱ǰײ����λ��
            }
        }

        if (isNextMove == true)
        //��isNextMoveΪ�棬��ͣ����Move
        {
            agent.SetDestination(point);
        }
    }

    //private void MoveToPoint(Vector3 pos)
    //{
    //    //ʹ��Vector3�Ĳ�ֵ�������ƶ�λ��
    //    transform.position = Vector3.MoveTowards(transform.position, pos, Time.deltaTime * 4.0f);

    //     //��Ŀ��ִ�λ�õ�ʱ�򣬽�isNextMove��Ϊfalse���ȴ���һ���ƶ�ָ��
    //    if (transform.position == pos)
    //        isNextMove = false;
    //} 
}
