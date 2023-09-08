using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BucketGameplay : BucketBase
{
	public static string EXAMPLE_KEY = "EXAMPLE_KEY";
	public static string DICT_KEY = "DICT_KEY";
	public static string LEVEL = "LEVEL";
	public static string PEN = "PEN";
	public static string PEN_COL = "PEN_COL";
	public static string AD_COUNTER = "AD_COUNTER";
	public static string NEXT_AD_COUNT = "NEXT_AD_COUNT";
	public static string TRINKET_SHOP_LEVEL = "TRINKET_SHOP_LEVEL";
	public static string PASSIVE_TRINKETS = "PASSIVE_TRINKETS";
	public static string ACTIVE_TRINKET = "ACTIVE_TRINKET";
    public static string EQUIPPED_COSMETICS = "EQUIPPED_COSMETICS";
    
    private string BUCKET_KEY = "BUCKET_GAMEPLAY";
	
	public override string GetBucketKey()
	{
		return BUCKET_KEY;
	}

	public override void AddKeys(Dictionary<string, BucketBase> keyBucketDict)
	{
		base.AddKeys(keyBucketDict);
		keyBucketDict.Add(EXAMPLE_KEY, this);
		keyBucketDict.Add(DICT_KEY, this);
		keyBucketDict.Add(LEVEL, this);
		keyBucketDict.Add(PEN, this);
		keyBucketDict.Add(PEN_COL, this);
		keyBucketDict.Add(AD_COUNTER, this);
		keyBucketDict.Add(NEXT_AD_COUNT, this);
		keyBucketDict.Add(TRINKET_SHOP_LEVEL, this);
		keyBucketDict.Add(PASSIVE_TRINKETS, this);
        keyBucketDict.Add(ACTIVE_TRINKET, this);
        keyBucketDict.Add(EQUIPPED_COSMETICS, this);
    }
}
