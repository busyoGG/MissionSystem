using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public enum MissionType
{
    Find,
    Collection,
    Kill
}

public enum MissionFilter
{
    Main,
    Sub,
    Branch//这个类型一定放最后 因为该类型不参与计算，枚举数不能影响计算
}

/// <summary>
/// 任务管理器
/// <para>请在预统计物品数量（即背包、杀人数等）加载之前加载本类，并且在预统计物品数量加载时执行RefreshPreCountNum方法</para>
/// </summary>
public class MissionManager
{
    private static MissionManager instance;

    private Dictionary<int, MissionTree> _lockedMission = new Dictionary<int, MissionTree>();

    private Dictionary<int, MissionTree> _unlockedMission = new Dictionary<int, MissionTree>();

    private Dictionary<int, MissionTree> _doneMission = new Dictionary<int, MissionTree>();

    private Dictionary<int, MissionSaveData> _missionSaveData = new Dictionary<int, MissionSaveData>();

    private Dictionary<int, float> _preCount = new Dictionary<int, float>();

    private MissionTree _curMission;

    public static MissionManager Instance()
    {
        if (instance == null)
        {
            instance = new MissionManager();
        }
        return instance;
    }

    private MissionManager() { }

    /// <summary>
    /// 加载存档
    /// </summary>
    /// <param name="save"></param>
    public void LoadSaveData(MissionSaveDataSO save)
    {
        for (int i = 0, len = save._missionSaveDatas.Count; i < len; i++)
        {
            MissionSaveData missionSaveData = save._missionSaveDatas[i];
            _missionSaveData.Add(missionSaveData.m_id, missionSaveData);
        }
    }

    public void Init()
    {
        //硬编码数据
        List<MissionTree> missions = new List<MissionTree>();

        MissionTree m1 = new MissionTree();
        m1.id = 0;
        m1.m_id = 0;
        m1.title = "任务1-1";
        m1.describe = "任务1-1的任务描述\n测试换行";
        m1.type = MissionType.Collection;
        m1.filter = MissionFilter.Main;
        m1.is_pre_count = true;
        m1.target.Add(0);
        m1.target_describe.Add("收集id=0的物品");
        m1.target_num.Add(0, 2);
        m1.award.Add("奖励1");
        m1.award_num.Add(1);
        m1.is_pre_unlock = true;

        MissionTree m2 = new MissionTree();
        m2.id = 1;
        m2.m_id = 0;
        m2.title = "任务1-2";
        m2.describe = "任务1-2的任务描述";
        m2.type = MissionType.Collection;
        m2.filter = MissionFilter.Main;
        m2.is_pre_count = false;
        m2.target.Add(0);
        m2.target_describe.Add("收集id=0的物品");
        m2.target_num.Add(0, 2);
        //m2.award.Add("奖励1");
        //m2.award_num.Add(1);

        m1.next = m2;

        MissionTree m3 = new MissionTree();
        m3.id = 0;
        m3.m_id = 1;
        m3.title = "任务2-1";
        m3.describe = "任务2-1的任务描述";
        m3.type = MissionType.Collection;
        m3.filter = MissionFilter.Sub;
        m3.is_pre_count = false;
        m3.target.Add(0);
        m3.target_describe.Add("收集id=0的物品");
        m3.target_num.Add(0, 2);
        m3.award.Add("奖励1");
        m3.award_num.Add(1);
        //m3.is_pre_unlock = true;


        MissionTree m4 = new MissionTree();
        m4.id = 0;
        m4.m_id = 2;
        m4.title = "任务3-1";
        m4.describe = "任务3-1的任务描述";
        m4.type = MissionType.Collection;
        m4.filter = MissionFilter.Branch;
        m4.is_pre_count = false;
        m4.target.Add(0);
        m4.target_describe.Add("收集id=0的物品");
        m4.target_num.Add(0, 2);
        m4.award.Add("奖励1");
        m4.award_num.Add(1);
        m4.is_pre_unlock = true;
        m4.branch_belong = 0;

        MissionTree m5 = new MissionTree();
        m5.id = 0;
        m5.m_id = 3;
        m5.title = "任务4-1";
        m5.describe = "任务4-1的任务描述";
        m5.type = MissionType.Collection;
        m5.filter = MissionFilter.Branch;
        m5.is_pre_count = false;
        m5.target.Add(0);
        m5.target_describe.Add("收集id=0的物品");
        m5.target_num.Add(0, 2);
        m5.award.Add("奖励1");
        m5.award_num.Add(1);
        m5.is_pre_unlock = true;
        m5.branch_belong = 0;

        m1.unlock_mission.Add(1);
        m2.branch.Add(2);
        m2.branch.Add(3);

        missions.Add(m1);
        missions.Add(m3);
        missions.Add(m4);
        missions.Add(m5);

        //初始化
        for (int i = 0, len = missions.Count; i < len; i++)
        {
            MissionTree mission = missions[i];
            MissionSaveData missionSaveData;

            _missionSaveData.TryGetValue(mission.m_id, out missionSaveData);

            if (missionSaveData != null)
            {
                if (missionSaveData.isDone)
                {
                    _doneMission.Add(mission.m_id, mission);
                }
                else
                {
                    while (mission.id != missionSaveData.id)
                    {
                        mission = mission.next;
                    }
                    _unlockedMission.Add(mission.m_id, mission);
                    mission.complete_num = missionSaveData.complete_num;
                }
            }
            else
            {
                if (mission.is_pre_unlock)
                {
                    _unlockedMission.Add(mission.m_id, mission);
                }
                else
                {
                    _lockedMission.Add(mission.m_id, mission);
                }
            }
        }
    }

