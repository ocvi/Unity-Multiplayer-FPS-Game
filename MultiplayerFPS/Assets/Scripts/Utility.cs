using UnityEngine;

public class Utility {
	//setting weapon layer on muzzle flash with looping through player model childs
	public static void SetLayerRecursively (GameObject _obj, int _newLayer)
	{
		if (_obj == null) 
			return;

		_obj.layer = _newLayer;

		foreach (Transform _child in _obj.transform) 
		{
			if (_child == null) 
				continue;

			SetLayerRecursively(_child.gameObject, _newLayer);
		}
	}
}
