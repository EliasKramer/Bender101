using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class EditorGameObjects : MonoBehaviour
{	static public Camera GetCamera() {
		Camera camera = null;

		if (SceneView.lastActiveSceneView != null && SceneView.lastActiveSceneView.camera != null) {
			camera = SceneView.lastActiveSceneView.camera;
		} else if (Camera.main != null) {
			camera = Camera.main;
		}
		return(camera);
	}

	static public Vector3 GetCameraPoint() {
		Vector3 pos = Vector3.zero;

		Camera camera = GetCamera();
		if (camera != null) {
			Ray worldRay = camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 1.0f));
			pos = worldRay.origin;
			pos.z = 0;
		} else {
			Debug.LogError("Scene Camera Not Found");
		}

		return(pos);
	}

    [MenuItem("GameObject/Slicer 2D/Slicer Controller/All Included", false, 4)]
    static void Create_SlicerController_Main() {	
		GameObject newGameObject = new GameObject("Slicer Controller");

        SpriteRenderer spriteRenderer = newGameObject.AddComponent<SpriteRenderer>();

        spriteRenderer.sprite = Resources.Load<Sprite>("Sprites/slicer_rectangle");

        SetupSprite2DGameObject(newGameObject);
	}

	[MenuItem("GameObject/Slicer 2D/Sliceable/Sprite/Rectangle", false, 4)]
    static void Create_Sliceable_Sprite_Rectangle() {	
		GameObject newGameObject = new GameObject("Sliceable 2D (Sprite Rectangle)");

        SpriteRenderer spriteRenderer = newGameObject.AddComponent<SpriteRenderer>();

        spriteRenderer.sprite = Resources.Load<Sprite>("Sprites/slicer_rectangle");

        SetupSprite2DGameObject(newGameObject);
	}

    [MenuItem("GameObject/Slicer 2D/Sliceable/Sprite/Circle", false, 4)]
    static void Create_Sliceable_Sprite_Circle() {	
		GameObject newGameObject = new GameObject("Sliceable 2D (Sprite Circle)");

        SpriteRenderer spriteRenderer = newGameObject.AddComponent<SpriteRenderer>();

        spriteRenderer.sprite = Resources.Load<Sprite>("Sprites/slicer_circle");

        SetupSprite2DGameObject(newGameObject);
	}

    [MenuItem("GameObject/Slicer 2D/Sliceable/Collider/Rectangle", false, 4)]
    static void Create_Sliceable_Collider_Rectangle() {	
		GameObject newGameObject = new GameObject("Sliceable 2D (Collider Rectangle)");

        BoxCollider2D collider = newGameObject.AddComponent<BoxCollider2D>();

        SetupMesh2DGameObject(newGameObject);
	}

    [MenuItem("GameObject/Slicer 2D/Sliceable/Collider/Circle", false, 4)]
    static void Create_Sliceable_Collider_Circle() {	
		GameObject newGameObject = new GameObject("Sliceable 2D (Collider Circle)");

        CircleCollider2D collider = newGameObject.AddComponent<CircleCollider2D>();

        SetupMesh2DGameObject(newGameObject);
	}

    [MenuItem("GameObject/Slicer 2D/Sliceable/Collider/Capsule", false, 4)]
    static void Create_Sliceable_Collider_Capsule() {	
		GameObject newGameObject = new GameObject("Sliceable 2D (Collider Capsule)");

        CapsuleCollider2D collider = newGameObject.AddComponent<CapsuleCollider2D>();

        SetupMesh2DGameObject(newGameObject);
	}

    [MenuItem("GameObject/Slicer 2D/Sliceable/Collider/Polygon", false, 4)]
    static void Create_Sliceable_Collider_Polygon() {	
		GameObject newGameObject = new GameObject("Sliceable 2D (Collider Polygon)");

        PolygonCollider2D collider = newGameObject.AddComponent<PolygonCollider2D>();

        SetupMesh2DGameObject(newGameObject);

        
	}

    public static void SetupMesh2DGameObject(GameObject newGameObject) {
        Slicer2D.Sliceable2D sliceable = newGameObject.AddComponent<Slicer2D.Sliceable2D>();

        sliceable.textureType = Slicer2D.Sliceable2D.TextureType.Mesh2D;

        sliceable.materialSettings.material = new Material(Shader.Find("Sprites/Default"));
        sliceable.materialSettings.material.color = Color.white;

        Utilities2D.ColliderLineRenderer2D lineRenderer = newGameObject.AddComponent<Utilities2D.ColliderLineRenderer2D>();

        lineRenderer.lineWidth = 0.25f;

		newGameObject.transform.position = GetCameraPoint();
    }

    public static void SetupSprite2DGameObject(GameObject newGameObject) {
        Slicer2D.Sliceable2D sliceable = newGameObject.AddComponent<Slicer2D.Sliceable2D>();

        CircleCollider2D collider = newGameObject.AddComponent<CircleCollider2D>();

        Utilities2D.ColliderLineRenderer2D lineRenderer = newGameObject.AddComponent<Utilities2D.ColliderLineRenderer2D>();

        lineRenderer.lineWidth = 0.25f;

		newGameObject.transform.position = GetCameraPoint();
    }



}
