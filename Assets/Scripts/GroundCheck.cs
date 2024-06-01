using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    private bool _isGrounded = false;
    private float platformHalfWidth, playerHalfWidth, distanceX;

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Ground") || other.gameObject.CompareTag("Platform"))
        {
            _isGrounded = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Ground") || other.gameObject.CompareTag("Platform"))
        {
            _isGrounded = false;
        }
    }

    public bool GetIsGrounded()
    {
        return _isGrounded;
    }

    private bool IsCollidingHorizontally(Collider2D other)
    {
        distanceX = Mathf.Abs(transform.position.x - other.transform.position.x);

        playerHalfWidth = GetComponent<Collider2D>().bounds.extents.x;
        platformHalfWidth = other.bounds.extents.x;

        return distanceX < playerHalfWidth + platformHalfWidth;
    }
}
