using TMPro;
using UniDex.Pokemons;
using UnityEngine;
using UnityEngine.UI;

namespace UniDex.Menus
{
    public class LoadingMenu : Menu
    {
        [SerializeField]
        private TMP_Text loadingText;
        [SerializeField]
        private Button retryButton;
        [SerializeField]
        private GameObject loadingIndicator;

        public override void Open()
        {
            base.Open();

            loadingIndicator.SetActive(true);
            retryButton.gameObject.SetActive(false);

            PokemonManager.Instance.OnPokemonFetchCompleted += OnPokemonsFetchCompleted;
            PokemonManager.Instance.OnPokemonFetchingProgressChanged += OnLoadingProgressChanged;
            retryButton.onClick.AddListener(Retry);

            PokemonManager.Instance.FetchPokemons();
        }

        public override void Close()
        {
            PokemonManager.Instance.OnPokemonFetchCompleted -= OnPokemonsFetchCompleted;
            PokemonManager.Instance.OnPokemonFetchingProgressChanged -= OnLoadingProgressChanged;
            retryButton.onClick.RemoveListener(Retry);

            base.Close();
        }

        private void OnPokemonsFetchCompleted(bool success)
        {
            if (success)
            {
                MenuManager.Instance.SwitchMenu<PokemonsMenu>();
            }
            else
            {
                loadingText.text = "Failed to get the pokemons - Ensure the device has access to internet connection";
                retryButton.gameObject.SetActive(true);
                loadingIndicator.SetActive(false);
            }
        }

        private void Retry()
        {
            loadingText.text = "Loading Pokemons";
            retryButton.gameObject.SetActive(false);
            loadingIndicator.SetActive(true);
            PokemonManager.Instance.FetchPokemons();
        }

        private void OnLoadingProgressChanged(int current, int total)
        {
            float progress = (float) current / total;
            loadingText.text = $"Loading Pokemons ({progress * 100:0}%)";
        }
    }
}
