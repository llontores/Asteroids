using UnityEngine;

public class Portal : MonoBehaviour
{
    public Portal targetPortal;
    public Axis teleportAxis;

    private void OnTriggerExit2D(Collider2D other)
    {
        if (targetPortal == null) return;

        Vector3 pos = other.transform.position;

        if (teleportAxis == Axis.X)
            pos.x = targetPortal.transform.position.x;
        else
            pos.y = targetPortal.transform.position.y;

        other.transform.position = pos;
    }
}