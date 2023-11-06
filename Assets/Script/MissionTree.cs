using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionTree
{
    public int id { get; set; }

    public int m_id { get; set; }

    public string title { get; set; }

    public string describe { get; set; }

    public MissionType type { get; set; }

    public MissionFilter filter { get; set; }

    public List<int> target { get; set; }
    public List<string> target_describe { get; set; }

    public Dictionary<int, float> target_num { get; set; }

    public Dictionary<int, float> complete_num { get; set; }

    public List<string> award { get; set; }

    public List<float> award_num { get; set; }

    public bool is_pre_count { get; set; }

    public GameObject chasing_obj { get; set; }

    public float limitTime { get; set; }

    public MissionTree next { get; set; }

    public List<int> unlock_mission { get; set; }

    public List<int> branch { get; set; }

    public int branch_belong { get; set; }

    public bool is_pre_unlock { get; set; }

    //等级限制等暂时不做

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
