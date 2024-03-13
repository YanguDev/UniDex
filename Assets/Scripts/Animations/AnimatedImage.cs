using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UniDex.Animations
{
    [RequireComponent(typeof(Image))]
    public class AnimatedImage : MonoBehaviour
    {
        [SerializeField]
        private Sprite[] spritesAnimation;
        [SerializeField]
        private int framesPerSecond;

        private Image image;
        private Coroutine animationCoroutine;

        private void OnEnable()
        {
            if (!image)
            {
                image = GetComponent<Image>();
            }

            if (spritesAnimation.Length == 0) return;

            animationCoroutine = StartCoroutine(AnimateCoroutine());
        }

        private void OnDisable()
        {
            if (animationCoroutine != null)
            {
                StopCoroutine(animationCoroutine);
            }
        }

        private IEnumerator AnimateCoroutine()
        {
            int frame = 0;
            while (gameObject.activeInHierarchy)
            {
                image.sprite = spritesAnimation[frame];

                if (++frame >= spritesAnimation.Length)
                {
                    frame = 0;
                }
                yield return new WaitForSeconds(1f / framesPerSecond);
            }
        }
    }
}
