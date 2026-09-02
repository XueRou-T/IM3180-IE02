[System.Serializable]

public class GridData 
{
    public bool hasPlatform;

    public PuzzleButton button;
    public PuzzleDoor door;
    public PuzzleLadder ladder_Up;
    public PuzzleLadder ladder_Down;
    public PuzzleGoal goal;

    public bool isBlocked;
}
