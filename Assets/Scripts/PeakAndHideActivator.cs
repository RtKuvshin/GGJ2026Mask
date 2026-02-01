using UnityEngine;

public class PeekAndHideActivator : MonoBehaviour
{
    public PeekAndHide peekAndHide;

    public void Activate()
    {
        peekAndHide.gameObject.SetActive(true);
        peekAndHide.ActivatePAH();
    }
}