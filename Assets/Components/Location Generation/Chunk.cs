using System;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    [SerializeField] int _chunkSeed;

    private float x = 0;
    private float y = 0;

    void Awake()
    {
        x = gameObject.transform.position.x;
        y = gameObject.transform.position.z;

        _chunkSeed = GetRandomSeed((int)x, (int)y, ChunkGenerator.Instance.Salt);
    }

    private void Start()
    {
        var chunkSize = ChunkGenerator.Instance.ChunkSize;
        float halfChunkSize = chunkSize / 2;

        var rnd = new System.Random(_chunkSeed);

        float startX = gameObject.transform.position.x - halfChunkSize;
        float endX = startX + chunkSize;
        float startY = gameObject.transform.position.z - halfChunkSize;
        float endY = startY + chunkSize;

        var radnomX = rnd.NextDouble() * (endX - startX) + startX;
        var radnomY = rnd.NextDouble() * (endY - startY) + startY;

        var obstacleId = rnd.Next(0, ChunkGenerator.Instance.Obstacles.Length);

        Instantiate(ChunkGenerator.Instance.Obstacles[obstacleId], new Vector3((float) radnomX, 0, (float)radnomY), Quaternion.identity, gameObject.transform);
    }

    private int GetRandomSeed(int x, int y, int salt)
    {
        if (x < 0) x += int.MaxValue;
        if (y < 0) y += int.MaxValue;

        return new System.Random(x).Next() + new System.Random(y).Next() * new System.Random(Math.Max(x, y)).Next() + salt;
    }
}
