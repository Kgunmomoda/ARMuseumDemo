using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMoving : MonoBehaviour
{
    float radian = 0; // ����
    float perRadian = 0.02f; // ÿ�α仯�Ļ���
    float radius = 0.4f; // �뾶
    Vector3 oldPos; // ��ʼʱ�������
    public Transform cube;
    // Use this for initialization
    void Start()
    {
        oldPos = transform.position; // �������λ�ñ��浽oldPos
    }

    // Update is called once per frame
    void Update()
    {
        if (oldPos != cube.position) oldPos = cube.position + new Vector3(0,(float)-0.6,0);
        radian += perRadian; // ����ÿ�μ�0.03
        float dy = Mathf.Cos(radian) * radius; // dy����������y��ı�����Ҳ����ʹ��sin���ҵ�һ���ʺϵ�ֵ�Ϳ���
        transform.position = oldPos + new Vector3(0, dy, 0);
    }
}
