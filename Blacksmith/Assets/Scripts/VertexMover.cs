using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class VertexMover : MonoBehaviour
{
    // Mesh를 얻기 위해 필요한 MeshFilter
    private MeshFilter meshFilter;
    private Mesh mesh;

    public float affectedNearByRatio = 0.2f;

    // 정점들을 추적하기 위해 필요함
    // 현재 상태뿐만 아니라 원래 위치 등도 포함
    Vector3[] currentVertices;
    Vector3[] vertexVelocities;
    Vector3[] initialVertices;
    Vector3[] normals;

    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        mesh = meshFilter.mesh;

        // 정점들 가져오기 (초기 상태와 현재 상태(아직 아무것도 하지 않았으므로 초기 상태와 동일))
        initialVertices = mesh.vertices;
        normals = mesh.normals;

        // 정점의 수는 변경되지 않으므로 이 두 배열의 길이는 항상 동일함
        currentVertices = new Vector3[initialVertices.Length];
        vertexVelocities = new Vector3[initialVertices.Length];
        for (int i = 0; i < initialVertices.Length; i++)
        {
            currentVertices[i] = initialVertices[i];
        }
    }

    private void UpdateVertices()
    {
        for (int i = 0; i < currentVertices.Length; i++)
        {
            currentVertices[i] += vertexVelocities[i] * Time.deltaTime;
        }

        mesh.vertices = currentVertices;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
    }

    public void ApplyPressureToPoint(Vector3 _point, float _pressure)
    {
        Array.Clear(vertexVelocities, 0, vertexVelocities.Length);

        int closestVertexIndex = GetClosestVertexIndex(_point);
        ApplyPressureToVertex(closestVertexIndex, _point, _pressure);

        var closestVertices = GetClosestVertices(closestVertexIndex, 10);
        foreach (var vertex in closestVertices)
        {
            ApplyOppositePressureToVertex(vertex.Key, closestVertexIndex, _pressure);
        }

        UpdateVertices();
    }

    private int GetClosestVertexIndex(Vector3 _position)
    {
        int closestIndex = -1;
        float closestDistance = float.MaxValue;
        for (int i = 0; i < currentVertices.Length; i++)
        {
            float distance = Vector3.Distance(currentVertices[i], transform.InverseTransformPoint(_position));
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestIndex = i;
            }
        }
        return closestIndex;
    }

    private Dictionary<int, float> GetClosestVertices(int _index, int count)
    {
        Dictionary<int, float> distances = new Dictionary<int, float>();
        for (int i = 0; i < currentVertices.Length; i++)
        {
            if (i == _index) continue; // 자기 자신은 제외
            float distance = Vector3.Distance(currentVertices[_index], currentVertices[i]);
            distances.Add(i, distance);
        }

        // 거리 기준으로 정렬하여 가장 가까운 count개의 버텍스를 선택
        return distances.OrderBy(pair => pair.Value).Take(count).ToDictionary(pair => pair.Key, pair => pair.Value);
    }

    public void ApplyPressureToVertex(int _index, Vector3 _position, float _pressure)
    {
        vertexVelocities[_index] = Vector3.zero;
        Vector3 distanceVerticePoint = currentVertices[_index] - transform.InverseTransformPoint(_position);
        float adaptedPressure = _pressure / (1f + distanceVerticePoint.sqrMagnitude);
        float velocity = adaptedPressure;
        vertexVelocities[_index] -= distanceVerticePoint.normalized * velocity;
    }

    public void ApplyOppositePressureToVertex(int _index, int closestVertexIndex, float _pressure)
    {
        Vector3 directionFromClosest = currentVertices[_index] - currentVertices[closestVertexIndex];
        float adaptedPressure = _pressure / (1f + directionFromClosest.sqrMagnitude);
        float velocity = adaptedPressure;
        vertexVelocities[_index] += directionFromClosest.normalized * velocity * affectedNearByRatio;
    }
}