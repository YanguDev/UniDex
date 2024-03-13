using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UniDex.Pokemons;
using UniDex.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace UniDex.Menus
{
    public class PokemonsMenu : Menu
    {
        [SerializeField]
        private UIDocument uiDocument;

        private List<PokemonObject> filteredPokemons;
        private Dictionary<PokemonObject, PokemonSlot> pokemonSlots = new Dictionary<PokemonObject, PokemonSlot>();

        private VisualElement PokemonContainer => uiDocument.rootVisualElement.Q(nameof(PokemonContainer));
        private ScrollView ScrollView => uiDocument.rootVisualElement.Q<ScrollView>();
        private TextField SearchInput => uiDocument.rootVisualElement.Q<TextField>(nameof(SearchInput));
        private Button ClearSearchButton => uiDocument.rootVisualElement.Q<Button>(nameof(ClearSearchButton));

        private string searchTerm;

        public override async void Open()
        {
            base.Open();

            ClearSearchButton.clicked += ClearSearch;
            SearchInput.RegisterValueChangedCallback(OnSearchBarChanged);
            
            SearchInput.SetValueWithoutNotify(searchTerm);
            RefreshClearButton();

            while (!PokemonManager.Instance.IsPokemonFetchCompleted)
            {
                await Task.Yield();
            }

            filteredPokemons ??= PokemonManager.Instance.AllPokemons.Values.ToList();

            CreatePokemonSlots();
        }

        public override void Close()
        {
            ClearSearchButton.clicked -= ClearSearch;
            SearchInput.UnregisterValueChangedCallback(OnSearchBarChanged);

            base.Close();
        }

        public void ScrollTo(PokemonObject pokemonObject)
        {
            if (!pokemonSlots.TryGetValue(pokemonObject, out PokemonSlot pokemonSlot)) return;

            // Need to wait for UI Toolkit Layout to be initialized first
            CoroutinesUtility.DelayByFrame(this, () => ScrollView.ScrollTo(pokemonSlot));
        }

        private void CreatePokemonSlots()
        {
            PokemonContainer.Clear();
            foreach (PokemonObject pokemonObject in filteredPokemons)
            {
                var pokemonSlot = new PokemonSlot(pokemonObject, OpenPokemonDetails);
                PokemonContainer.Add(pokemonSlot);

                if (!pokemonSlots.ContainsKey(pokemonObject))
                {
                    pokemonSlots.Add(pokemonObject, pokemonSlot);
                }
                else
                {
                    pokemonSlots[pokemonObject] = pokemonSlot;
                }
            }
        }

        private void OpenPokemonDetails(PokemonObject pokemonObject)
        {
            DetailsMenu detailsMenu = MenuManager.Instance.SwitchMenu<DetailsMenu>();
            detailsMenu.SetPokemonDetails(pokemonObject);
            detailsMenu.SetPokemonsContext(filteredPokemons, filteredPokemons.IndexOf(pokemonObject));
        }

        private void RefreshClearButton()
        {
            ClearSearchButton.visible = !string.IsNullOrEmpty(searchTerm);
        }

        private void OnSearchBarChanged(ChangeEvent<string> changeEvent)
        {
            searchTerm = changeEvent.newValue.ToLower();
            RefreshClearButton();

            filteredPokemons = PokemonManager.Instance.AllPokemons.Values.Where(pokemon => pokemon.Name.ToLower().Contains(searchTerm))
                .ToList();

            CreatePokemonSlots();
        }

        private void ClearSearch()
        {
            SearchInput.value = string.Empty;
        }
    }
}
