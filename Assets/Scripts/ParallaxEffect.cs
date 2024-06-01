using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ParallaxEffect : MonoBehaviour
{
    [SerializeField] private float parallaxSpeedX;
    [SerializeField] private float parallaxSpeedY;
    private Transform _cameraTransform;
    private float _startPositionX, _startPositionY;
    private float _spriteSizeX;

    private void Start()
    {
        if (Camera.main != null) _cameraTransform = Camera.main.transform;
        _startPositionX = transform.position.x;
        _startPositionY = transform.position.y;
        _spriteSizeX = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    private void Update()
    {
        float relativeDistX = _cameraTransform.position.x * parallaxSpeedX;
        float relativeDistY = _cameraTransform.position.y * parallaxSpeedY;
        transform.position = new Vector3(_startPositionX + relativeDistX, _startPositionY + relativeDistY, transform.position.z);
        
        float relativeCameraDistX = _cameraTransform.position.x * (1 - parallaxSpeedX);
        if (relativeCameraDistX > _startPositionX + _spriteSizeX)
        {
            _startPositionX += _spriteSizeX;
        }
        else if (relativeCameraDistX < _startPositionX - _spriteSizeX)
        {
            _startPositionX -= _spriteSizeX;
        }
    }
}
