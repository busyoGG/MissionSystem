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
    Sub
}

/// <summary>
/// ���������
/// <para>����Ԥ������Ʒ��������������ɱ�����ȣ�����֮ǰ���ر��࣬������Ԥ������Ʒ��������ʱִ��RefreshPreCountNum����</para>
/// </summary>
public class MissionManager
{
    private static MissionManager instance;

    private Dictionary<int, MissionTree> _lockedMission = new Dictionary<int, MissionTree>();

    private Dictionary<int, MissionTree> _unlockedMission = new Dictionary<int, MissionTree>();

    private Dictionary<int, MissionTree> _doneMission = new Dictionary<int, MissionTree>();

    private Dictionary<int, MissionSaveData> _missionSaveData = new Dictionary<int, MissionSaveData>();

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
    /// ���ش浵
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
        //Ӳ��������
        List<MissionTree> missions = new List<MissionTree>();

        MissionTree m1 = new MissionTree();
        m1.id = 0;
        m1.m_id = 0;
        m1.title = "����1-1";
        m1.describe = "����1-1����������\n���Ի���";
        m1.type = MissionType.Collection;
        m1.filter = MissionFilter.Main;
        m1.is_pre_count = false;
        m1.target.Add(0);
        m1.target_describe.Add("�ռ�id=0����Ʒ");
        m1.target_num.Add(0, 2);
        m1.award.Add("����1");
        m1.award_num.Add(1);
        m1.is_pre_unlock = true;

        MissionTree m2 = new MissionTree();
        m2.id = 1;
        m2.m_id = 0;
        m2.title = "����1-2";
        m2.describe = "����1-2����������";
        m2.type = MissionType.Collection;
        m2.filter = MissionFilter.Main;
        m2.is_pre_count = false;
        m2.target.Add(0);
        m2.target_describe.Add("�ռ�id=0����Ʒ");
        m2.target_num.Add(0, 2);
        m2.award.Add("����1");
        m2.award_num.Add(1);

        m1.next = m2;

        MissionTree m3 = new MissionTree();
        m3.id = 0;
        m3.m_id = 1;
        m3.title = "����2-1";
        m3.describe = "����2-1����������";
        m3.type = MissionType.Collection;
        m3.filter = MissionFilter.Main;
        m3.is_pre_count = false;
        m3.target.Add(0);
        m3.target_describe.Add("�ռ�id=0����Ʒ");
        m3.target_num.Add(0, 2);
        m3.award.Add("����1");
        m3.award_num.Add(1);

        m1.unlock_mission.Add(1);

        missions.Add(m1);
        missions.Add(m3);

        //��ʼ��
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
    /// ˢ��Ԥ��������������������
    /// </summary>
    /// <param name="targetId"></param>
    /// <param name="num"></param>
    public void RefreshPreCountNum(int targetId, float num)
    {
        foreach (var item in _unlockedMission)
        {
            MissionTree mission = item.Value;
            if (mission.is_pre_count)
            {
                SetCompleteNum(mission.m_id, targetId, num, false);
            }
        }
    }

    /// <summary>
    /// ���������������
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
            //��������
            SaveMissionData(missionId, mission.id, mission.complete_num, false);
        }
    }

    /// <summary>
    /// ��������Ƿ����
    /// </summary>
    /// <param name="missionId"></param>
    /// <returns></returns>
    public bool CheckComplete(int missionId)
    {
        MissionTree mission = _unlockedMission[missionId];
        for (int i = 0, len = mission.target.Count; i < len; i++)
        {
            int target = mission.target[i];
            if (mission.complete_num.Count == 0 || mission.target_num[target] > mission.complete_num[target])
            {
                return false;
            }
        }
        return true;
    }

    public void Next(int missionId)
    {
        MissionTree mission = _unlockedMission[missionId];
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
                    //��������
                    SaveMissionData(id, unlockMission.id, unlockMission.complete_num, false);
                }
            }
            _unlockedMission[missionId] = next;
            //��������
            SaveMissionData(missionId, next.id, next.complete_num, false);
        }
        else
        {
            _unlockedMission.Remove(missionId);
            _doneMission.Add(missionId, mission);
            //��������
            SaveMissionData(missionId, mission.id, mission.complete_num, true);
        }
    }

    private void SaveMissionData(int missionId, int stepId, Dictionary<int, float> completeNum, bool isDone)
    {

        //��������
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