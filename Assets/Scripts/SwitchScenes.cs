using UnityEngine;

public class CanvasLevelSwitcher : MonoBehaviour
{
    public GameObject[] canvases;

    public Transform teleportTarget;

    public string playerTag = "Player";

    public int targetCanvasIndex = 0;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(playerTag))
        {
            SwitchCanvas(collision.gameObject);
        }
    }

    private void SwitchCanvas(GameObject player)
    {
        if (canvases == null || canvases.Length == 0)
        {
            Debug.LogWarning("No canvases assigned in CanvasLevelSwitcher!");
            return;
        }

        if (targetCanvasIndex < 0 || targetCanvasIndex >= canvases.Length)
        {
            Debug.LogWarning("Invalid canvas index!");
            return;
        }

        foreach (GameObject canvas in canvases)
        {
            if (canvas != null)
                canvas.SetActive(false);
        }

        GameObject targetCanvas = canvases[targetCanvasIndex];
        targetCanvas.SetActive(true);

        if (teleportTarget != null)
        {
            player.transform.position = teleportTarget.position;
        }
        else
        {
            Debug.LogWarning("Teleport target not assigned!");
        }

        player.transform.SetParent(targetCanvas.transform, true);
    }
}
