
using UnityEngine;

public class L1_SearchTarget : MonoBehaviour
{
    public string targetId;
    public string displayName;
    public L1_SearchGameManager gameManager;

    void OnMouseDown()
    {
        // The manager handles clicks centrally.
        // This method is intentionally left empty.
    }

    public void HandleClick()
    {
        if (gameManager != null)
        {
            gameManager.CheckTarget(this);
        }
    }
}
