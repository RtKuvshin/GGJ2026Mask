using UnityEngine;

public class PeekAndHide : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public GameObject model;
    public ScarenessLevel scarenessLevel; // optional, for fear effects

    [Header("Activation")]
    [HideInInspector] public bool isActivated = false;

    [Header("Radius")]
    public float outerRadius = 10f; // start peeking
    public float hideRadius = 2f;   // start hiding behind corner

    [Header("Peek Movement")] 
    public Vector3 peekOffsetLocal = new Vector3(0.6f, 0f, 0f);  // peek to the side
    public Vector3 hideOffsetLocal = new Vector3(-0.6f, 0f, 0f); // move behind corner
    public float moveSpeed = 4f;
    public float moveSpeedHide = 20f;
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

    public void ActivatePAH()
    {
        isActivated = true;
        model.SetActive(true); // make him visible immediately when activated
    }

    void Update()
    {
        if (player == null || model == null || isDisabled || !isActivated) return;

        float dist = Vector3.Distance(player.position, model.transform.position);

        Vector3 targetPos = idleLocalPos;
        float speed = moveSpeed;

        if (dist <= hideRadius)
        {
            targetPos = hideLocalPos;
            speed = moveSpeedHide;
            model.SetActive(false); // hide immediately
        }
        else if (dist <= outerRadius)
        {
            targetPos = peekLocalPos;
            speed = moveSpeed;
            model.SetActive(true); // peek
        }
        else
        {
            targetPos = idleLocalPos;
            speed = moveSpeed;
            model.SetActive(true); // idle
        }

        MoveModelTowards(targetPos, speed);
    }

    void MoveModelTowards(Vector3 targetLocal, float speed)
    {
        model.transform.localPosition = Vector3.MoveTowards(model.transform.localPosition, targetLocal, speed * Time.deltaTime);
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
