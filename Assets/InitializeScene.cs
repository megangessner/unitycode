using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitializeScene : MonoBehaviour
{
	public GameObject[] shapes;
	public int num = 3;
	public int seed = 0;
	public int numFrames = 100;
	public float variability = 1f;
	public float friction = 0.2f;
	public float angularDrag = 0.05f;
	public bool togglePolygons = true;
	public bool saveRun = true;

	string folder = "screenshot_data";
	int framerate = 25;
	private float scale = 1f; // default

    // Start is called before the first frame update
    void Start()
    {
    	if (saveRun) {
    		Time.captureFramerate = framerate;
    	}

    	LoadSettings();
    	scale = Scale();

    	Debug.Log(num);

    	Random.InitState(seed);

    	for (int i=0; i < num; i++) {
    		int shapeindex;
    		Debug.Log(togglePolygons);
    		if (togglePolygons) {
    			shapeindex = (int)Random.Range(0,shapes.Length);
    		}
    		else {
    			shapeindex = 0; //index of circle only
    		}
    		GameObject aShape = Instantiate(shapes[shapeindex], Random.insideUnitCircle*8, Quaternion.identity);
    		Rigidbody2D rb = aShape.GetComponent<Rigidbody2D>();
    		Collider2D col = aShape.GetComponent<Collider2D>();
    		aShape.GetComponent<Transform>().localScale += new Vector3(1f, 1f, 0)*Scale(); 


    		Collider2D[] results = new Collider2D[num];
    		ContactFilter2D filter = new ContactFilter2D();

    		int numOverlaps = rb.OverlapCollider(filter.NoFilter(), results);
			Debug.Log(numOverlaps);

    		if (0 != rb.OverlapCollider(filter.NoFilter(), results)) {
    			for (int j=0; j < numOverlaps; j++){
    				ColliderDistance2D overlap = col.Distance(results[j]);
    				aShape.GetComponent<Transform>().Translate((Vector3)overlap.normal*overlap.distance, Space.World);
    				Debug.Log(aShape.GetComponent<Transform>().position);
    			}
    			rb = aShape.GetComponent<Rigidbody2D>();
    		}

    		// aShape.GetComponent<SpriteRenderer>().color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f); //randomizes color on each shape
    		// aShape.GetComponent<SpriteRenderer>().color = blue;
    		rb.velocity = (Vector2)Random.onUnitSphere*Random.Range(10*(1-variability),10*(1+variability));
    		rb.drag = friction;
    		rb.angularDrag = angularDrag;


    	}   
    }

    // Update is called once per frame
    void Update()
    {


    }

    IEnumerator RecordFrame()
    {
        yield return new WaitForEndOfFrame();

        var path = System.IO.Path.Combine(Application.streamingAssetsPath, folder);
        System.IO.Directory.CreateDirectory(path);

    	var name = System.String.Format("{0}/{1:D04}_screenshot.png", path, Time.frameCount );
    	ScreenCapture.CaptureScreenshot(name);

    }

    void LateUpdate()
    {
    	if (saveRun) {

    		StartCoroutine(RecordFrame());

    		if (Time.frameCount >= numFrames) {
    			Application.Quit();
    		}

    	}
    }

    private void LoadSettings()
    {
        // Path.Combine combines strings into a file path
        // Application.StreamingAssets points to Assets/StreamingAssets in the Editor, and the StreamingAssets folder in a build
        string filePath = System.IO.Path.Combine(Application.streamingAssetsPath, "settings.json");

        if(System.IO.File.Exists(filePath))
        {
            // Read the json from the file into a string
            string dataAsJson = System.IO.File.ReadAllText(filePath);
            // Pass the json to JsonUtility, and tell it to create a GameData object from it
            JsonUtility.FromJsonOverwrite(dataAsJson, this);
        }
        else
        {
            Debug.LogError("Cannot load game data!");
        }
    }

    private float Scale()
    {
    	int boxArea = 100; //depends on how we design the box...
    	float sizeRatio = Mathf.Sqrt(boxArea / (3f*num)); //we want the sum of the areas of the shapes to take up about a third of the available space.
    	float scaleFactor;
    	if (togglePolygons) {
    		scaleFactor = sizeRatio*Random.Range(1f-variability,1f+variability);
    	}
    	else {
    		scaleFactor = sizeRatio*Random.Range(Mathf.Pow(variability,2),1f+variability);
    	}
    	return scaleFactor;
    }
}
