using UniDex.Pokemons.API;
using UniDex.Pokemons.API.Data;
using UniDex.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace UniDex.Pokemons
{
    public class PokemonTester : MonoBehaviour
    {
        [SerializeField]
        private uint limit;
        [SerializeField]
        private UIDocument uiDocument;

        private async void OnEnable()
        {
            // var result = await PokemonAPI.GetPokemon(pokemonId);
            var result = await PokemonAPI.GetAllPokemons(limit);

            if (result.resultType == PokemonAPIResultType.Error)
            {
                throw new System.Exception(result.error);
            }

            foreach (var pokemonData in result.data)
            {
                var pokemonSlot = new PokemonSlot(pokemonData);
                uiDocument.rootVisualElement.Q("PokemonContainer").Add(pokemonSlot);
            }
        }
    }
}