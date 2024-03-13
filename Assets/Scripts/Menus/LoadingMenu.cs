using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniDex.Pokemons;
using UnityEngine;

namespace UniDex.Menus
{
    public class LoadingMenu : Menu
    {
        [SerializeField]
        private TMP_Text loadingText;

        public override void Open()
        {
            base.Open();

            PokemonManager.Instance.OnPokemonFetchCompleted += OnPokemonsLoaded;
            PokemonManager.Instance.OnPokemonFetchingProgressChanged += OnLoadingProgressChanged;

            if (PokemonManager.Instance.IsPokemonFetchCompleted)
            {
                OnPokemonsLoaded();
            }
        }

        public override void Close()
        {
            PokemonManager.Instance.OnPokemonFetchCompleted -= OnPokemonsLoaded;
            PokemonManager.Instance.OnPokemonFetchingProgressChanged -= OnLoadingProgressChanged;

            base.Close();
        }

        private void OnPokemonsLoaded()
        {
            MenuManager.Instance.SwitchMenu<PokemonsMenu>();
        }

        private void OnLoadingProgressChanged(int current, int total)
        {
            float progress = (float) current / total;
            loadingText.text = $"Loading Pokemons ({progress * 100:0}%)";
        }
    }
}
