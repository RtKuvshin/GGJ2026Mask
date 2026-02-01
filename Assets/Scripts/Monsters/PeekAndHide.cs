using UnityEngine;

public class PeekAndHide : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public GameObject model;

    [Header("Radius")]
    public float outerRadius = 10f; // start peeking
    public float hideRadius = 2f;   // start hiding behind corner

    [Header("Peek Movement")]
    public Vector3 peekOffsetLocal = new Vector3(0.6f, 0f, 0f);  // peek to the side
    public Vector3 hideOffsetLocal = new Vector3(-0.6f, 0f, 0f); // move “back” behind corner
    public float moveSpeed = 4f;
    public float moveSpeedHide = 20f;
    private bool shouldHide = false;
    [HideInInspector] public bool isDisabled = false;


    // internal
    Vector3 idleLocalPos;
    Vector3 peekLocalPos;
    Vector3 hideLocalPos;

    void Start()
    {
        if (model == null) Debug.LogError("PeekAndHide: assign Model GameObject.");
        if (player == null) Debug.LogError("PeekAndHide: assign Player Transform.");

        idleLocalPos = model.transform.localPosition;
        peekLocalPos = idleLocalPos + peekOffsetLocal;
        hideLocalPos = idleLocalPos + hideOffsetLocal;
    }

    void Update()
    {
        if (player == null || model == null || isDisabled) return; // stop everything if disabled

        float dist = Vector3.Distance(player.position, model.transform.position);

        // Decide target position
        Vector3 targetPos = idleLocalPos;
        float speed = moveSpeed; // default peek/idle speed

        if (dist <= hideRadius)
        {
            targetPos = hideLocalPos;
            speed = moveSpeedHide;
            shouldHide = true;
        }
        else if (dist <= outerRadius && dist > hideRadius)
        {
            targetPos = peekLocalPos;
            speed = moveSpeed;
            shouldHide = false;
        }
        else
        {
            targetPos = idleLocalPos;
            speed = moveSpeed;
        }
        

        // Make sure the model is active unless we want to hide
        if (!model.activeSelf && !shouldHide) model.SetActive(true);

        MoveModelTowards(targetPos, speed, shouldHide);
    }


    void MoveModelTowards(Vector3 targetLocal, float speed, bool hideAtTarget)
    {
        model.transform.localPosition = Vector3.MoveTowards(model.transform.localPosition, targetLocal, speed * Time.deltaTime);

        // If we’re hiding and reached target, disable the model
        if (hideAtTarget && Vector3.Distance(model.transform.localPosition, targetLocal) < 0.01f)
        {
            model.SetActive(false);
        }
    }
    

    void OnDrawGizmosSelected()
    {
        if (model == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(model.transform.position, outerRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(model.transform.position, hideRadius);
    }
}
