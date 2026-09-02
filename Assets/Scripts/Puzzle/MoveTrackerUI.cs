using UnityEngine;

public class MoveTrackerUI : MonoBehaviour
{
    public PuzzleManager puzzleManager;
    public GameObject moveSlotPrefab;
    public Transform moveSlotContainer;

    private MoveSlotUI[] moveSlots;

    void Start()
    {
        CreateMoveSlots();
    }

    void CreateMoveSlots()
    {
        int moveCount = puzzleManager.shadowMoveCount;

        moveSlots = new MoveSlotUI[moveCount];

        for (int i = 0; i < moveCount; i++)
        {
            GameObject moveSlot = Instantiate(moveSlotPrefab, moveSlotContainer);

            moveSlots[i] = moveSlot.GetComponent<MoveSlotUI>();
        }
    }

    public void UpdateMoveSlots()
    {
        for (int i = 0; i < moveSlots.Length; i++)
        {
            if (i < puzzleManager.moveHistory.Count)
            {
                moveSlots[i].SetMove(puzzleManager.moveHistory[i]);
            }
            else
            {
                moveSlots[i].ClearMove();
            }
        }
    }
}