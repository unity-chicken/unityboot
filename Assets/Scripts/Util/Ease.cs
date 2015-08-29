using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public enum EaseType {
	easeInQuad,
	easeOutQuad,
	easeInOutQuad,
	easeInCubic,
	easeOutCubic,
	easeInOutCubic,
	easeInQuart,
	easeOutQuart,
	easeInOutQuart,
	easeInQuint,
	easeOutQuint,
	easeInOutQuint,
	easeInSine,
	easeOutSine,
	easeInOutSine,
	easeInExpo,
	easeOutExpo,
	easeInOutExpo,
	easeInCirc,
	easeOutCirc,
	easeInOutCirc,
	linear,
	spring,
	easeInBounce,
	easeOutBounce,
	easeInOutBounce,
	easeOutStoneBounce,
	easeInBack,
	easeOutBack,
	easeInOutBack,
	easeInElastic,
	easeOutElastic,
	easeInOutElastic,
	blinkUp,
	blinkDown,
}

public enum EaseDirection { 
	Forward = 0, 
	Backward, 
	Alternative
};

public class EaseRunner {
	float duration = 1f;
	float deltaTimes = 0;
	float process = 0;
	float easeOld = 0;
	EaseType easeType = EaseType.easeOutQuad;

	public EaseRunner(EaseType easeType, float duration) {
		this.easeType = easeType;
		this.duration = duration;
	}

	public float Run() {
		deltaTimes += Time.deltaTime;
		deltaTimes = Mathf.Min(duration, deltaTimes);
		float oldProcess = process;
		process = deltaTimes / duration;
		return Ease.Run(easeType, 0, 1, process);
	}

	public float RunDelta() {
		float ease = Run();
		float delta = ease - easeOld;
		easeOld = ease;
		return delta;
	}

	public bool IsDone() {
		return process >= 1f;
	}

	public bool IsPlaying() {
		return process < 1f;
	}
}

public class Ease {

	public static float Run(EaseType easeType, float start, float end, float value) {
		switch (easeType) {
			case EaseType.easeInQuad:
				return easeInQuad(start, end, value);
			case EaseType.easeOutQuad:
				return easeOutQuad(start, end, value);
			case EaseType.easeInOutQuad:
				return easeInOutQuad(start, end, value);
			case EaseType.easeInCubic:
				return easeInCubic(start, end, value);
			case EaseType.easeOutCubic:
				return easeOutCubic(start, end, value);
			case EaseType.easeInOutCubic:
				return easeInOutCubic(start, end, value);
			case EaseType.easeInQuart:
				return easeInQuart(start, end, value);
			case EaseType.easeOutQuart:
				return easeOutQuart(start, end, value);
			case EaseType.easeInOutQuart:
				return easeInOutQuart(start, end, value);
			case EaseType.easeInQuint:
				return easeInQuint(start, end, value);
			case EaseType.easeOutQuint:
				return easeOutQuint(start, end, value);
			case EaseType.easeInOutQuint:
				return easeInOutQuint(start, end, value);
			case EaseType.easeInSine:
				return easeInSine(start, end, value);
			case EaseType.easeOutSine:
				return easeOutSine(start, end, value);
			case EaseType.easeInOutSine:
				return easeInOutSine(start, end, value);
			case EaseType.easeInExpo:
				return easeInExpo(start, end, value);
			case EaseType.easeOutExpo:
				return easeOutExpo(start, end, value);
			case EaseType.easeInOutExpo:
				return easeInOutExpo(start, end, value);
			case EaseType.easeInCirc:
				return easeInCirc(start, end, value);
			case EaseType.easeOutCirc:
				return easeOutCirc(start, end, value);
			case EaseType.easeInOutCirc:
				return easeInOutCirc(start, end, value);
			case EaseType.linear:
				return linear(start, end, value);
			case EaseType.spring:
				return spring(start, end, value);
			case EaseType.blinkUp:
				return blinkUp(start, end, value);
			case EaseType.blinkDown:
				return blinkDown(start, end, value);
			case EaseType.easeInBounce:
				return easeInBounce(start, end, value);
			case EaseType.easeOutBounce:
				return easeOutBounce(start, end, value);
			case EaseType.easeInOutBounce:
				return easeInOutBounce(start, end, value);
			case EaseType.easeOutStoneBounce:
				return easeOutStoneBounce(start, end, value);
			case EaseType.easeInBack:
				return easeInBack(start, end, value);
			case EaseType.easeOutBack:
				return easeOutBack(start, end, value);
			case EaseType.easeInOutBack:
				return easeInOutBack(start, end, value);
			case EaseType.easeInElastic:
				return easeInElastic(start, end, value);
			case EaseType.easeOutElastic:
				return easeOutElastic(start, end, value);
			case EaseType.easeInOutElastic:
				return easeInOutElastic(start, end, value);
		}

		return 0;
	}


