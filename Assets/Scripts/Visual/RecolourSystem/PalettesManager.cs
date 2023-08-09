using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PalettesManager : Singleton<PalettesManager>
{
	protected override void Init()
	{
	}

	public Palette GetCurrentPalette()
	{
		int level = SaveSystem.Instance.GetInt(BucketGameplay.LEVEL);
		int modLvl = level % PalettesConfig.Instance.palettes.Count;
		Palette palette = PalettesConfig.Instance.palettes[modLvl];
		return palette;
	}
}
