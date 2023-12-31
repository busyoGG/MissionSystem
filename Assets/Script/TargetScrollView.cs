using System.Collections;
using TMPro;
using UnityEngine.UI;

public class TargetScrollView : ScrollViewScript<int>
{
    private void Start()
    {
        base.Start();
        actions.Add(UpdateItemData);
        EventManager.AddListening("target", "refresh_target", (ArrayList data) =>
        {
            RefreshList(this.data);
        });
    }
    protected void UpdateItemData(ItemTransformData cell, int index)
    {
        TextMeshProUGUI content = cell.item.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

        int id = (int)data[0];

        MissionTree mission = MissionManager.Instance().GetUnlockedMissionById(id);
        if (mission == null)
        {
            mission = MissionManager.Instance().GetUnlockedMissionById(id);
        }

        float compeleteNum;
        if (mission.is_pre_count)
        {
            compeleteNum = MissionManager.Instance().GetPreCountNum(mission.target[index]);
        }
        else
        {
            mission.complete_num.TryGetValue(mission.target[index], out compeleteNum);
        }

        if (compeleteNum > mission.target_num[index])
        {
            compeleteNum = mission.target_num[index];
        }

        content.text = mission.target_describe[index] + "(" + compeleteNum + "/" + mission.target_num[index] + ")";
    }
}
