using UnityEngine;

public class StairsTrigger : MonoBehaviour
{
    private Transform[] stepTargets;

    void Awake()
    {
        Transform stepParent = transform.Find("StepTargets");
        if (stepParent != null)
        {
            int count = stepParent.childCount;
            stepTargets = new Transform[count];
            for (int i = 0; i < count; i++)
            {
                stepTargets[i] = stepParent.GetChild(i);
            }
        }
    }

}