	static public float linear(float start, float end, float value){
		return Mathf.Lerp(start, end, value);
	}
	
	static public float clerp(float start, float end, float value){
		float min = 0.0f;
		float max = 360.0f;
		float half = Mathf.Abs((max - min) * 0.5f);
		float retval = 0.0f;
		float diff = 0.0f;
		if ((end - start) < -half){
			diff = ((max - start) + end) * value;
			retval = start + diff;
		}else if ((end - start) > half){
			diff = -((max - end) + start) * value;
			retval = start + diff;
		}else retval = start + (end - start) * value;
		return retval;
	}

	static public float spring(float start, float end, float value){
		value = Mathf.Clamp01(value);
		value = (Mathf.Sin(value * Mathf.PI * (0.2f + 2.5f * value * value * value)) * Mathf.Pow(1f - value, 2.2f) + value) * (1f + (1.2f * (1f - value)));
		return start + (end - start) * value;
	}

	static public float blinkUp(float start, float end, float value){
		float center = (end-start) / 2;
		if (value < center) {
			return 0;
		}

		return 1;
	}

	static public float blinkDown(float start, float end, float value){
		float center = (end-start) / 2;
		if (value < center) {
			return 1;
		}

		return 0;
	}

	static public float easeInQuad(float start, float end, float value){
		end -= start;
		return end * value * value + start;
	}

	static public float easeOutQuad(float start, float end, float value){
		end -= start;
		return -end * value * (value - 2) + start;
	}

	static public float easeInOutQuad(float start, float end, float value){
		value /= .5f;
		end -= start;
		if (value < 1) return end * 0.5f * value * value + start;
		value--;
		return -end * 0.5f * (value * (value - 2) - 1) + start;
	}

	static public float easeInCubic(float start, float end, float value){
		end -= start;
		return end * value * value * value + start;
	}

	static public float easeOutCubic(float start, float end, float value){
		value--;
		end -= start;
		return end * (value * value * value + 1) + start;
	}

	static public float easeInOutCubic(float start, float end, float value){
		value /= .5f;
		end -= start;
		if (value < 1) return end * 0.5f * value * value * value + start;
		value -= 2;
		return end * 0.5f * (value * value * value + 2) + start;
	}

	static public float easeInQuart(float start, float end, float value){
		end -= start;
		return end * value * value * value * value + start;
	}

	static public float easeOutQuart(float start, float end, float value){
		value--;
		end -= start;
		return -end * (value * value * value * value - 1) + start;
	}

	static public float easeInOutQuart(float start, float end, float value){
		value /= .5f;
		end -= start;
		if (value < 1) return end * 0.5f * value * value * value * value + start;
		value -= 2;
		return -end * 0.5f * (value * value * value * value - 2) + start;
	}

	static public float easeInQuint(float start, float end, float value){
		end -= start;
		return end * value * value * value * value * value + start;
	}

	static public float easeOutQuint(float start, float end, float value){
		value--;
		end -= start;
		return end * (value * value * value * value * value + 1) + start;
	}

	static public float easeInOutQuint(float start, float end, float value){
		value /= .5f;
		end -= start;
		if (value < 1) return end * 0.5f * value * value * value * value * value + start;
		value -= 2;
		return end * 0.5f * (value * value * value * value * value + 2) + start;
	}

	static public float easeInSine(float start, float end, float value){
		end -= start;
		return -end * Mathf.Cos(value * (Mathf.PI * 0.5f)) + end + start;
	}

	static public float easeOutSine(float start, float end, float value){
		end -= start;
		return end * Mathf.Sin(value * (Mathf.PI * 0.5f)) + start;
	}

	static public float easeInOutSine(float start, float end, float value){
		end -= start;
		return -end * 0.5f * (Mathf.Cos(Mathf.PI * value) - 1) + start;
	}

	static public float easeInExpo(float start, float end, float value){
		end -= start;
		return end * Mathf.Pow(2, 10 * (value - 1)) + start;
	}

	static public float easeOutExpo(float start, float end, float value){
		end -= start;
		return end * (-Mathf.Pow(2, -10 * value ) + 1) + start;
	}

	static public float easeInOutExpo(float start, float end, float value){
		value /= .5f;
		end -= start;
		if (value < 1) return end * 0.5f * Mathf.Pow(2, 10 * (value - 1)) + start;
		value--;
		return end * 0.5f * (-Mathf.Pow(2, -10 * value) + 2) + start;
	}

