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

        private VisualElement PokemonContainerElement => uiDocument.rootVisualElement.Q(pokemonContainerElementID);

        public override async void Open()
        {
            base.Open();
            
            while (!PokemonManager.Instance.IsPokemonFetchCompleted)
            {
                await Task.Yield();
            }

            CreatePokemonSlots();
        }

        private void CreatePokemonSlots()
        {
            foreach (PokemonObject pokemonObject in PokemonManager.Instance.AllPokemons.Values)
            {
                var pokemonSlot = new PokemonSlot(pokemonObject, OpenPokemonDetails);
                PokemonContainerElement.Add(pokemonSlot);
            }

        }

        private void OpenPokemonDetails(PokemonObject pokemonObject)
        {
            MenuManager.Instance.SwitchMenu<DetailsMenu>()
                .SetPokemonDetails(pokemonObject);
        }
    }
}
