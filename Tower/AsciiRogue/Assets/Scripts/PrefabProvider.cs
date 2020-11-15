using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PrefabProvider 
{
    public static class Rooms
    {
        public const string PREFAB_ROOM_PATH = "PrefabRooms/";

        public static bool Loaded { private set; get; } = false;


        private static HashSet<PrefabRoom> _loadedRooms;


        private static Dictionary<Vector2Int, Dictionary<string, HashSet<PrefabRoom>>> _indexTable;

        private static Dictionary<Vector2Int, HashSet<PrefabRoom>> _sizeTable;


        static Rooms()
        {
            _loadedRooms = new HashSet<PrefabRoom>();
            _indexTable = new Dictionary<Vector2Int, Dictionary<string, HashSet<PrefabRoom>>>();
            _sizeTable = new Dictionary<Vector2Int, HashSet<PrefabRoom>>();
            LoadPrefabs();
        }


        public static void LoadPrefabs()
        {
            _loadedRooms.Clear();
            
            foreach (var item in _indexTable)
            {
                item.Value.Clear();
            }
            _indexTable.Clear();
            foreach (var room in Resources.LoadAll<PrefabRoom>(PREFAB_ROOM_PATH))
            {
                if (_loadedRooms.Add(room))
                {
                    Vector2Int size = new Vector2Int(room.width, room.height);

                    if (!_sizeTable.ContainsKey(size))
                    {
                        _sizeTable.Add(size, new HashSet<PrefabRoom>());
                    }
                    _sizeTable[size].Add(room);

                    if (!_indexTable.ContainsKey(size))
                    {
                        _indexTable.Add(size, new Dictionary<string, HashSet<PrefabRoom>>());
                    }

                    foreach (var tag in room.Tags)
                    {
                        if (!_indexTable[size].ContainsKey(tag))
                        {
                            _indexTable[size].Add(tag, new HashSet<PrefabRoom>());
                        }
                        _indexTable[size][tag].Add(room);
                    }
                }                
            }
            Loaded = true;
        }

        public static bool TryGetRoom(int width, int height, FloorGenData genData, string requiredTag, out PrefabRoom room)
        {
            if (_indexTable.ContainsKey(new Vector2Int(width,height)))
            {
                // there is a room of that size
                foreach (var r in _indexTable[new Vector2Int(width,height)])
                {                    
                    if (r.Key == requiredTag)
                    {
                        // we found the right tag
                        foreach (var opt in r.Value.AsArray().EnumerateRandom())
                        {
                            if (genData.PlacedRooms.Contains(opt))
                            {
                                continue;
                            }
                            bool valid = true;
                            foreach (var myTag in opt.Tags)
                            {
                                if (genData.ExcludedTags.ContainsKey(myTag))
                                {
                                    valid = false;
                                    break;
                                }
                            }
                            if (!valid) continue;
                            foreach (var exTag in opt.ExcludeTags)
                            {
                                if (genData.ExistingTags.ContainsKey(exTag))
                                {
                                    valid = false;
                                    break;
                                }
                            }
                            if (valid)
                            {
                                room = opt;
                                return true;
                            }
                        }
                    }
                }
            }



            room = null;
            return false;
        }

        public static bool TryGetRoom(int width, int height, FloorGenData genData, out PrefabRoom room)
        {
            if (_sizeTable.ContainsKey(new Vector2Int(width, height)))
            {
                // there is a room of that size
                foreach (var r in _sizeTable[new Vector2Int(width, height)].AsArray().EnumerateRandom())
                {
                    if (genData.PlacedRooms.Contains(r))
                    {
                        continue;
                    }
                    // we found the right tag

                    bool valid = true;
                    foreach (var myTag in r.Tags)
                    {
                        if (genData.ExcludedTags.ContainsKey(myTag))
                        {
                            valid = false;
                            break;
                        }
                    }
                    if (!valid) continue;
                    foreach (var exTag in r.ExcludeTags)
                    {
                        if (genData.ExistingTags.ContainsKey(exTag))
                        {
                            valid = false;
                            break;
                        }
                    }
                    if (valid)
                    {
                        room = r;
                        return true;
                    }


                }
            }

            room = null;
            return false;
        }



    }
}
