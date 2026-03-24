using UnityEngine;
using DG.Tweening;

public class GrowthManager : MonoBehaviour
{
    [Header("基本設定")]
    // ★ここを変更！ [] をつけて「複数形」にしたよ
    public GameObject[] segmentPrefabs; 
    public float moveSpeed = 2.0f;
    public float segmentInterval = 0.5f;
    public LayerMask wallLayer;
    public float rayDistance = 2.0f;

    [Header("DOTweenアニメーション")]
    public float growDuration = 0.5f;
    public Ease growEase = Ease.OutSine;

    [Header("B: ノイズによるゆらぎ")]
    public float wiggleStrength = 30f;
    public float wiggleSpeed = 2.0f;
    private float noiseOffset;

    [Header("C: 枝分かれ設定")]
    [Range(0, 100)]
    public float branchProbability = 10f;
    public int maxGeneration = 3;
    public int currentGeneration = 0;
    public float branchAngle = 30f;

    [Header("成長停止の設定")]
    public int maxSegments = 20;
    public float lifeSpan = 10f;
    private int spawnedCount = 0;
    private float age = 0f;
    private bool isFinished = false;

    private Vector3 lastSpawnPosition;

    void Start()
    {
        noiseOffset = Random.Range(0f, 1000f);
        lastSpawnPosition = transform.position;

        SpawnSegment();
    }

    void Update()
    {
        if (isFinished) return;

        age += Time.deltaTime;
        if (age >= lifeSpan)
        {
            StopGrowth();
            return;
        }

        ApplyWiggle();
        MoveAndStick();

        if (Vector3.Distance(transform.position, lastSpawnPosition) >= segmentInterval)
        {
            SpawnSegment();
        }
    }

    void ApplyWiggle()
    {
        float noise = Mathf.PerlinNoise(Time.time * wiggleSpeed, noiseOffset);
        float wiggle = (noise - 0.5f) * 2.0f;
        transform.Rotate(Vector3.up, wiggle * wiggleStrength * Time.deltaTime);
    }

    void MoveAndStick()
    {
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);

        RaycastHit hit;
        if (Physics.Raycast(transform.position + transform.up * 0.5f, -transform.up, out hit, rayDistance, wallLayer))
        {
            transform.position = hit.point + hit.normal * 0.05f;

            Vector3 upOnWall = Vector3.ProjectOnPlane(Vector3.up, hit.normal).normalized;
            if (upOnWall.sqrMagnitude < 0.001f)
            {
                upOnWall = transform.forward;
            }

            Quaternion targetRot = Quaternion.LookRotation(upOnWall, hit.normal);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 5f);
        }
        else
        {
            transform.Rotate(Vector3.right, 80f * Time.deltaTime);
        }
    }

    void SpawnSegment()
    {
        // ★変更点：プレハブが登録されているかチェック
        if (segmentPrefabs != null && segmentPrefabs.Length > 0)
        {
            // ★ここが魔法！ランダムに1つ選ぶサイコロ
            int randomIndex = Random.Range(0, segmentPrefabs.Length);
            GameObject selectedPrefab = segmentPrefabs[randomIndex];

            GameObject segment = Instantiate(selectedPrefab, transform.position, transform.rotation);
            Vector3 targetScale = segment.transform.localScale;

            segment.transform.localScale = new Vector3(targetScale.x, targetScale.y, 0);
            segment.transform.DOScaleZ(targetScale.z, growDuration).SetEase(growEase);

            segment.transform.Rotate(Vector3.forward, Random.Range(0f, 360f));

            spawnedCount++;

            if (spawnedCount >= maxSegments)
            {
                StopGrowth();
            }
            else
            {
                CheckBranching();
            }
        }
        lastSpawnPosition = transform.position;
    }

    void CheckBranching()
    {
        if (currentGeneration < maxGeneration && Random.Range(0f, 100f) < branchProbability)
        {
            GameObject newBranch = Instantiate(gameObject, transform.position, transform.rotation);
            GrowthManager manager = newBranch.GetComponent<GrowthManager>();

            manager.currentGeneration = this.currentGeneration + 1;
            manager.maxSegments = Mathf.RoundToInt(this.maxSegments * 0.7f);
            
            // ★枝分かれした先でも、同じプレハブリストを引き継ぐ
            manager.segmentPrefabs = this.segmentPrefabs;

            newBranch.transform.Rotate(Vector3.up, Random.Range(-branchAngle, branchAngle));
            newBranch.transform.Translate(Vector3.forward * 0.1f);
        }
    }

    public void StopGrowth()
    {
        isFinished = true;
    }
}