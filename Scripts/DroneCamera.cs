using UnityEngine;

public class DroneCamera : MonoBehaviour
{
    [Header("最初に目指すポイント（Start地点）")]
    public Waypoint currentTarget; 

    [Header("ドローンの設定")]
    public float speed = 50.0f; // ★速くても大丈夫！
    public float rotationSpeed = 5.0f; // 向きを変える速さ

    void Start()
    {
        // ★ここが新機能！
        // 再生した瞬間、最初のポイントに強制ワープ！
        if (currentTarget != null)
        {
            transform.position = currentTarget.transform.position;
        }
    }

    void Update()
    {
        if (currentTarget == null) return;

        // --- 1. 移動する ---
        // ターゲットに向かって進む
        transform.position = Vector3.MoveTowards(transform.position, currentTarget.transform.position, speed * Time.deltaTime);

        // --- 2. 向きを変える ---
        // ターゲットの方を向く
        Vector3 direction = currentTarget.transform.position - transform.position;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
        }

        // --- 3. 次のポイントへ（脱線防止機能付き） ---
        // 「距離が近づいたら」ではなく「移動で到達するなら」判定に強化
        float distanceToTarget = Vector3.Distance(transform.position, currentTarget.transform.position);
        
        // もし「今のフレームの移動距離」よりも「残り距離」が短かったら（＝到着！）
        if (distanceToTarget <= speed * Time.deltaTime * 1.5f) // 少し余裕を持たせる
        {
            // 次の行き先があるか確認
            if (currentTarget.nextPoints != null && currentTarget.nextPoints.Length > 0)
            {
                // ズレを直すために、一旦きっちりポイントの上に置く
                transform.position = currentTarget.transform.position;

                // 次の場所をランダムに選ぶ
                int dice = Random.Range(0, currentTarget.nextPoints.Length);
                currentTarget = currentTarget.nextPoints[dice];
            }
        }
    }
}