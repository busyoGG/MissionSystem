using System.Collections.Generic;
using UnityEngine;

public class MissionTree
{
    /// <summary>
    /// 任务步骤id
    /// </summary>
    public int id { get; set; }
    /// <summary>
    /// 任务id
    /// </summary>
    public int m_id { get; set; }
    /// <summary>
    /// 任务标题
    /// </summary>
    public string title { get; set; }
    /// <summary>
    /// 任务描述
    /// </summary>
    public string describe { get; set; }
    /// <summary>
    /// 任务类型
    /// </summary>
    public MissionType type { get; set; }
    /// <summary>
    /// 任务类别
    /// </summary>
    public MissionFilter filter { get; set; }
    /// <summary>
    /// 任务目标
    /// </summary>
    public List<int> target { get; set; }
    /// <summary>
    /// 任务目标描述
    /// </summary>
    public List<string> target_describe { get; set; }
    /// <summary>
    /// 任务目标数量
    /// </summary>
    public Dictionary<int, float> target_num { get; set; }
    /// <summary>
    /// 任务完成数量
    /// </summary>
    public Dictionary<int, float> complete_num { get; set; }
    /// <summary>
    /// 任务奖励
    /// </summary>
    public List<string> award { get; set; }
    /// <summary>
    /// 任务奖励数量
    /// </summary>
    public List<float> award_num { get; set; }
    /// <summary>
    /// 是否预统计
    /// </summary>
    public bool is_pre_count { get; set; }
    /// <summary>
    /// 下一任务
    /// </summary>
    public MissionTree next { get; set; }
    /// <summary>
    /// 任务解锁列表
    /// </summary>
    public List<int> unlock_mission { get; set; }
    /// <summary>
    /// 任务分支
    /// </summary>
    public List<int> branch { get; set; }
    /// <summary>
    /// 分支所属任务
    /// </summary>
    public int branch_belong { get; set; }
    /// <summary>
    /// 是否直接解锁
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
