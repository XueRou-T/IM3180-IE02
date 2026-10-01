using UnityEngine;

[System.Serializable]
public class HelpPage
{
    public string title;

    [TextArea(3,8)]
    public string description;

    public Sprite image;
}
