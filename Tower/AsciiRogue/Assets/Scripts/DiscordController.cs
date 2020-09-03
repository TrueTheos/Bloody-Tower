using System;
using UnityEngine;

public class DiscordController : MonoBehaviour
{
    /*private Discord.Discord _discord;

    private void Start()
    {
        _discord = new Discord.Discord(700002566963986482, (UInt64)Discord.CreateFlags.Default);

        var activity = new Discord.Activity
        {
            State = "In Play Mode",
            Details = "Developing",
            Instance = true,
        };

        _discord.GetActivityManager().UpdateActivity(activity, (result) =>
        {
            if (result == Discord.Result.Ok)
            {
                Console.WriteLine("Success!");
            }
            else
            {
                Console.WriteLine("Failed");
            }
        });
    }

    private void Update()
    {
        _discord.RunCallbacks();
    }

    private void OnApplicationQuit()
    {
        _discord.Dispose();
    }*/
}