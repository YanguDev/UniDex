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
        [Header("UI")]
        [SerializeField]
        private UIDocument uiDocument;
        [SerializeField]
        private string pokemonContainerElementID = "PokemonContainer";

        private List<PokemonObject> filteredPokemons;
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

            if (filteredPokemons == null)
            {
                filteredPokemons = PokemonManager.Instance.AllPokemons.Values.ToList();
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
            foreach (PokemonObject pokemonObject in filteredPokemons)
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
            DetailsMenu detailsMenu = MenuManager.Instance.SwitchMenu<DetailsMenu>();
            detailsMenu.SetPokemonDetails(pokemonObject);
            detailsMenu.SetPokemonsContext(filteredPokemons, filteredPokemons.IndexOf(pokemonObject));
        }
    }
}
