using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static class RunManager
{
    private static RunData _currentData;

    public static RunData CurrentRun => _currentData;

    private static Dictionary<string, ScoreBoardInfo> ScoreBoardConfig;

    public static class Names
    {
        // scoreboard
        public const string FloorReached = "Floor Reached";
        public const string EnemiesKilled = "Enemies Killed";
        public const string GoldEarned = "Gold Earned";
        public const string CharLevel = "Character Level";
        public const string XPEarned = "XP Earned";


        // optional



        // fluff
        public const string DoorsOpen = "Doors Opened";
        public const string DoorsUnlocked = "Doors Unlocked";

        public const string TilesMoved = "Tiles Moved";
        public const string EnemiesAttacked = "Enemies Attacked";

        public const string ItemsThrown = "Items Thrown";
    }



    private const int OPTIONAL_COUNT = 4;
    private const int FLUFF_COUNT = 2;

    private const int LEFT_PAD = 5;

    public static void StartNewRun()
    {
        _currentData = new RunData();
        ScoreBoardConfig = new Dictionary<string, ScoreBoardInfo>();
        InitValues();
    }
    

    private static void InitEntry(string name, object startValue, RunData.EntryType type,System.Func<object,int> weight = null,System.Func<object,string> format = null)
    {
        ScoreBoardInfo info = new ScoreBoardInfo(type, format ?? ((o) => o.ToString()), weight ?? ((_) => 10));
        ScoreBoardConfig.Add(name, info);
        CurrentRun.Set(name, startValue);
    }

    private static void InitValues()
    {

        InitEntry(Names.FloorReached, 0, RunData.EntryType.Scoreboard);
        InitEntry(Names.EnemiesKilled, 0, RunData.EntryType.Scoreboard);
        InitEntry(Names.CharLevel, 0, RunData.EntryType.Scoreboard);
        // InitEntry(Names.GoldEarned, 0, RunData.EntryType.Scoreboard);
        InitEntry(Names.XPEarned, 0, RunData.EntryType.Scoreboard);


        InitEntry(Names.DoorsOpen, 0, RunData.EntryType.Fluff, (o) => ((int)o) > 0 ? 10 : 0);
        InitEntry(Names.DoorsUnlocked, 0, RunData.EntryType.Fluff, (o) => ((int)o) > 0 ? 10 : 0);
        InitEntry(Names.TilesMoved, 0, RunData.EntryType.Fluff,(o)=>((int)o)>0 ? 10:0 );
        InitEntry(Names.EnemiesAttacked, 0, RunData.EntryType.Fluff, (o) => ((int)o) > 0 ? 10 : 0);
        InitEntry(Names.ItemsThrown, 0, RunData.EntryType.Fluff,(o)=>((int)o)>0?10:0);
    }

    public static string GetResultScreen()
    {
        if (_currentData == null)
        {
            return "Results";
        }

        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < 3; i++)
        {
            sb.AppendLine();
        }

        sb.AppendLine("Run Results".PadLeft(35));
        for (int i = 0; i < 5; i++)
        {
            sb.AppendLine();
        }

        int count = 0;
        int entryWidth = 25;
        int space = 4;


        List<KeyValuePair<string, ScoreBoardInfo>> scoreboard = new List<KeyValuePair<string, ScoreBoardInfo>>();
        int scoreboardSum = 0;
        List<KeyValuePair<string, ScoreBoardInfo>> optional = new List<KeyValuePair<string, ScoreBoardInfo>>();
        int optionalSum = 0;
        List<KeyValuePair<string, ScoreBoardInfo>> fluff = new List<KeyValuePair<string, ScoreBoardInfo>>();
        int fluffSum = 0;

        foreach (var item in ScoreBoardConfig)
        {
            int value = item.Value.Weighting(CurrentRun.Get(item.Key));
            switch (item.Value.Type)
            {
                case RunData.EntryType.Scoreboard:
                    if (value > 0)
                    {
                        scoreboardSum += value;
                        scoreboard.Add(item);
                    }
                    break;
                case RunData.EntryType.Optional:
                    if (value > 0)
                    {
                        optionalSum += value;
                        optional.Add(item);
                    }
                    break;
                case RunData.EntryType.Fluff:
                    if (value > 0)
                    {
                        fluffSum += value;
                        fluff.Add(item);
                    }
                    break;
            }
        }

        // Scoreboard
        foreach (KeyValuePair<string, ScoreBoardInfo> item in scoreboard)
        {            
            if (count%2 == 0)
            {
                sb.Append("".PadLeft(LEFT_PAD));
                sb.Append(item.Key.PadRight(entryWidth - item.Value.Formatter(CurrentRun.Get(item.Key)).Length) + item.Value.Formatter(CurrentRun.Get(item.Key)));
                sb.Append("".PadRight(space));
            }
            else
            {
                sb.Append(item.Key.PadRight(entryWidth - item.Value.Formatter(CurrentRun.Get(item.Key)).Length) + item.Value.Formatter(CurrentRun.Get(item.Key)));
                sb.AppendLine();
            }
            count++;
        }
        if (count % 2 == 1)
        {
            sb.AppendLine("".PadRight(entryWidth));
            count++;
        }
        //sb.AppendLine();

        // optional
        for (int i = 0; i < OPTIONAL_COUNT; i++)
        {
            // get random 
            if (optional.Count==0)
            {
                break;
            }
            int rValue = RNG.Range(0, optionalSum);
            KeyValuePair<string, ScoreBoardInfo> current = new KeyValuePair<string, ScoreBoardInfo>();

            foreach (var item in optional)
            {
                if (rValue<=item.Value.Weighting(CurrentRun.Get(item.Key)))
                {
                    // we found it 
                    current = item;
                    break;
                }
                else
                {
                    rValue = rValue - item.Value.Weighting(CurrentRun.Get(item.Key));
                }
            }
            optional.Remove(current);
            fluffSum -= current.Value.Weighting(CurrentRun.Get(current.Key));


            if (count % 2 == 0)
            {
                sb.Append("".PadLeft(LEFT_PAD));
                sb.Append(current.Key.PadRight(entryWidth - current.Value.Formatter(CurrentRun.Get(current.Key)).Length) + current.Value.Formatter(CurrentRun.Get(current.Key)));
                sb.Append("".PadRight(space));
            }
            else
            {
                sb.Append(current.Key.PadRight(entryWidth - current.Value.Formatter(CurrentRun.Get(current.Key)).Length) + current.Value.Formatter(CurrentRun.Get(current.Key)));
                sb.AppendLine();
            }
            count++;
        }
        if (count % 2 == 1)
        {
            sb.AppendLine();
            count++;
        }

        sb.AppendLine();

        // Fluff

        for (int i = 0; i < FLUFF_COUNT; i++)
        {
            // get random 
            if (fluff.Count == 0)
            {
                break;
            }
            int rValue = RNG.Range(0, fluffSum);
            KeyValuePair<string, ScoreBoardInfo> current = new KeyValuePair<string, ScoreBoardInfo>();

            foreach (var item in fluff)
            {
                if (rValue <= item.Value.Weighting(CurrentRun.Get(item.Key)))
                {
                    // we found it 
                    current = item;
                    break;
                }
                else
                {
                    rValue = rValue - item.Value.Weighting(CurrentRun.Get(item.Key));
                }
            }
            fluff.Remove(current);
            fluffSum -= current.Value.Weighting(CurrentRun.Get(current.Key));

            if (count % 2 == 0)
            {
                sb.Append("".PadLeft(LEFT_PAD));
                sb.Append(current.Key.PadRight(entryWidth - current.Value.Formatter(CurrentRun.Get(current.Key)).Length) + current.Value.Formatter(CurrentRun.Get(current.Key)));
                sb.Append("".PadRight(space));
            }
            else
            {
                sb.Append(current.Key.PadRight(entryWidth - current.Value.Formatter(CurrentRun.Get(current.Key)).Length) + current.Value.Formatter(CurrentRun.Get(current.Key)));
                sb.AppendLine();
            }
            count++;
        }
        if (count % 2 == 1)
        {
            sb.AppendLine();
            count++;
        }
        return sb.ToString();
    }

}
