using UnityEngine;

public class LockedParallaxCamera : MonoBehaviour
{
    [Header("Camera Lock")]
    public Vector3 lockedPosition = new Vector3(-17.32f, 1.64f, -4.3f);

    [Header("Parallax Settings")]
    [SerializeField] private float moveSensitivity = 0.1f;
    [SerializeField] private Vector2 maxOffset = new Vector2(0.3f, 0.2f);
    [SerializeField] private float smoothReturn = 3f;

    [Header("Background Layers")]
    [SerializeField] private Transform[] parallaxLayers;
    [SerializeField] private float[] depthMultipliers = { 0.7f, 0.4f, 0.2f };

    private Vector2 _mouseInput;
    private Vector3[] _initialLayerPositions;

    void Start()
    {
        // Force camera to locked position
        transform.position = lockedPosition;

        // Store initial world positions of all layers
        _initialLayerPositions = new Vector3[parallaxLayers.Length];
        for (int i = 0; i < parallaxLayers.Length; i++)
        {
            if (parallaxLayers[i] != null)
            {
                _initialLayerPositions[i] = parallaxLayers[i].position;
            }
        }

        // Auto-set multipliers if empty
        if (depthMultipliers == null || depthMultipliers.Length != parallaxLayers.Length)
        {
            depthMultipliers = new float[parallaxLayers.Length];
            for (int i = 0; i < depthMultipliers.Length; i++)
            {
                depthMultipliers[i] = 1f / (i + 1.5f);
            }
        }
    }

    void Update()
    {
        _mouseInput = new Vector2(
            (Input.mousePosition.x / Screen.width - 0.5f) * 2f,
            (Input.mousePosition.y / Screen.height - 0.5f) * 2f
        );

        ApplyParallax();
    }

    void ApplyParallax()
    {
        // Calculate virtual camera movement (without actually moving camera)
        Vector3 virtualOffset = new Vector3(
            Mathf.Lerp(-maxOffset.x, maxOffset.x, (_mouseInput.x + 1f) * 0.5f),
            Mathf.Lerp(-maxOffset.y, maxOffset.y, (_mouseInput.y + 1f) * 0.5f),
            0
        );

        // Move layers based on virtual offset
        for (int i = 0; i < parallaxLayers.Length; i++)
        {
            if (parallaxLayers[i] != null)
            {
                Vector3 layerMovement = new Vector3(
                    -virtualOffset.x * depthMultipliers[i],
                    -virtualOffset.y * depthMultipliers[i],
                    0
                );

                // Use world position instead of local position
                Vector3 targetPosition = _initialLayerPositions[i] + layerMovement;

                parallaxLayers[i].position = Vector3.Lerp(
                    parallaxLayers[i].position,
                    targetPosition,
                    smoothReturn * Time.deltaTime
                );
            }
        }
    }
}