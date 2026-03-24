using UnityEngine;

public class RouteConnector : MonoBehaviour
{
    public Waypoint[] allPoints; 

    [ContextMenu("★つなぐ★")]
    public void ConnectAll()
    {
        for (int i = 0; i < allPoints.Length; i++)
        {
            allPoints[i].nextPoints = new Waypoint[1];
            if (i == allPoints.Length - 1) allPoints[i].nextPoints[0] = allPoints[0];
            else allPoints[i].nextPoints[0] = allPoints[i + 1];
        }
        Debug.Log("接続完了！");
    }
}