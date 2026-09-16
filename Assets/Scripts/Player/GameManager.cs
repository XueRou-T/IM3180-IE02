using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // global access point

    // Inventory: names of items collected, e.g. "PersonalID", "WorkID", "Wallet", "Key"
    private List<string> collectedItems = new List<string>();

    // Notes/clues collected as text, e.g. security question answers found in other scenes
    private List<string> collectedNotes = new List<string>();

    void Awake()
    {
        // Singleton pattern: only one GameManager should ever exist
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // survives scene loads
        }
        else
        {
            Destroy(gameObject); // if one already exists, destroy this duplicate
        }
    }

    public void AddItem(string itemName)
    {
        if (!collectedItems.Contains(itemName))
        {
            collectedItems.Add(itemName);
            Debug.Log("Collected: " + itemName);
        }
    }

    public bool HasItem(string itemName)
    {
        return collectedItems.Contains(itemName);
    }

    public bool HasAllItems(List<string> itemNames)
    {
        foreach (string item in itemNames)
        {
            if (!collectedItems.Contains(item)) return false;
        }
        return true;
    }

    public void AddNote(string noteText)
    {
        if (!collectedNotes.Contains(noteText))
        {
            collectedNotes.Add(noteText);
            Debug.Log("Note added: " + noteText);
        }
    }

    public List<string> GetAllItems()
    {
        return collectedItems;
    }

    public List<string> GetAllNotes()
    {
        return collectedNotes;
    }
}