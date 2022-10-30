using UnityEngine;

public class MoveBall : MonoBehaviour
{
    private Rigidbody rbBall;
    [SerializeField] GameObject indicatorDirection;

    private void Start()
    {
        rbBall = GetComponent<Rigidbody>();
    }

    public void StartActionBall(float throwForce)
    {
        Vector3 loocDirection = (indicatorDirection.transform.position - transform.position).normalized;

        rbBall.AddForce(loocDirection * throwForce, ForceMode.Impulse);
    }
}
