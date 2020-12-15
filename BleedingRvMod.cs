using MelonLoader;
using UnityEngine;

namespace Bleeding_RV
{
	internal class BleedingRvMod : MelonMod
	{

		public override void OnApplicationStart()
		{
			Debug.Log($"[{InfoAttribute.Name}] version {InfoAttribute.Version} loaded!");
		}
	}
}
