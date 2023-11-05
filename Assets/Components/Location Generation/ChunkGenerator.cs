using System.Collections.Generic;
using UnityEngine;

public class ChunkGenerator : MonoBehaviour
{
    [Header("Chunk prefab")]
    [SerializeField] GameObject _chunk;
    public GameObject Chunk => _chunk;

    [Header("Chunk size")]
    [SerializeField] int _chunkSize;
    public int ChunkSize => _chunkSize;

    [Header("Chunk size")]
    [SerializeField] GenerationRange _generationRange;
    public int GenerationRange => (int)_generationRange;

    [SerializeField] GameObject[] _obstacles;
    public GameObject[] Obstacles => _obstacles;

    [Header("Location generation salt")]
    [SerializeField] int _salt;
    public int Salt => _salt;

    [SerializeField] Transform _trackedTransform;
    public Transform TrackedTransform => _trackedTransform;

    public static ChunkGenerator Instance { get; private set; }


    private Dictionary<Vector2Int, GameObject> _chunks = new Dictionary<Vector2Int, GameObject>();

    private void Awake() => Instance = this;

    void FixedUpdate()
    {
        var pos = new Vector2(_trackedTransform.position.x, _trackedTransform.position.z);
        GenerateChunks(pos);
    }

    private void GenerateChunks(Vector2 pos)
    {
        var x = Mathf.RoundToInt(pos.x / _chunkSize);
        var y = Mathf.RoundToInt(pos.y / _chunkSize);

        var leftTopChunk = new Vector2Int(x - GenerationRange / 2, y - GenerationRange / 2);

        for (int i = 0; i < GenerationRange; i++)
        {
            for (int j = 0; j < GenerationRange; j++)
            {
                int currentX = leftTopChunk.x + i;
                int currentY = leftTopChunk.y + j;

                if (!_chunks.ContainsKey(new Vector2Int(currentX, currentY)))
                {
                    var chunk = Instantiate(Chunk, new Vector3(currentX * _chunkSize, 0, currentY * _chunkSize), Quaternion.identity);
                    _chunks[new Vector2Int(currentX, currentY)] = chunk;
                }
            }
        }

        var removeChunks = new List<KeyValuePair<Vector2Int, GameObject>>();
        var distance = GenerationRange / 2;
        foreach (var kvp in _chunks)
        {

            if (Mathf.Abs(x - kvp.Key.x) > distance || Mathf.Abs(y - kvp.Key.y) > distance)
            {
                removeChunks.Add(kvp);
                Destroy(kvp.Value.gameObject);
            }
        }

        foreach (var kvp in removeChunks)
        {
            _chunks.Remove(kvp.Key);
        }
    }
}

enum GenerationRange
{
    Small = 3,
    Medium = 7,
    Large = 14,
}
