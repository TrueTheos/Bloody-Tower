using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "AI/Omni/TriggerTextAI")]
public class TriggerTextAI : BaseOmniAI
{
    public int Width;
    public int Height;

    [TextArea]
    [Tooltip("The order of sending message is top to bot")]
    public List<string> Message;

    public override void Calculate(OmniBehaviour t)
    {
        RectInt rec = new RectInt(t.Position.x, t.Position.y, Width, Height);

        if (rec.Contains(PlayerMovement.playerMovement.position))
        {
            for (int i = 0; i < Message.Count; i++)
            {            
                GameManager.manager.UpdateMessages(Message[i]);
                GameManager.manager.playerStats.__sanity -= 5;
            }
            GameManager.manager.enemies.Remove(t.gameObject);
            GameObject.Destroy(t.gameObject);
        }
    }
}
