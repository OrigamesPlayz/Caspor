using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectOutliner : MonoBehaviour
{
    public float rayDistance = 2f;
    private Outline currentOutline;
    public Possess possess;

    void Update()
    {
        Vector3 rayDirection = transform.rotation * Quaternion.Euler(90f, 0f, 0f) * Vector3.forward;

        Ray ray = new Ray(transform.position, rayDirection);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, rayDistance))
        {
            if (hit.collider.CompareTag("Possessable"))
            {
                Outline outline = hit.collider.GetComponent<Outline>();

                if (outline == null)
                    outline = hit.collider.gameObject.AddComponent<Outline>();

                if (currentOutline != outline)
                {
                    DisableCurrentOutline();
                    currentOutline = outline;
                }

                outline.enabled = true;
            }
            else
            {
                DisableCurrentOutline();
            }
        }
        else
        {
            DisableCurrentOutline();
        }
    }


    void DisableCurrentOutline()
    {
        if (currentOutline != null && !possess.isPossessing)
        {
            currentOutline.enabled = false;
            currentOutline = null;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector3 rayDirection = transform.rotation * Quaternion.Euler(90f, 0f, 0f) * Vector3.forward;
        Gizmos.DrawRay(transform.position, rayDirection * rayDistance);
    }

    public GameObject CurrentOutlinedObject
    {
        get
        {
            return currentOutline != null ? currentOutline.gameObject : null;
        }
    }
}