	static public float easeInCirc(float start, float end, float value){
		end -= start;
		return -end * (Mathf.Sqrt(1 - value * value) - 1) + start;
	}

	static public float easeOutCirc(float start, float end, float value){
		value--;
		end -= start;
		return end * Mathf.Sqrt(1 - value * value) + start;
	}

	static public float easeInOutCirc(float start, float end, float value){
		value /= .5f;
		end -= start;
		if (value < 1) return -end * 0.5f * (Mathf.Sqrt(1 - value * value) - 1) + start;
		value -= 2;
		return end * 0.5f * (Mathf.Sqrt(1 - value * value) + 1) + start;
	}

	static public float easeInBounce(float start, float end, float value){
		end -= start;
		float d = 1f;
		return end - easeOutBounce(0, end, d-value) + start;
	}

	static public float easeOutBounce(float start, float end, float value){
		value /= 1f;
		end -= start;
		if (value < (1 / 2.75f)){
			return end * (7.5625f * value * value) + start;
		}else if (value < (2 / 2.75f)){
			value -= (1.5f / 2.75f);
			return end * (7.5625f * (value) * value + .75f) + start;
		}else if (value < (2.5 / 2.75)){
			value -= (2.25f / 2.75f);
			return end * (7.5625f * (value) * value + .9375f) + start;
		}else{
			value -= (2.625f / 2.75f);
			return end * (7.5625f * (value) * value + .984375f) + start;
		}
	}


	static public float easeOutStoneBounce(float start, float end, float value){
		value /= 1f;
		//end -= start;

		if (value < (2 / 2.75f)){ // 0.36
			return (1.89071f * value * value);
		}else{
			value -= (2.375f / 2.75f); // 0.954
			return (1.89071f * (value) * value + .964842f);
		}
	}


	static public float easeInOutBounce(float start, float end, float value){
		end -= start;
		float d = 1f;
		if (value < d* 0.5f) return easeInBounce(0, end, value*2) * 0.5f + start;
		else return easeOutBounce(0, end, value*2-d) * 0.5f + end*0.5f + start;
	}

	static public float easeInBack(float start, float end, float value){
		end -= start;
		value /= 1;
		float s = 1.70158f;
		return end * (value) * value * ((s + 1) * value - s) + start;
	}

	static public float easeOutBack(float start, float end, float value){
		float s = 1.70158f;
		end -= start;
		value = (value) - 1;
		return end * ((value) * value * ((s + 1) * value + s) + 1) + start;
	}

	static public float easeInOutBack(float start, float end, float value){
		float s = 1.70158f;
		end -= start;
		value /= .5f;
		if ((value) < 1){
			s *= (1.525f);
			return end * 0.5f * (value * value * (((s) + 1) * value - s)) + start;
		}
		value -= 2;
		s *= (1.525f);
		return end * 0.5f * ((value) * value * (((s) + 1) * value + s) + 2) + start;
	}
	
	static public float easeInElastic(float start, float end, float value){
		end -= start;
		
		float d = 1f;
		float p = d * .3f;
		float s = 0;
		float a = 0;
		
		if (value == 0) return start;
		
		if ((value /= d) == 1) return start + end;
		
		if (a == 0f || a < Mathf.Abs(end)){
			a = end;
			s = p / 4;
			}else{
			s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
		}
		
		return -(a * Mathf.Pow(2, 10 * (value-=1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p)) + start;
	}		

	static public float easeOutElastic(float start, float end, float value){
		end -= start;
		
		float d = 1f;
		float p = d * .3f;
		float s = 0;
		float a = 0;
		
		if (value == 0) return start;
		
		if ((value /= d) == 1) return start + end;
		
		if (a == 0f || a < Mathf.Abs(end)){
			a = end;
			s = p * 0.25f;
			}else{
			s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
		}
		
		return (a * Mathf.Pow(2, -10 * value) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p) + end + start);
	}		
	
