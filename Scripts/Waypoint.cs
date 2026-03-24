using UnityEngine;

public class Waypoint : MonoBehaviour
{
    // 次に行ける場所リスト
    public Waypoint[] nextPoints;

    private void OnDrawGizmos()
    {
        if (nextPoints == null) return;
        Gizmos.color = Color.cyan;
        foreach (var point in nextPoints)
        {
            if (point != null)
            {
                Gizmos.DrawLine(transform.position, point.transform.position);
                Gizmos.DrawSphere(point.transform.position, 0.5f);
            }
        }
    }
}