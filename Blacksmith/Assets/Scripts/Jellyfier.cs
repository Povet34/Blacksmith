using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Jellyfier : MonoBehaviour
{
    // 젤리 오브젝트가 얼마나 빠르게 튀어오를지를 설명하는 값
    public float bounceSpeed;

    // 튀어오르는 것을 결국 멈추게 하기 위해 필요한 값
    public float stiffness;

    // Mesh를 얻기 위해 필요한 MeshFilter
    private MeshFilter meshFilter;
    private Mesh mesh;

    // 정점들을 추적하기 위해 필요함
    // 현재 상태뿐만 아니라 원래 위치 등도 포함
    Vector3[] initialVertices;
    Vector3[] currentVertices;
    Vector3[] vertexVelocities;

    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        mesh = meshFilter.mesh;
        //fallForce = Random.Range(25, 80);

        // 정점들 가져오기 (초기 상태와 현재 상태(아직 아무것도 하지 않았으므로 초기 상태와 동일))
        initialVertices = mesh.vertices;

        // 정점의 수는 변경되지 않으므로 이 두 배열의 길이는 항상 동일함
        currentVertices = new Vector3[initialVertices.Length];
        vertexVelocities = new Vector3[initialVertices.Length];
        for (int i = 0; i < initialVertices.Length; i++)
        {
            currentVertices[i] = initialVertices[i];
        }
    }

    private void Update()
    {
        UpdateVertices();
    }

    private void UpdateVertices()
    {
        // 모든 정점을 순회하며 속도에 따라 업데이트
        for (int i = 0; i < currentVertices.Length; i++)
        {
            // 현재 속도를 정점에 더하기 전에 젤리 오브젝트가 튀어오를 수 있도록 해야 함
            // 이를 위해 먼저 변위 값을 계산
            // 초기 메쉬 형태를 저장했으므로 이를 사용하여 시간이 지남에 따라 초기 위치로 되돌릴 수 있음
            Vector3 currentDisplacement = currentVertices[i] - initialVertices[i];
            vertexVelocities[i] -= currentDisplacement * bounceSpeed * Time.deltaTime;

            // 튀어오르는 것을 멈추기 위해 속도를 시간이 지남에 따라 줄여야 함
            vertexVelocities[i] *= 1f - stiffness * Time.deltaTime;
            currentVertices[i] += vertexVelocities[i] * Time.deltaTime;
        }

        // 변경 사항을 볼 수 있도록 mesh.vertices를 현재 정점으로 설정
        mesh.vertices = currentVertices;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
    }

    public void ApplyPressureToPoint(Vector3 _point, float _pressure)
    {
        // 모든 정점을 순회하며 압력을 적용
        for (int i = 0; i < currentVertices.Length; i++)
        {
            ApplyPressureToVertex(i, _point, _pressure);
        }
    }

    public void ApplyPressureToVertex(int _index, Vector3 _position, float _pressure)
    {
        // 각 정점에 얼마나 많은 압력을 가해야 하는지 계산하기 위해
        // 정점과 메쉬에 닿은 지점 사이의 거리를 계산
        Vector3 distanceVerticePoint = currentVertices[_index] - transform.InverseTransformPoint(_position);

        // 이제 역제곱 법칙을 사용해야 함
        // 압력을 거리 제곱으로 나누어 적응된 압력을 계산
        float adaptedPressure = _pressure / (1f + distanceVerticePoint.sqrMagnitude);

        // 이제 이 압력을 사용하여 정점의 속도를 계산
        // 먼저 질량으로 가속도를 계산한 다음 가속도 * Time.deltaTime으로 속도를 계산
        float velocity = adaptedPressure * Time.deltaTime;
        // 속도에 방향을 추가해야 함, 이전에 계산한 정점 지점의 정규화된 거리를 사용하여 계산
        vertexVelocities[_index] += distanceVerticePoint.normalized * velocity;
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
}