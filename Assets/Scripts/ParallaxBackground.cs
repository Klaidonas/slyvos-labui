using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [SerializeField] private GameObject[] backgroundLayers;
    [SerializeField] private float[] parallaxFactors = { 0.01f, 0.005f, 0.002f }; // Very slow: Close=0.01, Mid=0.005, Far=0.002
    private Vector3 lastCameraPosition;

    void Start()
    {
        lastCameraPosition = Camera.main.transform.position;
        if (backgroundLayers.Length != parallaxFactors.Length)
        {
            Debug.LogError("Number of background layers must match number of parallax factors!");
        }
    }

    void Update()
    {
        Vector3 cameraMovement = Camera.main.transform.position - lastCameraPosition;
        for (int i = 0; i < backgroundLayers.Length; i++)
        {
            if (backgroundLayers[i] != null)
            {
                float parallaxX = cameraMovement.x * parallaxFactors[i]; // Only X-axis movement
                Vector3 newPosition = backgroundLayers[i].transform.position + new Vector3(parallaxX, 0f, 0f);
                backgroundLayers[i].transform.position = newPosition;

                // Optional: Loop the background if it moves off-screen
                float width = backgroundLayers[i].GetComponent<SpriteRenderer>().bounds.size.x;
                if (newPosition.x < -width) newPosition.x += width * 2;
                if (newPosition.x > width) newPosition.x -= width * 2;
                backgroundLayers[i].transform.position = newPosition;
            }
        }
        lastCameraPosition = Camera.main.transform.position;
    }
}