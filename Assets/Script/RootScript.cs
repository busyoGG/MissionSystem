using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class RootScript : MonoBehaviour
{
    public MissionSaveDataSO Save;

    public MissionScrollView List;
    void Start()
    {
        MissionManager.Instance().LoadSaveData(Save);
        MissionManager.Instance().Init();

        MissionManager.Instance().RefreshPreCountNum(0,3);

        List<string> subData = new List<string>();
        List<MissionTree> data = new List<MissionTree>();
        Dictionary<int, MissionTree> unlockedMission = MissionManager.Instance().GetUnlockedMission();

        Dictionary<MissionFilter, bool> title = new Dictionary<MissionFilter, bool>();
        foreach (var tree in unlockedMission)
        {
            if (tree.Value.filter != MissionFilter.Branch)
            {
                data.Add(tree.Value);
                if (!title.ContainsKey(tree.Value.filter))
                {
                    title.Add(tree.Value.filter, true);
                    subData.Add(MissionManager.Instance().GetMissionFilterString(tree.Value.filter));
                }
            }
        }

        //for (int i = 0, len = 15; i < len; i++)
        //{
        //    data.Add(new MissionTree());
        //}

        //for(int i = 0,len = 2; i < len; i++)
        //{
        //    subData.Add("test");
        //}

        List._subData = subData;
        List.data = data;

        InitEventListener();
    }

    private void InitEventListener()
    {
        EventManager.AddListening("root", "refresh_mission_list", (ArrayList data) =>
        {
            List<string> subData = new List<string>();
            List<MissionTree> missions = new List<MissionTree>();
            Dictionary<int, MissionTree> unlockedMission = MissionManager.Instance().GetUnlockedMission();

            Dictionary<MissionFilter, bool> title = new Dictionary<MissionFilter, bool>();
            int subNum = 0;
            foreach (var tree in unlockedMission)
            {
                if (tree.Value.filter != MissionFilter.Branch)
                {
                    missions.Add(tree.Value);
                    if (!title.ContainsKey(tree.Value.filter))
                    {
                        title.Add(tree.Value.filter, true);
                        //subData.Add(MissionManager.Instance().GetMissionFilterString(tree.Value.filter));
                        if(subNum < (int)tree.Value.filter)
                        {
                            subNum = (int)tree.Value.filter;
                        }
                    }
                }
            }
            for(int i = 0; i < subNum + 1; i++) {
                subData.Add(MissionManager.Instance().GetMissionFilterString((MissionFilter)i));
            }

            List._subData = subData;
            List.data = missions;

            Debug.Log("刷新任务列表");
        });
    }
}
