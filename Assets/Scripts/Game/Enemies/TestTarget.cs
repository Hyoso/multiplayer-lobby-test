using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTarget : TargetBase
{
    protected override void PlayDeathSequence()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.enabled = false;
    }
}
