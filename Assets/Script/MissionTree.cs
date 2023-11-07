using System.Collections.Generic;
using UnityEngine;

public class MissionTree
{
    /// <summary>
    /// ������id
    /// </summary>
    public int id { get; set; }
    /// <summary>
    /// ����id
    /// </summary>
    public int m_id { get; set; }
    /// <summary>
    /// �������
    /// </summary>
    public string title { get; set; }
    /// <summary>
    /// ��������
    /// </summary>
    public string describe { get; set; }
    /// <summary>
    /// ��������
    /// </summary>
    public MissionType type { get; set; }
    /// <summary>
    /// �������
    /// </summary>
    public MissionFilter filter { get; set; }
    /// <summary>
    /// ����Ŀ��
    /// </summary>
    public List<int> target { get; set; }
    /// <summary>
    /// ����Ŀ������
    /// </summary>
    public List<string> target_describe { get; set; }
    /// <summary>
    /// ����Ŀ������
    /// </summary>
    public Dictionary<int, float> target_num { get; set; }
    /// <summary>
    /// �����������
    /// </summary>
    public Dictionary<int, float> complete_num { get; set; }
    /// <summary>
    /// ������
    /// </summary>
    public List<string> award { get; set; }
    /// <summary>
    /// ����������
    /// </summary>
    public List<float> award_num { get; set; }
    /// <summary>
    /// �Ƿ�Ԥͳ��
    /// </summary>
    public bool is_pre_count { get; set; }
    /// <summary>
    /// ��һ����
    /// </summary>
    public MissionTree next { get; set; }
    /// <summary>
    /// ��������б�
    /// </summary>
    public List<int> unlock_mission { get; set; }
    /// <summary>
    /// �����֧
    /// </summary>
    public List<int> branch { get; set; }
    /// <summary>
    /// ��֧��������
    /// </summary>
    public int branch_belong { get; set; }
    /// <summary>
    /// �Ƿ�ֱ�ӽ���
    /// </summary>
    public bool is_pre_unlock { get; set; }

    public MissionTree()
    {
        target = new List<int>();
        target_num = new Dictionary<int, float>();
        complete_num = new Dictionary<int, float>();
        award = new List<string>();
        award_num = new List<float>();
        unlock_mission = new List<int>();
        target_describe = new List<string>();
        branch = new List<int>();
    }
}
