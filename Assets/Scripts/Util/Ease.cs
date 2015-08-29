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
	easeInBack,
	easeOutBack,
	easeInOutBack,
	easeInElastic,
	easeOutElastic,
	easeInOutElastic,
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
	private const float HalfPi = Mathf.PI * .5f;
	private const float DoublePi = Mathf.PI * 2f;

	public static float Run(EaseType easeType, float from, float to, float time) {
		switch (easeType) {
			case EaseType.easeInQuad:
				return easeInQuad(from, to, time);
			case EaseType.easeOutQuad:
				return easeOutQuad(from, to, time);
			case EaseType.easeInOutQuad:
				return easeInOutQuad(from, to, time);
			case EaseType.easeInCubic:
				return easeInCubic(from, to, time);
			case EaseType.easeOutCubic:
				return easeOutCubic(from, to, time);
			case EaseType.easeInOutCubic:
				return easeInOutCubic(from, to, time);
			case EaseType.easeInQuart:
				return easeInQuart(from, to, time);
			case EaseType.easeOutQuart:
				return easeOutQuart(from, to, time);
			case EaseType.easeInOutQuart:
				return easeInOutQuart(from, to, time);
			case EaseType.easeInQuint:
				return easeInQuint(from, to, time);
			case EaseType.easeOutQuint:
				return easeOutQuint(from, to, time);
			case EaseType.easeInOutQuint:
				return easeInOutQuint(from, to, time);
			case EaseType.easeInSine:
				return easeInSine(from, to, time);
			case EaseType.easeOutSine:
				return easeOutSine(from, to, time);
			case EaseType.easeInOutSine:
				return easeInOutSine(from, to, time);
			case EaseType.easeInExpo:
				return easeInExpo(from, to, time);
			case EaseType.easeOutExpo:
				return easeOutExpo(from, to, time);
			case EaseType.easeInOutExpo:
				return easeInOutExpo(from, to, time);
			case EaseType.easeInCirc:
				return easeInCirc(from, to, time);
			case EaseType.easeOutCirc:
				return easeOutCirc(from, to, time);
			case EaseType.easeInOutCirc:
				return easeInOutCirc(from, to, time);
			case EaseType.linear:
				return linear(from, to, time);
			case EaseType.spring:
				return spring(from, to, time);
			case EaseType.easeInBounce:
				return easeInBounce(from, to, time);
			case EaseType.easeOutBounce:
				return easeOutBounce(from, to, time);
			case EaseType.easeInOutBounce:
				return easeInOutBounce(from, to, time);
			case EaseType.easeInBack:
				return easeInBack(from, to, time);
			case EaseType.easeOutBack:
				return easeOutBack(from, to, time);
			case EaseType.easeInOutBack:
				return easeInOutBack(from, to, time);
			case EaseType.easeInElastic:
				return easeInElastic(from, to, time);
			case EaseType.easeOutElastic:
				return easeOutElastic(from, to, time);
			case EaseType.easeInOutElastic:
				return easeInOutElastic(from, to, time);
		}

		return 0;
	}


	static public float linear(float from, float to, float time){
		return Mathf.Lerp(from, to, time);
	}

	static public float spring(float from, float to, float time){
		time = Mathf.Clamp01(time);
		time = (Mathf.Sin(time * Mathf.PI * (.2f + 2.5f * time * time * time)) * Mathf.Pow(1f - time, 2.2f) + time) * (1f + (1.2f * (1f - time)));
		return from + (to - from) * time;
	}

	static public float easeInQuad(float from, float to, float time){
		return Mathf.Lerp(from, to, time * time);
	}

	static public float easeOutQuad(float from, float to, float time){
		return Mathf.Lerp(from, to, -time * (time - 2f));
	}

	static public float easeInOutQuad(float from, float to, float time){
		if ((time /= .5f) < 1f)
			return Mathf.Lerp(from, to, .5f * time * time);
		return Mathf.Lerp(from, to, -.5f * (((--time) * (time - 2f) - 1f)));
	}

	static public float easeInCubic(float from, float to, float time){
		return Mathf.Lerp(from, to, time * time * time);
	}

	static public float easeOutCubic(float from, float to, float time){
		return Mathf.Lerp(from, to, (time -= 1f) * time * time + 1f);
	}

	static public float easeInOutCubic(float from, float to, float time){
		if ((time /= .5f) < 1f)
			return Mathf.Lerp(from, to, .5f * time * time * time);
		return Mathf.Lerp(from, to, .5f * ((time -= 2) * time * time + 2f));
	}

	static public float easeInQuart(float from, float to, float time){
		return Mathf.Lerp(from, to, time * time * time * time);
	}

	static public float easeOutQuart(float from, float to, float time){
		return Mathf.Lerp(from, to, -((time -= 1f) * time * time * time - 1f));
	}

	static public float easeInOutQuart(float from, float to, float time){
		if ((time /= .5f) < 1f)
			return Mathf.Lerp(from, to, .5f * time * time * time * time);
		return Mathf.Lerp(from, to, -.5f * ((time -= 2f) * time * time * time - 2f));
	}

	static public float easeInQuint(float from, float to, float time){
		return Mathf.Lerp(from, to, time * time * time * time * time);
	}

	static public float easeOutQuint(float from, float to, float time){
		return Mathf.Lerp(from, to, (time -= 1f) * time * time * time * time + 1f);
	}

	static public float easeInOutQuint(float from, float to, float time){
		if ((time /= .5f) < 1f)
			return Mathf.Lerp(from, to, .5f * time * time * time * time * time);
		return Mathf.Lerp(from, to, .5f * ((time -= 2f) * time * time * time * time + 2f));
	}

	static public float easeInSine(float from, float to, float time){
		return Mathf.Lerp(from, to, 1f - Mathf.Cos(time * HalfPi));
	}

	static public float easeOutSine(float from, float to, float time){
		return Mathf.Lerp(from, to, Mathf.Sin(time * HalfPi));
	}

	static public float easeInOutSine(float from, float to, float time){
		return Mathf.Lerp(from, to, .5f * (1f - Mathf.Cos(Mathf.PI * time)));
	}

	static public float easeInExpo(float from, float to, float time){
		return Mathf.Lerp(from, to, Mathf.Pow(2f, 10f * (time - 1f)));
	}

	static public float easeOutExpo(float from, float to, float time){
		return Mathf.Lerp(from, to, -Mathf.Pow(2f, -10f * time) + 1f);
	}

	static public float easeInOutExpo(float from, float to, float time){
		if ((time /= .5f) < 1f)
			return Mathf.Lerp(from, to, .5f * Mathf.Pow(2f, 10f * (time - 1f)));
		return Mathf.Lerp(from, to, .5f * (-Mathf.Pow(2f, -10f * --time) + 2f));
	}

	static public float easeInCirc(float from, float to, float time){
		return Mathf.Lerp(from, to, -(Mathf.Sqrt(1f - time * time) - 1f));
	}

	static public float easeOutCirc(float from, float to, float time){
		return Mathf.Lerp(from, to, Mathf.Sqrt(1f - (time -= 1f) * time));
	}

	static public float easeInOutCirc(float from, float to, float time){
		if ((time /= .5f) < 1f)
			return Mathf.Lerp(from, to, -.5f * (Mathf.Sqrt(1f - time * time) - 1f));
		return Mathf.Lerp(from, to, .5f * (Mathf.Sqrt(1f - (time -= 2f) * time) + 1f));
	}

	static public float easeInBounce(float from, float to, float time){
		to -= from;
		return to - easeOutBounce(0f, to, 1f - time) + from;
	}

	static public float easeOutBounce(float from, float to, float time){
		to -= from;
		if (time < (1f / 2.75f))
			return to * (7.5625f * time * time) + from;
		if (time < (2f / 2.75f))
			return to * (7.5625f * (time -= (1.5f / 2.75f)) * time + .75f) + from;
		if (time < (2.5f / 2.75f))
			return to * (7.5625f * (time -= (2.25f / 2.75f)) * time + .9375f) + from;
		return to * (7.5625f * (time -= (2.625f / 2.75f)) * time + .984375f) + from;
	}

	static public float easeInOutBounce(float from, float to, float time){
		to -= from;
		if (time < .5f)
			return easeInBounce(0f, to, time * 2f) * .5f + from;
		return easeOutBounce(0f, to, time * 2f - 1f) * .5f + to * .5f + from;
	}

	static public float easeInBack(float from, float to, float time){
		const float s = 1.70158f;
		to -= from;
		return to * time * time * ((s + 1f) * time - s) + from;
	}

	static public float easeOutBack(float from, float to, float time){
		const float s = 1.70158f;
		to -= from;
		return to * (--time * time * ((s + 1f) * time + s) + 1f) + from;
	}

	static public float easeInOutBack(float from, float to, float time){
		const float s = 1.70158f * 1.525f;
		to -= from;
		if ((time /= .5f) < 1f)
			return to * .5f * (time * time * ((s + 1f) * time - s)) + from;
		return to * .5f * ((time -= 2) * time * ((s + 1f) * time  + s) + 2f) + from;
	}
	
	static public float easeInElastic(float from, float to, float time){
		const float p = .3f;
		const float s = p / 4f;
		to -= from;
		return to * -(Mathf.Pow(2f, 10f * (time -= 1f)) * Mathf.Sin((time - s) * DoublePi / p)) + from;
	}		

	static public float easeOutElastic(float from, float to, float time){
		const float p = .3f;
		const float s = p / 4f;
		to -= from;
		return to * Mathf.Pow(2f, -10f * time) * Mathf.Sin((time - s) * DoublePi / p) + to + from;
	}		
	
	static public float easeInOutElastic(float from, float to, float time){
		const float p = .3f * 1.5f;
		const float s = p / 4f;
		to -= from;
		if ((time /= .5f) < 1f)
			return -.5f * (to * Mathf.Pow(2f, 10f * (time -= 1f)) * Mathf.Sin((time - s) * DoublePi / p)) + from;
		return to * Mathf.Pow(2f, -10f * (time -= 1f)) * Mathf.Sin((time - s) * DoublePi / p) * .5f + to + from;
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
