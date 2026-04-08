using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class SecurityCameraFieldOfView : MonoBehaviour	//카메라 시각화 시야
{
	[Header("연결")]
	public SecurityCameraController cameraBase;

	[Header("시야 시각화")]
	public Material fovMaterial;
	public int meshResolution = 30; //시야 밑면 정점. 높을수록 둥글어짐. ex) 4로 설정하면 사각뿔

	private MeshFilter meshFilter;
	private Mesh fovMesh;

	[Header("테두리 설정")]
	public bool showOutline = true;
	public Material outlineMaterial;

	private GameObject outlineObj;
	private MeshFilter outlineFilter;
	private Mesh outlineMesh;


	//컴포넌트 세팅
	void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
		fovMesh = new Mesh();
		fovMesh.name = "FOV Mesh";
		meshFilter.mesh = fovMesh;

		if (fovMaterial != null)
		{
			GetComponent<MeshRenderer>().material = fovMaterial;
		}
		else
		{
			Debug.LogError("FOV Material 미할당");
		}

		if (cameraBase == null)
		{
			cameraBase = GetComponentInParent<SecurityCameraController>();
		}

		//테두리용 자식오브젝트
		CreateOutlineObject();
    }


	void CreateOutlineObject()
	{
		outlineObj = new GameObject("FOV Outline");
		outlineObj.transform.SetParent(transform, false);
		outlineFilter = outlineObj.AddComponent<MeshFilter>();
		MeshRenderer ren = outlineObj.AddComponent<MeshRenderer>();
		ren.shadowCastingMode = ShadowCastingMode.Off;

		outlineMesh = new Mesh() {name = "Outline Mesh"};
		outlineFilter.mesh = outlineMesh;
		if (outlineMaterial != null)
		{
			ren.material = outlineMaterial;
		}
	}

	// 카메라 시각화 시야 갱신
	void LateUpdate()
    {
		DrawFOV();
    }

	// 카메라 시각화 시야(원뿔)
	void DrawFOV()
	{
		if (cameraBase == null)
		{
			return;
		}

		float radius = cameraBase.viewRadius;
		float angle = cameraBase.viewAngle;

		float baseRadius = radius * Mathf.Tan(angle * 0.5f * Mathf.Deg2Rad);	//거리에 따른 밑면 반지름

		List<Vector3> vertices = new List<Vector3>();
		List<int> triangles = new List<int>();
		List<int> outlines = new List<int>();

		vertices.Add(Vector3.zero); //시야 시작점(원뿔 꼭지점)

		// 원뿔 밑면 둘레 정점(Vertex. 시야 끝 꼭지점) 구하기
		for (int i = 0; i <= meshResolution; i++)
		{
			float currentAngle = (i * (360f / meshResolution)) * Mathf.Deg2Rad + (45f * Mathf.Deg2Rad);

			//원 둘레 좌표, 거리 계산
			float x = Mathf.Cos(currentAngle) * baseRadius;
			float y = Mathf.Sin(currentAngle) * baseRadius;
			float z = radius;
			vertices.Add(new Vector3(x, y, z));

			if (i > 0)		//둘레점 순서를 바꾸면 원 바깥 시점에서 표시되지 않음
			{
				triangles.Add(0);		//시작점
				triangles.Add(i + 1);	//현재 원 둘레 점
				triangles.Add(i);		//이전 원 둘레 점

				if (showOutline == true)
				{
					//세로선
					outlines.Add(0);
					outlines.Add(i);

					//가로선
					outlines.Add(i);
					outlines.Add(i + 1);
				}
			}
		}

		fovMesh.Clear();
		fovMesh.vertices = vertices.ToArray();
		fovMesh.triangles = triangles.ToArray();
		fovMesh.RecalculateNormals();   //물체 빛 반사를 위한 법선 계산

		if (showOutline && outlineMesh != null)
		{
			outlineObj.SetActive(true);
			outlineMesh.Clear();
			outlineMesh.vertices = vertices.ToArray();
			outlineMesh.SetIndices(outlines.ToArray(), MeshTopology.Lines, 0);
		}
		else if (outlineObj != null)
		{
			outlineObj.SetActive(false);
		}
	}
}
