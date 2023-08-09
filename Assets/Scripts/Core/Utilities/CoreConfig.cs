using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoreConfig : GenericConfig<CoreConfig>
{
	public string projectVersion = "0.0.1";

	// add any buckets here
	public List<BucketBase> projectBuckets = new List<BucketBase>
	{
		new BucketGameplay(),
		new BucketCore()
	};
}
