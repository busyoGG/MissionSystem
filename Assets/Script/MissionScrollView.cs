using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MissionScrollView : ScrollViewScript<MissionTree>
{
    public List<string> _subData;

    void Start()
    {
        base.Start();
        actions.Add(UpdateItemDataMission);
        actions.Add(UpdateItemDataType);
        actions.Add(UpdateItemDataBranch);
    }

    protected override void InitItemType()
    {
        List<int[]> type = new List<int[]>();

        List<List<int[]>> missionBucket = new List<List<int[]>>();

        for (int i = 0, len = _subData.Count; i < len; i++)
        {
            missionBucket.Add(new List<int[]>());
        }

        for (int i = 0, len = data.Count; i < len; i++)
        {
            if (data[i].branch.Count == 0)
            {
                missionBucket[(int)data[i].filter].Add(new int[] { 0, i });
            }
            else
            {
                bool missionDone = true;
                foreach (var id in data[i].branch)
                {
                    MissionTree mission = MissionManager.Instance().GetUnlockedMissionById(id);
                    if (mission != null)
                    {
                        missionDone = false;
                        break;
                    }
                }

                if (!missionDone)
                {
                    missionBucket[(int)data[i].filter].Add(new int[] { 2, i });
                }
            }
        }

        for (int i = 0, len = missionBucket.Count; i < len; i++)
        {
            int len2 = missionBucket[i].Count;
            if (len2 > 0)
            {
                type.Add(new int[] { 1, i });
            }
            for (int j = 0; j < len2; j++)
            {
                type.Add(missionBucket[i][j]);
            }
        }

        itemType = type;
    }

    protected void UpdateItemDataMission(ItemTransformData cell, int index)
    {
        TextMeshProUGUI content = cell.item.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        content.text = data[index].title;

        cell.item.onClick.RemoveAllListeners();
        cell.item.onClick.AddListener(() =>
        {
            EventManager.TriggerEventSticky("refresh_mission_panel", new ArrayList { data[index] });
            Debug.Log("展开任务面板");
        });
    }

    protected void UpdateItemDataType(ItemTransformData cell, int index)
    {
        TextMeshProUGUI content = cell.item.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        content.text = _subData[index];

        cell.item.onClick.RemoveAllListeners();
        cell.item.onClick.AddListener(() =>
        {
            if (cell.height >= 90)
            {
                SetCellSize(cell, new Vector2(cell.width, cell.height / 3));
            }
            else
            {
                SetCellSize(cell, new Vector2(cell.width, cell.height * 3));
            }
            //EventManager.TriggerEventSticky("refresh_mission_panel", new ArrayList { data[index] });
        });
    }

    protected void UpdateItemDataBranch(ItemTransformData cell, int index)
    {
        TextMeshProUGUI content = cell.item.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        content.text = data[index].title;

        BranchScrollView bsv = cell.item.transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<BranchScrollView>();

        List<MissionTree> missionTrees = new List<MissionTree>();
        for (int i = 0, len = data[index].branch.Count; i < len; i++)
        {
            MissionTree mission = MissionManager.Instance().GetUnlockedMissionById(data[index].branch[i]);
            if (mission != null)
            {
                missionTrees.Add(mission);
            }
        }

        bsv.inputData = missionTrees;
        RectTransform bsvRect = bsv.Scroll.GetComponent<RectTransform>();
        //bsvRect.sizeDelta = new Vector2(bsv]Rect.rect.width, 0);
        if (cell.height >= bsvRect.rect.height + 50)
        {
            SetCellSize(cell, new Vector2(cell.width, 50 + bsvRect.rect.height),false);
            bsv.Scroll.transform.localPosition = new Vector2(10, bsv.Scroll.transform.localPosition.y);
        }
        else
        {
            bsv.Scroll.transform.localPosition = new Vector2(-1000, bsv.Scroll.transform.localPosition.y);
        }

        cell.item.onClick.RemoveAllListeners();
        cell.item.onClick.AddListener(() =>
        {
            if (cell.height >= bsvRect.rect.height + 50)
            {
                SetCellSize(cell, new Vector2(cell.width, 50));
                bsv.Scroll.transform.localPosition = new Vector2(-1000, bsv.Scroll.transform.localPosition.y);
            }
            else
            {
                SetCellSize(cell, new Vector2(cell.width, 50 + bsvRect.rect.height));
                bsv.Scroll.transform.localPosition = new Vector2(10, bsv.Scroll.transform.localPosition.y);
            }
        });
    }
}
