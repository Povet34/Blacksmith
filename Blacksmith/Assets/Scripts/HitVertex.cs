using UnityEngine;

public class HitVertex : MonoBehaviour
{
    private Material material; // 동적으로 생성된 머티리얼을 저장할 변수
    private Texture2D emissionTexture;
    public float radius = 10f; // 원의 반경

    void Start()
    {
        // URP Lit 머티리얼 동적으로 생성
        material = new Material(Shader.Find("Universal Render Pipeline/Lit"));

        // 머티리얼을 투명하게 설정
        material.SetFloat("_Surface", 1); // 1은 Transparent를 의미
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        material.SetInt("_ZWrite", 0);
        material.DisableKeyword("_ALPHATEST_ON");
        material.EnableKeyword("_ALPHABLEND_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;

        // 스무스니스와 메탈릭 값을 0으로 설정
        material.SetFloat("_Smoothness", 0f);
        material.SetFloat("_Metallic", 0f);

        // 알베도 컬러의 알파 값을 0으로 설정
        Color albedoColor = material.GetColor("_BaseColor");
        albedoColor.a = 0.01f;
        material.SetColor("_BaseColor", albedoColor);

        // Create a black texture dynamically.
        emissionTexture = new Texture2D(128, 128); // Create a 128x128 texture.
        // Fill the texture with black color.
        for (int y = 0; y < emissionTexture.height; y++)
        {
            for (int x = 0; x < emissionTexture.width; x++)
            {
                emissionTexture.SetPixel(x, y, Color.black);
            }
        }

        emissionTexture.Apply(); // Apply changes to the texture.

        // Emission 속성에 Emission 텍스처 설정
        material.SetTexture("_EmissionMap", emissionTexture);
        material.SetColor("_EmissionColor", Color.white); // Emission 색상을 흰색으로 설정
        material.EnableKeyword("_EMISSION"); // Emission 활성화

        material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;

        // 머티리얼을 메쉬 렌더러에 적용
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            // 기존 머티리얼 리스트를 가져옴
            Material[] materials = meshRenderer.materials;

            // 새로운 머티리얼 리스트를 생성하고 기존 머티리얼을 복사
            Material[] newMaterials = new Material[materials.Length + 1];
            for (int i = 0; i < materials.Length; i++)
            {
                newMaterials[i] = materials[i];
            }

            // 새로운 머티리얼을 리스트의 마지막에 추가
            newMaterials[newMaterials.Length - 1] = material;

            // 새로운 머티리얼 리스트를 메쉬 렌더러에 설정
            meshRenderer.materials = newMaterials;
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                MeshFilter meshFilter = hit.collider.GetComponent<MeshFilter>();
                if (meshFilter != null)
                {
                    Mesh mesh = meshFilter.mesh;
                    Vector3[] vertices = mesh.vertices;
                    Vector2[] uvs = mesh.uv;

                    // 충돌 지점에서 가장 가까운 버텍스를 찾음
                    int closestVertexIndex = GetClosestVertexIndex(hit.point, vertices, meshFilter.transform);

                    // 가장 가까운 버텍스의 UV 좌표를 가져옴
                    Vector2 uv = uvs[closestVertexIndex];

                    // UV 좌표를 사용하여 Emission 텍스처에 원을 그림
                    ApplyHitEffect(uv);
                }
            }
        }
    }

    private int GetClosestVertexIndex(Vector3 hitPoint, Vector3[] vertices, Transform meshTransform)
    {
        int closestIndex = -1;
        float closestDistance = float.MaxValue;
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 worldVertex = meshTransform.TransformPoint(vertices[i]);
            float distance = Vector3.Distance(hitPoint, worldVertex);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestIndex = i;
            }
        }
        return closestIndex;
    }

    public void ApplyHitEffect(Vector2 uv)
    {
        int x = (int)(uv.x * emissionTexture.width);
        int y = (int)(uv.y * emissionTexture.height);

        // 원 그리기
        for (int i = -Mathf.CeilToInt(radius); i <= Mathf.CeilToInt(radius); i++)
        {
            for (int j = -Mathf.CeilToInt(radius); j <= Mathf.CeilToInt(radius); j++)
            {
                if (i * i + j * j <= radius * radius)
                {
                    int drawX = Mathf.Clamp(x + i, 0, emissionTexture.width - 1);
                    int drawY = Mathf.Clamp(y + j, 0, emissionTexture.height - 1);
                    emissionTexture.SetPixel(drawX, drawY, Color.white);
                }
            }
        }

        emissionTexture.Apply(); // Apply changes to the texture
    }
}