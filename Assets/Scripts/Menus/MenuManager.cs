using System;
using System.Linq;
using UniDex.Patterns;
using UnityEngine;

namespace UniDex.Menus
{
    public class MenuManager : MonoSingleton<MenuManager>
    {
        [SerializeField]
        private Menu initialMenu;

        private Menu[] menus;

        private Menu currentMenu;

        protected override void Awake()
        {
            base.Awake();
            Initialize();
        }

        public T SwitchMenu<T>() where T : Menu
        {
            if (currentMenu)
            {
                currentMenu.Close();
            }

            T newMenu = GetMenu<T>();
            if (!newMenu)
            {
                throw new SystemException($"Couldn't find menu of type {typeof(T)}");
            }

            if (currentMenu != newMenu)
            {
                newMenu.Open();
                currentMenu = newMenu;
            }

            return newMenu;
        }

        public T GetMenu<T>() where T : Menu
        {
            return menus.SingleOrDefault(menu => menu is T) as T;
        }

        private void Initialize()
        {
            menus = GetComponentsInChildren<Menu>(true);

            foreach (Menu menu in menus)
            {
                menu.gameObject.SetActive(false);
            }

            currentMenu = initialMenu;

            if (currentMenu)
            {
                currentMenu.Open();
            }
        }
    }
}
