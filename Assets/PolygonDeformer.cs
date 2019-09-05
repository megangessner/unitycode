using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolygonDeformer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {    	
    	SpriteRenderer spriteRend = GetComponent<SpriteRenderer>();
    	Sprite sprite = spriteRend.sprite;
    	Sprite dupe = Instantiate(sprite);
    	var transform = GetComponent<Transform>(); 
		spriteRend.sprite = dupe;

    	Vector2[] vertices = dupe.vertices;
    	int sides = vertices.Length;
    	List<Vector2> shape = new List<Vector2>(GetComponent<PolygonCollider2D>().points);
    	Vector2[] rectVertices = new Vector2[sides];

    	// settings are read in from settings.json in initialize scene script, we want to use here for wonkiness
    	GameObject ShapeCollection = GameObject.Find("ShapeCollection");
    	InitializeScene settings = ShapeCollection.GetComponent<InitializeScene>();

    	// because apparently we need to be operating with vertices coords in three separate spaces

    	for (int i=0; i < sides; i++) {

    		var adjustedPoint = Random.Range(1f-settings.variability,1.0f)*vertices[i]; // reasonable wonkiness range

    		rectVertices[i].x = Mathf.Clamp(
                (adjustedPoint.x - dupe.bounds.center.x -
                    (dupe.textureRectOffset.x / dupe.texture.width) + dupe.bounds.extents.x) /
                (2.0f * dupe.bounds.extents.x) * dupe.rect.width,
                0.0f, dupe.rect.width);

            rectVertices[i].y = Mathf.Clamp(
                (adjustedPoint.y - dupe.bounds.center.y -
                    (dupe.textureRectOffset.y / dupe.texture.height) + dupe.bounds.extents.y) /
                (2.0f * dupe.bounds.extents.y) * dupe.rect.height,
                0.0f, dupe.rect.height);

            shape[shape.FindIndex(vertex => vertex == vertices[i])] = rectVertices[i];
    	}

    	List<Vector2[]> physicsShape = new List<Vector2[]>{shape.ToArray()};

    	dupe.OverrideGeometry(rectVertices, sprite.triangles);
    	dupe.OverridePhysicsShape(physicsShape);

    	// destroying and recreating the polygon collider forces it to be the same geometry as the physics shape.
    	Destroy(GetComponent<PolygonCollider2D>());
    	gameObject.AddComponent<PolygonCollider2D>();

    	GetComponent<Transform>().localScale += new Vector3(settings.variability, settings.variability, 0); // after moving points inwards... this scales to be closer to average size of shape in box
        
    }
}
