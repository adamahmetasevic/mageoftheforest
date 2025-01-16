using UnityEngine;

public class FrozenLever : LeverBase
{
    [SerializeField] private Animator leverAnimator;
    private bool isFrozen = true;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name.Contains("Fireball") && isFrozen)
        {
            UnfreezeLever();
            Destroy(collision.gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && Input.GetKeyDown(KeyCode.F) && !isFrozen && !isActivated)
        {
            ActivateLever();
        }
    }

    public override void ActivateLever()
    {
        isActivated = true;
        if (leverAnimator != null)
        {
            leverAnimator.SetTrigger("Activate");
        }
        else
        {
            //Debug.LogWarning("Lever Animator is not assigned!");
        }
       // Debug.Log("Frozen lever activated!");
        OpenDoor();
    }

    private void UnfreezeLever()
    {
        isFrozen = false;
        //Debug.Log("Lever unfrozen!");
        if (leverAnimator != null)
        {
            leverAnimator.SetTrigger("Unfreeze");
        }
        else
        {
            //Debug.LogWarning("Lever Animator is not assigned!");
        }
    }
}