using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootScript : MonoBehaviour
{
    public MissionSaveDataSO Save;

    public MissionScrollView List;
    void Start()
    {
        MissionManager.Instance().LoadSaveData(Save);
        MissionManager.Instance().Init();
        
        List<MissionTree> data = new List<MissionTree> ();
        Dictionary<int,MissionTree> unlockedMission = MissionManager.Instance().GetUnlockedMission();

        foreach (var tree in unlockedMission)
        {
            data.Add (tree.Value);
        }

        List.data = data;

        InitEventListener();
    }

    private void InitEventListener()
    {
        EventManager.AddListening("root", "refresh_mission_list", (ArrayList data) =>
        {
            List<MissionTree> missions = new List<MissionTree>();
            Dictionary<int, MissionTree> unlockedMission = MissionManager.Instance().GetUnlockedMission();

            foreach (var tree in unlockedMission)
            {
                missions.Add(tree.Value);
            }

            List.data = missions;

            Debug.Log("刷新任务列表");
        });
    }
}
