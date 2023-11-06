using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemTransformData
{
    public Vector2 pos { get; set; }

    public float height { get; set; }

    public float width { get; set; }

    public int cell_index { get; set; }

    public int item_index { get; set; }

    public Button item { get; set; }

    public int item_type { get; set; }

    public ItemTransformData next { get; set; }

    public ItemTransformData parent { get; set; }

    public void CloneTo(ItemTransformData itd)
    {
        itd.pos = pos;
        itd.height = height;
        itd.width = width;
        itd.cell_index = cell_index;
        itd.item_index = item_index;
        itd.item = item;
        itd.item_type = item_type;
    }

    public ItemTransformData Clone()
    {
        ItemTransformData itd = new ItemTransformData();
        CloneTo(itd);
        return itd;
    }
}
