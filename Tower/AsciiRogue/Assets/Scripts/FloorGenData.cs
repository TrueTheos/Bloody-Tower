using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorGenData
{

    public List<PrefabRoom> PlacedRooms = new List<PrefabRoom>();

    public int PrefabRoomCount => PlacedRooms.Count;
    
    public Dictionary<string, int> ExistingTags = new Dictionary<string, int>();
    public Dictionary<string, int> ExcludedTags = new Dictionary<string, int>();

    public List<TagSetCheck> RequiredChecks = new List<TagSetCheck>();

    public FloorGenData()
    {
    }

    public void AddCheck(TagSetCheck check)
    {
        RequiredChecks.Add(check);
    }

    public void AddRoom(PrefabRoom room)
    {
        PlacedRooms.Add(room);

        foreach (var myTag in room.Tags)
        {
            AddExistingTag(myTag);
        }
        foreach (var exTag in room.ExcludeTags)
        {
            AddExcludeTag(exTag);
        }

    }

    private void AddExcludeTag(string exTag)
    {
        if (!ExcludedTags.ContainsKey(exTag))
        {
            ExcludedTags.Add(exTag, 0);
        }
        ExcludedTags[exTag]++;
    }

    private void AddExistingTag(string myTag)
    {
        if (!ExistingTags.ContainsKey(myTag))
        {
            ExistingTags.Add(myTag, 0);
        }
        ExistingTags[myTag]++;
    }
}
