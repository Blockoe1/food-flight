/*****************************************************************************
// File Name : BurgerScript.cs
// Author : Andrew Higgins
// Creation Date : 2/3/2026
// Last Modified : 2/3/2026
//
// Brief Description :  Manages the burger object.
*****************************************************************************/
using CustomAttributes;
using System.Collections;
using UnityEngine;

public class BurgerScript : MonoBehaviour
{
    [SerializeField] private LayerMask ignorePlayersMask;
    [SerializeField, Tooltip("The array of colliders to disable when the burger collision is disabled.")] 
    private Collider[] disabledColliders;

    private bool grabDisabled;

    #region Component References
    [Header("Components")]
    [SerializeReference, ReadOnly] protected Rigidbody rb;

    [ContextMenu("Get Component References")]
    private void Reset()
    {
        rb = GetComponent<Rigidbody>();
    }
    #endregion

    #region Properties
    public Rigidbody Body => rb;
    #endregion

    /// <summary>
    /// Disables grabbing for a certain amount of time.
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    public void DisableGrabbing()
    {
        // Prevent duplicate coroutines.
        if (grabDisabled) { return; }
        grabDisabled = true;

        // Disable all colliders in the disable array.
        foreach(Collider collider in disabledColliders)
        {
            collider.excludeLayers = collider.excludeLayers | ignorePlayersMask;
        }
    }

    /// <summary>
    /// Schedules the burger to re-enable it's grabbing.
    /// </summary>
    /// <param name="time"></param>
    public void ScheduleGrabReenable(float time)
    {
        StartCoroutine(DisableGrabbingRoutine(time));
    }
    public IEnumerator DisableGrabbingRoutine(float time)
    {
        yield return new WaitForSeconds(time);
        grabDisabled = false;

        // Re-Enable all colliders in the disable array.
        foreach (Collider collider in disabledColliders)
        {
            collider.excludeLayers = collider.excludeLayers & ~ignorePlayersMask;
        }
    }
}
