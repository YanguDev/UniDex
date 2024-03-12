using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniDex.Patterns;
using Unity.VisualScripting;
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
            if (newMenu == null)
            {
                throw new SystemException($"Couldn't find menu of type {typeof(T)}");
            }

            newMenu.Open();
            currentMenu = newMenu;
            return newMenu;
        }

        public T GetMenu<T>() where T : Menu
        {
            return menus.SingleOrDefault(menu => menu is T) as T;
        }

        private void Initialize()
        {
            menus = GetComponentsInChildren<Menu>();

            foreach (Menu menu in menus)
            {
                if (menu != initialMenu)
                {
                    menu.gameObject.SetActive(false);
                }
            }

            currentMenu = initialMenu;
        }
    }
}
