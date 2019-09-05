using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StackBlocks : MonoBehaviour
{

	public GameObject block;
	public int numShapes;
	public int seed = 0;
    // Start is called before the first frame update
    void Start()
    {
    	Random.InitState(seed);
    	float height = 0.0f;

    	for (int i = 0; i < numShapes; i++) {
    		GameObject newBlock = Instantiate(block, new Vector3(Random.value, 0.0f, Random.value), Quaternion.AngleAxis(Random.Range(0,180), Vector3.up));

    		newBlock.GetComponent<MeshRenderer>().material.color = Random.ColorHSV();

    		newBlock.transform.localScale += Random.insideUnitSphere;
    		newBlock.transform.position += Vector3.up*(height + 0.5f*newBlock.transform.localScale.y); //stack the center of this block on last object
    		height += newBlock.transform.localScale.y; // new height of pile
    	}
        
    }

    void OnGUI() {
    	if (GUI.Button(new Rect(10,10,100,90), "Unfreeze")) {
    		Rigidbody[] rbs = FindObjectsOfType<Rigidbody>();
    		foreach(Rigidbody rb in rbs) {
    			rb.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
    		}
    	}
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