	static public float easeInOutElastic(float start, float end, float value){
		end -= start;
		
		float d = 1f;
		float p = d * .3f;
		float s = 0;
		float a = 0;
		
		if (value == 0) return start;
		
		if ((value /= d*0.5f) == 2) return start + end;
		
		if (a == 0f || a < Mathf.Abs(end)){
			a = end;
			s = p / 4;
			}else{
			s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
		}
		
		if (value < 1) return -0.5f * (a * Mathf.Pow(2, 10 * (value-=1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p)) + start;
		return a * Mathf.Pow(2, -10 * (value-=1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p) * 0.5f + end + start;
	}


	/**
	* Sequence of eleapsedTimes until elapsedTime is >= duration.
	*
	* Note: elapsedTimes are calculated using the value of Time.deltatTime each
	* time a value is requested.
	*/
	static Vector3 Identity(Vector3 v) {
		return v;
	}
 
	static Vector3 TransformDotPosition(Transform t) {
		return t.position;
	}
 
	public delegate Vector3 ToVector3<T>(T v);
	public delegate float Function(float a, float b, float c, float d);
 

	/**
	 * Returns sequence generator from the first node, through each control point,
	 * and to the last node. N points are generated between each node (slices)
	 * using Catmull-Rom.
	 */
	public static IEnumerable<Vector3> NewCatmullRom(Transform[] nodes, int slices, bool loop) {
		return NewCatmullRom<Transform>(nodes, TransformDotPosition, slices, loop);
	}
 
	/**
	 * A Vector3[] variation of the Transform[] NewCatmullRom() function.
	 * Same functionality but using Vector3s to define curve.
	 */
	public static IEnumerable<Vector3> NewCatmullRom(Vector3[] points, int slices, bool loop) {
		return NewCatmullRom<Vector3>(points, Identity, slices, loop);
	}
 
	/**
	 * Generic catmull-rom spline sequence generator used to implement the
	 * Vector3[] and Transform[] variants. Normally you would not use this
	 * function directly.
	 */
	static IEnumerable<Vector3> NewCatmullRom<T>(IList nodes, ToVector3<T> toVector3, int slices, bool loop) {
		// need at least two nodes to spline between
		if (nodes.Count >= 2) {
 
			// yield the first point explicitly, if looping the first point
			// will be generated again in the step for loop when interpolating
			// from last point back to the first point
			yield return toVector3((T)nodes[0]);
 
			int last = nodes.Count - 1;
			for (int current = 0; loop || current < last; current++) {
				// wrap around when looping
				if (loop && current > last) {
					current = 0;
				}
				// handle edge cases for looping and non-looping scenarios
				// when looping we wrap around, when not looping use start for previous
				// and end for next when you at the ends of the nodes array
				int previous = (current == 0) ? ((loop) ? last : current) : current - 1;
				int start = current;
				int end = (current == last) ? ((loop) ? 0 : current) : current + 1;
				int next = (end == last) ? ((loop) ? 0 : end) : end + 1;
 
				// adding one guarantees yielding at least the end point
				int stepCount = slices + 1;
				for (int step = 1; step <= stepCount; step++) {
					yield return CatmullRom(toVector3((T)nodes[previous]),
									 toVector3((T)nodes[start]),
									 toVector3((T)nodes[end]),
									 toVector3((T)nodes[next]),
									 step, stepCount);
				}
			}
		}
	}
 
	/**
	 * A Vector3 Catmull-Rom spline. Catmull-Rom splines are similar to bezier
	 * splines but have the useful property that the generated curve will go
	 * through each of the control points.
	 *
	 * NOTE: The NewCatmullRom() functions are an easier to use alternative to this
	 * raw Catmull-Rom implementation.
	 *
	 * @param previous the point just before the start point or the start point
	 *				 itself if no previous point is available
	 * @param start generated when elapsedTime == 0
	 * @param end generated when elapsedTime >= duration
	 * @param next the point just after the end point or the end point itself if no
	 *			 next point is available
	 */
	static Vector3 CatmullRom(Vector3 previous, Vector3 start, Vector3 end, Vector3 next, 
								float elapsedTime, float duration) {
		// References used:
		// p.266 GemsV1
		//
		// tension is often set to 0.5 but you can use any reasonable value:
		// http://www.cs.cmu.edu/~462/projects/assn2/assn2/catmullRom.pdf
		//
		// bias and tension controls:
		// http://local.wasp.uwa.edu.au/~pbourke/miscellaneous/interpolation/
 
		float percentComplete = elapsedTime / duration;
		float percentCompleteSquared = percentComplete * percentComplete;
		float percentCompleteCubed = percentCompleteSquared * percentComplete;
 
		return previous * (-0.5f * percentCompleteCubed +
								   percentCompleteSquared -
							0.5f * percentComplete) +
				start   * ( 1.5f * percentCompleteCubed +
						   -2.5f * percentCompleteSquared + 1.0f) +
				end	 * (-1.5f * percentCompleteCubed +
							2.0f * percentCompleteSquared +
							0.5f * percentComplete) +
				next	* ( 0.5f * percentCompleteCubed -
							0.5f * percentCompleteSquared);
	}
}
