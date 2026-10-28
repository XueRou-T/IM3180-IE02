using UnityEngine;

public class ExpandScript : MonoBehaviour
{
    public float growthRate = 0.5f;

    void Update()
    {
        transform.localScale += Vector3.one * growthRate * Time.deltaTime;
    }
}
