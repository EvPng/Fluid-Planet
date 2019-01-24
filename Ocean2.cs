using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ocean2 : MonoBehaviour
{
    // Create UI controllers in Unity

    public GameObject ocean;
    public Terrain t;

    public class LiquidVertex
    {
        public float forceY = 0.0f;
        public float velocityY = 0.0f;
        public Vector3 waterPosition;
        public Vector3 groundPosition;
    }

    LiquidVertex[] lv;

    Vector3[] v;

    // Referencing perlin.cs
    Perlin noise = new Perlin();

    // Slider control amplitude of the ocean waves
    [Range(0.1f, 10.0f)]
    public float amplitude = 5.0f;

    // Slider control speed of the waves
    [Range(0.1f, 10.0f)]
    public float speed = 1.0f;

    // Slider control frequency of the waves (affect number of bumps)
    [Range(0.1f, 3.0f)]
    public float frequency = 1.0f;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Mesh m = GetComponent<MeshFilter>().mesh;

        if (v == null)
        {
            v = m.vertices;
        }

        Vector3[] vertices = new Vector3[v.Length];

        // Iterate over each vertex in the mesh plane to make waves
        for (int i = 0; i < v.Length; ++i)
        {
            Vector3 vertex = v[i];

            // Get the position of the ocean vertex relative to the terrain
            Vector3 rV = transform.TransformPoint(vertex);

            // Calculate the relative height differenc between the terrain and the ocean vertex
            float rH = (20.0f + vertex.y - t.SampleHeight(rV)) / 20.0f;

            // Add randomized noises to some of the vertex
            if (i % 177 == 0)
            {
                float timey = Time.time * speed + 1.173547f;
                
                vertex.y += noise.Noise(timey + vertex.x, timey + vertex.y, timey + vertex.z) * 8f;
                
                // To make the update less frequent
                StartCoroutine(SlowUpdate());
            }
            else
            {
                // Apply wave function to vertex
                float a = Mathf.Sin((Time.time * speed + v[i].x + v[i].y) * frequency) * amplitude;
                float b = Mathf.Cos((Time.time * speed + v[i].y + v[i].z) * frequency);
                float c = 1.0f;
                float d = 0.0f;

                // Respond to the relative distance to the terrain
                // If the wave is around an island, make the wave bigger
                if (rH <= 0.0f)
                {
                    c = 6f;
                }
                // Else if the wave is above a deep canyon, make the wave lower
                else if (rH > 0.8f)
                {
                    c = -2f;
                    d = -1f;
                }

                // Add to the y axis of the vertex
                vertex.y += a * b * c + d;
                
            }

            // Apply to the original vertice of the plane
            vertices[i] = vertex;
        }

        // Recalculate the mesh vertices and normals to be ready for rendering
        m.vertices = vertices;
        m.RecalculateNormals();
    }

    // Function - Slow Update
    public IEnumerator SlowUpdate()
    {
        while (true)
        {

            yield return new WaitForSeconds(.5f);
        }
    }
}