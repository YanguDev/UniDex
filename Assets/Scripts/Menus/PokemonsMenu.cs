using System.Collections;
using System.Collections.Generic;
using UniDex.Pokemons;
using UniDex.Pokemons.API;
using UniDex.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace UniDex.Menus
{
    public class PokemonsMenu : Menu
    {
        [Header("API")]
        [SerializeField]
        private uint pokemonLimit;
        [Header("UI")]
        [SerializeField]
        private UIDocument uiDocument;
        [SerializeField]
        private string pokemonContainerElementID = "PokemonContainer";

        private VisualElement PokemonContainerElement => uiDocument.rootVisualElement.Q(pokemonContainerElementID);

        private async void OnEnable()
        {
            var result = await PokemonAPI.GetAllPokemons(pokemonLimit);

            if (result.resultType == PokemonAPIResultType.Error)
            {
                throw new System.Exception(result.error);
            }
            
            CreatePokemonSlots(result.data);
        }

        private void CreatePokemonSlots(PokemonObject[] pokemonObjects)
        {
            foreach (var pokemonObject in pokemonObjects)
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
