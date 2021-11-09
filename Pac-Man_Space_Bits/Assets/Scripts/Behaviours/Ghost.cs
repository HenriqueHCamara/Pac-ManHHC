using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    [SerializeField] GhostsSet ghostsSet;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        AddToSet();
    }

    private void AddToSet()
    {
        if (ghostsSet != null && !ghostsSet.Items.Contains(this))
        {
            ghostsSet.Add(this);
        }
    }

    private void OnDisable()
    {
        if (ghostsSet != null)
        {
            ghostsSet.Remove(this);
        }
    }
}
