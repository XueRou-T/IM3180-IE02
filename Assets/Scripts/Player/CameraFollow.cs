using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 5f;
    public Vector3 offset = new Vector3(0f, 0f, -10f);

    [Header("Map Bounds (camera won't show beyond this)")]
    public bool useBounds = false;
    public Vector2 minBounds = new Vector2(-10f, -10f);
    public Vector2 maxBounds = new Vector2(10f, 10f);

    private Camera cam;

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        cam = GetComponent<Camera>();
        FindPlayer();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindPlayer();
    }

    void FindPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            target = player.transform;
        }
        else
        {
            Debug.LogWarning("CameraFollow: No GameObject with tag 'Player' found in the new scene!");
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;

        if (useBounds)
        {
            desiredPosition = ClampToBounds(desiredPosition);
        }

        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
    }

    Vector3 ClampToBounds(Vector3 pos)
    {
        if (cam == null) cam = GetComponent<Camera>();

        // Half of what the camera can see, in world units
        float halfHeight = cam.orthographicSize;
        float halfWidth = halfHeight * cam.aspect;

        float minX = minBounds.x + halfWidth;
        float maxX = maxBounds.x - halfWidth;
        float minY = minBounds.y + halfHeight;
        float maxY = maxBounds.y - halfHeight;

        // If the map is smaller than the camera view on an axis, just center on the map
        pos.x = (minX > maxX) ? (minBounds.x + maxBounds.x) / 2f : Mathf.Clamp(pos.x, minX, maxX);
        pos.y = (minY > maxY) ? (minBounds.y + maxBounds.y) / 2f : Mathf.Clamp(pos.y, minY, maxY);

        return pos;
    }

    // Draws the bounds as a yellow box in the Scene view when the camera is selected
    void OnDrawGizmosSelected()
    {
        if (!useBounds) return;

        Gizmos.color = Color.yellow;
        Vector2 center = (minBounds + maxBounds) / 2f;
        Vector2 size = maxBounds - minBounds;
        Gizmos.DrawWireCube(new Vector3(center.x, center.y, 0f), new Vector3(size.x, size.y, 0f));
    }
}