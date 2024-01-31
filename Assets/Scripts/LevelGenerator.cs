using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private Platform platform;
    [SerializeField] private float angleStep;
    [SerializeField] private int platformAmount;
    [SerializeField] private float platformHeight;
    [SerializeField] private Material badMaterial;
    [SerializeField] private Material winMaterial;
    // Start is called before the first frame update
    [ContextMenu("Generate")]
    public void GenerateLevel()
    {
        for (int i = 0; i < platformAmount-1; i++)
        {
            
            var newPlatform= Instantiate(platform,transform.position+ (Vector3.up*-platformHeight*i),Quaternion.Euler(0,angleStep*i,0),transform);
           
            if(Random.Range(0f, 100f) < 15)
            {
                newPlatform.colorRenderer.material=badMaterial;
                newPlatform.tag = "Bad";
            }
            else
            {
				newPlatform.colorRenderer.sharedMaterial.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
                newPlatform.tag = "Good";
			}
        }
		var endPoint = Instantiate(platform, transform.position + (Vector3.up * -platformHeight * (platformAmount-1)), Quaternion.Euler(0, angleStep * (platformAmount-1), 0), transform);
        endPoint.colorRenderer.material = winMaterial;
        endPoint.tag = "Win";

	}
    [ContextMenu("Clean")]
    public void Clean() {
        int childCount=transform.childCount;
        for (int i = childCount-1;i >= 0;i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up*5*Time.deltaTime);
    }
}
