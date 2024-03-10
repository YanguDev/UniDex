using UniDex.Pokemons.API;
using UniDex.Pokemons.API.Data;
using UnityEngine;

namespace UniDex.Pokemons
{
    public class PokemonTester : MonoBehaviour
    {
        [SerializeField]
        private uint pokemonId;

        private async void Start()
        {
            // var result = await PokemonAPI.GetPokemon(pokemonId);
            var result = await PokemonAPI.GetAllPokemonSpecies(limit: 20);

            if (result.resultType == PokemonAPIResultType.Error)
            {
                throw new System.Exception(result.error);
            }

            foreach (var species in result.data)
            {
                Debug.Log(species.name);
            }
        }
    }
}