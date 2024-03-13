using UnityEngine;

namespace UniDex.Menus
{
    public abstract class Menu : MonoBehaviour
    {
        public virtual void Open()
        {
            gameObject.SetActive(true);
        }

        public virtual void Close()
        {
            gameObject.SetActive(false);
        }
    }
}