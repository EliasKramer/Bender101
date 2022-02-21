using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities2D;
using Utilities2D.Extensions;

public class LinearCut {
	public Pair2 pairCut;
	public float size = 1f;

	static public LinearCut Create(Pair2 pair, float size) {
		LinearCut cut = new LinearCut();
		cut.size = size;
		cut.pairCut = pair;
		return(cut);
	}

	public Vector2List GetPointsList(float multiplier = 1f){
		float rot = pairCut.a.Atan2(pairCut.b);

		Vector2 a = pairCut.a;
		Vector2 b = pairCut.a;
		Vector2 c = pairCut.b;
		Vector2 d = pairCut.b;

		a = a.Push(rot + Mathf.PI / 4, size * multiplier);
		b = b.Push(rot - Mathf.PI / 4, size * multiplier);
		c = c.Push(rot + Mathf.PI / 4 + Mathf.PI, size * multiplier);
		d = d.Push(rot - Mathf.PI / 4 + Mathf.PI, size * multiplier);

		Vector2List result = new Vector2List(true);
		result.Add(a);
		result.Add(b);
		result.Add(c);
		result.Add(d);

		return(result);
	}
}
