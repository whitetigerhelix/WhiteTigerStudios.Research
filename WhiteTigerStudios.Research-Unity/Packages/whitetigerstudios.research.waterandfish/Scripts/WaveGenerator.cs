/*
 * Copyright (c) 2020 Razeware LLC
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * Notwithstanding the foregoing, you may not use, copy, modify, merge, publish, 
 * distribute, sublicense, create a derivative work, and/or sell copies of the 
 * Software in any work that is designed, intended, or marketed for pedagogical or 
 * instructional purposes related to programming, coding, application development, 
 * or information technology.  Permission for such use, copying, modification,
 * merger, publication, distribution, sublicensing, creation of derivative works, 
 * or sale is expressly withheld.
 *    
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;

namespace WhitetigerStudios.Research.WaterAndFish
{
    public class WaveGenerator : MonoBehaviour
    {
        // JobHandle serves three primary functions:
        //  - Scheduling a job correctly.
        //  - Making the main thread wait for a job’s completion.
        //  - Adding dependencies. Dependencies ensure that a job only starts after another job completes. 
        //    This prevents two jobs from changing the same data at the same time.  It segments the logical flow of your game.
        JobHandle meshModificationJobHandle;
        UpdateMeshJob meshModificationJob; // Reference an UpdateMeshJob so the entire class can access it.

        // Responsible for transporting the vertices and normals of the water mesh to and from the jobs.
        NativeArray<Vector3> waterVertices;
        NativeArray<Vector3> waterNormals;

        [Header("Wave Parameters")]
        [Tooltip("Scales the Perlin noise function.")]
        public float waveScale;
        [Tooltip("The speed that the Perlin noise shifts over time.")]
        public float waveOffsetSpeed;
        [Tooltip("The height multiplier of the Perlin noise.")]
        public float waveHeight;

        [Header("References and Prefabs")]
        public MeshFilter waterMeshFilter;
        private Mesh waterMesh;

        // Initialize data
        private void Start()
        {
            waterMesh = waterMeshFilter.sharedMesh; // Use "mesh" if not in a prefab

            // Mark the waterMesh as dynamic so Unity can optimize sending vertex changes from the CPU to the GPU.
            waterMesh.MarkDynamic();

            // Initialize waterVertices with the vertices of the waterMesh and assign a persistent allocator.
            waterVertices = new NativeArray<Vector3>(waterMesh.vertices, Allocator.Persistent);

            waterNormals = new NativeArray<Vector3>(waterMesh.normals, Allocator.Persistent);
        }

        // Creating a job and assigning the variables within the job
        private void Update()
        {
            // Initialize the UpdateMeshJob with all the variables required for the job.
            meshModificationJob = new UpdateMeshJob()
            {
                vertices = waterVertices,
                normals = waterNormals,
                offsetSpeed = waveOffsetSpeed,
                time = Time.time,
                scale = waveScale,
                height = waveHeight
            };

            // IJobParallelFor’s Schedule() requires the length of the loop and the batch size. 
            // The batch size determines how many segments to divide the work into.
            meshModificationJobHandle = meshModificationJob.Schedule(waterVertices.Length, 64);
        }

        // Ensuring completion and setting vertices
        private void LateUpdate()
        {
            // Ensures the completion of the job because you can’t get the result of the vertices inside the job before it completes.
            meshModificationJobHandle.Complete();

            // Unity allows you to directly set the vertices of a mesh from a job.
            // This is a new improvement that eliminates copying the data back and forth between threads.
            waterMesh.SetVertices(meshModificationJob.vertices);

            // Recalculate the normals of the mesh so that the lighting interacts with the deformed mesh correctly.
            waterMesh.RecalculateNormals();
        }

        // Disposing NativeArrays
        private void OnDestroy()
        {
            waterVertices.Dispose();
            waterNormals.Dispose();
        }

        // Wave generator job
        [BurstCompile]
        private struct UpdateMeshJob : IJobParallelFor
        {
            // This is a public NativeArray to read and write vertex data between the job and the main thread.
            public NativeArray<Vector3> vertices;

            // The [ReadOnly] tag tells the Job System that you only want to read the data from the main thread.
            [ReadOnly]
            public NativeArray<Vector3> normals;

            // These variables control how the Perlin noise function acts. The main thread passes them in.
            public float offsetSpeed;
            public float scale;
            public float height;

            // Note that you cannot access statics such as Time.time within a job. 
            // Instead, you pass them in as variables during the job’s initialization.
            public float time;

            // Perlin noise function to sample Perlin noise given an x and a y parameter.
            private float Noise(float x, float y)
            {
                float2 pos = math.float2(x, y);
                return noise.snoise(pos);
            }

            // Job execution code
            public void Execute(int i)
            {
                // Ensure the wave only affects the vertices facing upwards. This excludes the base of the water.
                if (normals[i].z > 0f)
                {
                    // Get a reference to the current vertex.
                    var vertex = vertices[i];

                    // Sample Perlin noise with scaling and offset transformations.
                    float transformedX = vertex.x * scale + offsetSpeed * time;
                    float transformedY = vertex.y * scale + offsetSpeed * time;
                    float noiseValue = Noise(transformedX, transformedY);

                    // Apply the value of the current vertex within the vertices.
                    float transformedHeight = noiseValue * height + 0.3f;
                    vertices[i] = new Vector3(vertex.x, vertex.y, transformedHeight);
                }
            }
        }
    }
}
