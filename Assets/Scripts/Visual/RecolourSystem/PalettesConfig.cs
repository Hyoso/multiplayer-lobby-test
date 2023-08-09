using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PalettesConfig : GenericConfig<PalettesConfig>
{
	public List<Palette> palettes = new List<Palette>();
}
