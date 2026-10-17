
using UnityEngine;

public class L1_SearchTarget : MonoBehaviour
{
    public string targetId;
    public string displayName;

    public L1_SearchGameManager gameManager;

    void OnMouseDown()
    {
        if (gameManager != null)
        {
            gameManager.CheckTarget(this);
        }
    }
}
