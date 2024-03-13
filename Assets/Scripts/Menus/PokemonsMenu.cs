using System.Collections.Generic;
using System.Threading.Tasks;
using UniDex.Pokemons;
using UniDex.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace UniDex.Menus
{
    public class PokemonsMenu : Menu
    {
        [Header("UI")]
        [SerializeField]
        private UIDocument uiDocument;
        [SerializeField]
        private string pokemonContainerElementID = "PokemonContainer";

        private Dictionary<PokemonObject, PokemonSlot> pokemonSlots = new Dictionary<PokemonObject, PokemonSlot>();

        private VisualElement PokemonContainerElement => uiDocument.rootVisualElement.Q(pokemonContainerElementID);
        private ScrollView ScrollView => uiDocument.rootVisualElement.Q<ScrollView>();

        public override async void Open()
        {
            base.Open();

            while (!PokemonManager.Instance.IsPokemonFetchCompleted)
            {
                await Task.Yield();
            }

            CreatePokemonSlots();
        }

        public void ScrollTo(PokemonObject pokemonObject)
        {
            if (!pokemonSlots.TryGetValue(pokemonObject, out PokemonSlot pokemonSlot)) return;

            // Need to wait for UI Toolkit Layout to be initialized first
            CoroutinesUtility.DelayByFrame(this, () => ScrollView.ScrollTo(pokemonSlot));
        }

        private void CreatePokemonSlots()
        {
            foreach (PokemonObject pokemonObject in PokemonManager.Instance.AllPokemons.Values)
            {
                var pokemonSlot = new PokemonSlot(pokemonObject, OpenPokemonDetails);
                PokemonContainerElement.Add(pokemonSlot);

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
            MenuManager.Instance.SwitchMenu<DetailsMenu>()
                .SetPokemonDetails(pokemonObject);
        }
    }
}
