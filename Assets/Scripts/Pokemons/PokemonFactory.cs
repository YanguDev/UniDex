using System.Threading.Tasks;
using UniDex.Pokemons.API;
using UniDex.Pokemons.API.Data;
using UnityEngine;

namespace UniDex.Pokemons
{
    public static class PokemonFactory
    {
        public static async Task<PokemonObject> CreatePokemonFromAPI(string pokemonNameOrID)
        {
            var pokemonResult = await PokemonAPI.GetPokemon(pokemonNameOrID);
            if (pokemonResult.IsError)
            {
                Debug.LogError($"Couldn't fetch Pokemon - {pokemonNameOrID}. Error: {pokemonResult.Error}");
                return null;
            }

            Pokemon pokemon = pokemonResult.Data;
            var pokemonSpeciesResult = await PokemonAPI.GetPokemonSpecies(pokemon);
            if (pokemonResult.IsError)
            {
                Debug.LogError($"Couldn't fetch Pokemon Species - {pokemonNameOrID}. Error: {pokemonResult.Error}");
                return null;
            }

            PokemonSpecies pokemonSpecies = pokemonSpeciesResult.Data;
            string spriteURL = pokemon.sprites.front_default;
            Texture pokemonTexture = null;
            if (!string.IsNullOrEmpty(spriteURL))
            {
                pokemonTexture = await TextureDownloader.DownloadFromURL(pokemon.sprites.front_default);
                if (pokemonTexture)
                {
                    pokemonTexture.filterMode = FilterMode.Point;
                }
            }

            return new PokemonObject(pokemon, pokemonSpecies, pokemonTexture);
        }

        public static async Task<PokemonObject> CreatePokemonFromAPI(uint pokemonID)
        {
            return await CreatePokemonFromAPI(pokemonID.ToString());
        } 
    }
}
