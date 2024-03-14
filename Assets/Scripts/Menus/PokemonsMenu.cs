using System;
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

        private int firstVisibleIndex = 0;
        private int lastVisibleIndex = int.MaxValue;
        private string searchTerm;
        private List<PokemonObject> filteredPokemons;
        private Dictionary<PokemonObject, PokemonSlot> pokemonSlots = new Dictionary<PokemonObject, PokemonSlot>();

        private VisualElement PokemonContainer => uiDocument.rootVisualElement.Q(nameof(PokemonContainer));
        private ScrollView ScrollView => uiDocument.rootVisualElement.Q<ScrollView>();
        private TextField SearchInput => uiDocument.rootVisualElement.Q<TextField>(nameof(SearchInput));
        private Button ClearSearchButton => uiDocument.rootVisualElement.Q<Button>(nameof(ClearSearchButton));


        public override async void Open()
        {
            base.Open();

            ClearSearchButton.clicked += ClearSearch;
            ScrollView.verticalScroller.valueChanged += OnScrollValueChanged;
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
            ScrollView.verticalScroller.valueChanged -= OnScrollValueChanged;
            SearchInput.UnregisterValueChangedCallback(OnSearchBarChanged);

            base.Close();
        }

        public void ScrollTo(PokemonObject pokemonObject)
        {
            if (!pokemonSlots.TryGetValue(pokemonObject, out PokemonSlot pokemonSlot)) return;

            // Need to wait for UI Toolkit Layout to be initialized first
            CoroutinesUtility.DelayByFrame(this, () => ScrollView.ScrollTo(pokemonSlot));
        }

        private void OnScrollValueChanged(float scrollChanged)
        {
            UpdateSlotsVisibility();
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

            CoroutinesUtility.DelayByFrame(this, InitializeSlotsVisibility);

        }

        private void InitializeSlotsVisibility()
        {
            int index = 0;
            int? firstVisibleIndex = null;
            int? lastVisibleIndex = null;
            foreach (VisualElement pokemonSlot in PokemonContainer.Children())
            {
                if (!IsElementVisible(pokemonSlot))
                {
                    pokemonSlot.visible = false;
                    lastVisibleIndex ??= index - 1;
                }
                else
                {
                    firstVisibleIndex ??= index;
                    pokemonSlot.visible = true;
                }
                index++;
            }

            this.firstVisibleIndex = firstVisibleIndex.Value;
            this.lastVisibleIndex = lastVisibleIndex.Value;
        }

        private void UpdateSlotsVisibility()
        {
            bool firstIndexChanged = false;
            for (int i = Mathf.Max(0, firstVisibleIndex - 1); i >= 0; i--)
            {
                VisualElement slot = PokemonContainer.contentContainer[i];
                if (!IsElementVisible(slot)) break;

                slot.visible = true;
                firstVisibleIndex = i;
                firstIndexChanged = true;
            }

            bool lastIndexChanged = false;
            for (int i = Mathf.Min(lastVisibleIndex + 1, PokemonContainer.childCount); i < PokemonContainer.childCount; i++)
            {
                VisualElement slot = PokemonContainer.contentContainer[i];
                if (!IsElementVisible(slot)) break;

                slot.visible = true;
                lastVisibleIndex = i;
                lastIndexChanged = true;
            }

            for (int i = firstVisibleIndex; i <= lastVisibleIndex; i++)
            {
                VisualElement slot = PokemonContainer.contentContainer[i];
                bool isVisible = IsElementVisible(slot);
                slot.visible = isVisible;
                if (firstIndexChanged && !isVisible)
                {
                    ChangeSlotsVisibility(i, lastVisibleIndex, false);
                    lastVisibleIndex = i - 1;
                    break;
                }

                if (lastIndexChanged && isVisible)
                {
                    firstVisibleIndex = i - 1;
                    break;
                }
            }
        }

        private void ChangeSlotsVisibility(int from, int to, bool visible)
        {
            for (int i = from; i <= to; i++)
            {
                PokemonContainer.contentContainer[i].visible = visible;
            }
        }

        private bool IsElementVisible(VisualElement element)
        {
            Rect position = element.worldBound;
            return position.yMax >= 0 - 400 && position.y < Screen.height + 400;
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
