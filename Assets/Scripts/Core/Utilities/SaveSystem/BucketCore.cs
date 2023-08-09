using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BucketCore : BucketBase
{
	public static string GDPR_ACCEPTED = "GDPR_ACCEPTED";

    private string BUCKET_KEY = "BUCKET_CORE";

    public override string GetBucketKey()
    {
        return BUCKET_KEY;
    }
    public override void AddKeys(Dictionary<string, BucketBase> keyBucketDict)
    {
        base.AddKeys(keyBucketDict);
        keyBucketDict.Add(GDPR_ACCEPTED, this);
    }
}
