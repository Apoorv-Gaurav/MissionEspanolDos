
using UnityEngine;

public class L02_BodyPartTarget : MonoBehaviour
{
    public string targetId;
    public string displayName;

    public bool IsCorrectTarget(string id)
    {
        return targetId == id;
    }
}
