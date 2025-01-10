using UnityEngine;

public class HitVertex : MonoBehaviour
{
    private Material material; // �������� ������ ��Ƽ������ ������ ����
    private Texture2D emissionTexture;
    public float radius = 10f; // ���� �ݰ�

    void Start()
    {
        // URP Lit ��Ƽ���� �������� ����
        material = new Material(Shader.Find("Universal Render Pipeline/Lit"));

        // ��Ƽ������ �����ϰ� ����
        material.SetFloat("_Surface", 1); // 1�� Transparent�� �ǹ�
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        material.SetInt("_ZWrite", 0);
        material.DisableKeyword("_ALPHATEST_ON");
        material.EnableKeyword("_ALPHABLEND_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;

        // �������Ͻ��� ��Ż�� ���� 0���� ����
        material.SetFloat("_Smoothness", 0f);
        material.SetFloat("_Metallic", 0f);

        // �˺��� �÷��� ���� ���� 0���� ����
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

        // Emission �Ӽ��� Emission �ؽ�ó ����
        material.SetTexture("_EmissionMap", emissionTexture);
        material.SetColor("_EmissionColor", Color.white); // Emission ������ ������� ����
        material.EnableKeyword("_EMISSION"); // Emission Ȱ��ȭ

        material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;

        // ��Ƽ������ �޽� �������� ����
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            // ���� ��Ƽ���� ����Ʈ�� ������
            Material[] materials = meshRenderer.materials;

            // ���ο� ��Ƽ���� ����Ʈ�� �����ϰ� ���� ��Ƽ������ ����
            Material[] newMaterials = new Material[materials.Length + 1];
            for (int i = 0; i < materials.Length; i++)
            {
                newMaterials[i] = materials[i];
            }

            // ���ο� ��Ƽ������ ����Ʈ�� �������� �߰�
            newMaterials[newMaterials.Length - 1] = material;

            // ���ο� ��Ƽ���� ����Ʈ�� �޽� �������� ����
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

                    // �浹 �������� ���� ����� ���ؽ��� ã��
                    int closestVertexIndex = GetClosestVertexIndex(hit.point, vertices, meshFilter.transform);

                    // ���� ����� ���ؽ��� UV ��ǥ�� ������
                    Vector2 uv = uvs[closestVertexIndex];

                    // UV ��ǥ�� ����Ͽ� Emission �ؽ�ó�� ���� �׸�
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

        // �� �׸���
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