    public MissionTree GetCurMission()
    {
        return _curMission;
    }

    public Dictionary<int, MissionTree> GetUnlockedMission()
    {
        return _unlockedMission;
    }

    public Dictionary<int, MissionTree> GetDoneMission()
    {
        return _doneMission;
    }

    public MissionTree GetUnlockedMissionById(int id)
    {
        MissionTree missionTree;
        _unlockedMission.TryGetValue(id, out missionTree);
        return missionTree;
    }

    /// <summary>
    /// 获得任务筛选类型文字描述
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public string GetMissionFilterString(MissionFilter type)
    {
        switch (type)
        {
            case MissionFilter.Main:
            default:
                return "主线";
            case MissionFilter.Sub:
                return "支线";
        }
    }

    /// <summary>
    /// 获得任务筛选类型
    /// </summary>
    /// <param name="filter"></param>
    /// <returns></returns>
    public MissionFilter GetMissionFileter(string filter)
    {
        switch (filter)
        {
            case "主线":
            default:
                return MissionFilter.Main;
            case "支线":
                return MissionFilter.Sub;
        }
    }

    /// <summary>
    /// 刷新预统计类型任务的完成数量
    /// </summary>
    /// <param name="targetId"></param>
    /// <param name="num"></param>
    public void RefreshPreCountNum(int targetId, float num, bool isAdd = false)
    {
        if (!_preCount.ContainsKey(targetId))
        {
            _preCount.Add(targetId, 0);
        }
        if (isAdd)
        {
            _preCount[targetId] += num;
        }
        else
        {
            _preCount[targetId] = num;
        }
    }

    /// <summary>
    /// 获得预统计数量
    /// </summary>
    /// <param name="targetId"></param>
    /// <returns></returns>
    public float GetPreCountNum(int targetId)
    {
        return _preCount[targetId];
    }

    /// <summary>
    /// 设置任务完成数量
    /// </summary>
    /// <param name="missionId"></param>
    /// <param name="completeNum"></param>
    /// <param name="isAdd"></param>
    public void SetCompleteNum(int missionId, int targetId, float completeNum, bool isAdd = true)
    {
        MissionTree mission;
        _unlockedMission.TryGetValue(missionId, out mission);
        if (mission != null)
        {
            if (!mission.complete_num.ContainsKey(targetId))
            {
                mission.complete_num.Add(targetId, 0);
            }
            if (isAdd)
            {
                mission.complete_num[targetId] += completeNum;
            }
            else
            {
                mission.complete_num[targetId] = completeNum;
            }
            //保存数据
            SaveMissionData(missionId, mission.id, mission.complete_num, false);
        }
    }

    /// <summary>
    /// 检测任务是否完成
    /// </summary>
    /// <param name="missionId"></param>
    /// <returns></returns>
    public bool CheckComplete(MissionTree mission)
    {
        for (int i = 0, len = mission.target.Count; i < len; i++)
        {
            int target = mission.target[i];
            if (mission.is_pre_count)
            {
                if (mission.target_num[target] > _preCount[target])
                {
                    return false;
                }
            }
            else
            {
                if (mission.complete_num.Count == 0 || mission.target_num[target] > mission.complete_num[target])
                {
                    return false;
                }
            }
        }
        return true;
    }

    public void Next(MissionTree mission)
    {
        //MissionTree mission = _unlockedMission[missionId];
        int missionId = mission.m_id;
        MissionTree next = mission.next;
        if (next != null)
        {
            if (mission.unlock_mission.Count > 0)
            {
                for (int i = 0, len = mission.unlock_mission.Count; i < len; i++)
                {
                    int id = mission.unlock_mission[i];
                    MissionTree unlockMission = _lockedMission[id];
                    _unlockedMission.Add(unlockMission.m_id, unlockMission);
                    _lockedMission.Remove(id);
                    //保存数据
                    SaveMissionData(id, unlockMission.id, unlockMission.complete_num, false);
                }
            }
            //下一任务
            _unlockedMission[missionId] = next;
            //保存数据
            SaveMissionData(missionId, next.id, next.complete_num, false);
        }
        else
        {
            _unlockedMission.Remove(missionId);
            _doneMission.Add(missionId, mission);
            //保存数据
            SaveMissionData(missionId, mission.id, mission.complete_num, true);
        }

        //获得奖励
        GetAward(mission);

        if (mission.filter == MissionFilter.Branch)
        {
            MissionTree branchMissionRoot = GetUnlockedMissionById(mission.branch_belong);
            bool missionDone = true;
            foreach (var id in branchMissionRoot.branch)
            {
                MissionTree branch = GetUnlockedMissionById(id);
                if (branch != null)
                {
                    missionDone = false;
                    break;
                }
            }
            if (missionDone)
            {
                Next(branchMissionRoot);
            }
        }
    }

    private void GetAward(MissionTree mission)
    {
        string award = "";
        for (int i = 0, len = mission.award.Count; i < len; i++)
        {
            award += mission.award[i] + " 数量:" + mission.award_num[i];
        }
        Debug.Log("获得任务奖励 ===> " + award);
    }

    private void SaveMissionData(int missionId, int stepId, Dictionary<int, float> completeNum, bool isDone)
    {

        //保存数据
        MissionSaveData missionSaveData;
        _missionSaveData.TryGetValue(missionId, out missionSaveData);
        if (missionSaveData == null)
        {
            missionSaveData = new MissionSaveData();
            missionSaveData.m_id = missionId;
            _missionSaveData.Add(missionId, missionSaveData);
        }

        missionSaveData.id = stepId;
        missionSaveData.complete_num = completeNum;
        missionSaveData.isDone = isDone;
    }
}
