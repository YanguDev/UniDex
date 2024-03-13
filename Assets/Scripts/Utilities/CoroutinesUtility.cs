using System;
using System.Collections;
using UnityEngine;

namespace UniDex
{
    public static class CoroutinesUtility
    {
        public static Coroutine DelayByFrame(MonoBehaviour context, Action callback)
        {
            return context.StartCoroutine(DelayByFramesCoroutine(1, callback));
        }

        public static Coroutine DelayByFrames(MonoBehaviour context, int frames, Action callback)
        {
            return context.StartCoroutine(DelayByFramesCoroutine(frames, callback));
        }

        private static IEnumerator DelayByFramesCoroutine(int frames, Action callback)
        {
            for (int i = 0; i < frames; i++)
            {
                yield return null;
            }

            callback?.Invoke();
        }
    }
}
