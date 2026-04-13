using UnityEngine;

public class DroneCamera : MonoBehaviour
{
    [Header("Start地点")]
    public Waypoint currentTarget; 

    [Header("ドローンの設定")]
    public float speed = 50.0f; 
    public float rotationSpeed = 5.0f; 

    void Start()
    {
        
        if (currentTarget != null)
        {
            transform.position = currentTarget.transform.position;
        }
    }

    void Update()
    {
        if (currentTarget == null) return;

       
        transform.position = Vector3.MoveTowards(transform.position, currentTarget.transform.position, speed * Time.deltaTime);

       
        Vector3 direction = currentTarget.transform.position - transform.position;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
        }

        
        float distanceToTarget = Vector3.Distance(transform.position, currentTarget.transform.position);
        
        
        if (distanceToTarget <= speed * Time.deltaTime * 1.5f) 
        {
            
            if (currentTarget.nextPoints != null && currentTarget.nextPoints.Length > 0)
            {
               
                transform.position = currentTarget.transform.position;

                
                int dice = Random.Range(0, currentTarget.nextPoints.Length);
                currentTarget = currentTarget.nextPoints[dice];
            }
        }
    }
}
