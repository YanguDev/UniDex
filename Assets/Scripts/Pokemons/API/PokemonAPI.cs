using Newtonsoft.Json;
using System.Threading.Tasks;
using UniDex.Pokemons.API.Data;
using UnityEngine;
using UnityEngine.Networking;

namespace UniDex.Pokemons.API
{
    public static class PokemonAPI
    {
        private const string BASE_URL = "https://pokeapi.co/api/v2/";
        
        /// <returns>Resource list of all available pokemons.</returns>
        public static async Task<PokemonAPIResult<NamedAPIResourceList>> GetPokemonList(uint limit = 10000, uint offset = 0)
        {
            return await GetAPIData<NamedAPIResourceList>($"pokemon?limit={limit}&offset={offset}");
        }

        /// <returns>Resource list of all available Pokemon Species.</returns>
        public static async Task<PokemonAPIResult<NamedAPIResourceList>> GetPokemonSpeciesList(uint limit = 10000, uint offset = 0)
        {
            return await GetAPIData<NamedAPIResourceList>($"pokemon-species?limit={limit}&offset={offset}");
        }

        /// <returns>Information about a Pokemon with provided name or ID.</returns>
        public static async Task<PokemonAPIResult<Pokemon>> GetPokemon(string pokemonNameOrID)
        {
            return await GetAPIData<Pokemon>($"pokemon/{pokemonNameOrID}");
        }

        /// <returns>Information about a Pokemon with provided ID.</returns>
        public static async Task<PokemonAPIResult<Pokemon>> GetPokemon(uint pokemonID)
        {
            return await GetPokemon(pokemonID.ToString());
        }

        /// <returns>Information about a Pokemon species with provided name or ID.</returns>
        public static async Task<PokemonAPIResult<PokemonSpecies>> GetPokemonSpecies(string pokemonNameOrID)
        {
            return await GetAPIData<PokemonSpecies>($"pokemon-species/{pokemonNameOrID}");
        }

        /// <returns>Information about a Pokemon species with provided ID.</returns>
        public static async Task<PokemonAPIResult<PokemonSpecies>> GetPokemonSpecies(uint pokemonID)
        {
            return await GetPokemonSpecies(pokemonID.ToString());
        }

        /// <returns>Information about a Pokemon species with provided Pokemon.</returns>
        public static async Task<PokemonAPIResult<PokemonSpecies>> GetPokemonSpecies(Pokemon pokemon)
        {
            return await GetPokemonSpecies(pokemon.species.name);
        }

        private static async Task<PokemonAPIResult<T>> GetAPIData<T>(string endpoint)
        {
            using (UnityWebRequest webRequest = CreateEndpointRequest(endpoint))
            {
                UnityWebRequestAsyncOperation asyncOperation = webRequest.SendWebRequest();
                while (!asyncOperation.isDone)
                {
                    await Task.Yield();
                }

                if (!webRequest.IsSuccessful())
                {
                    return new PokemonAPIResult<T>(webRequest.error);
                }
                
                return new PokemonAPIResult<T>(JsonUtility.FromJson<T>(webRequest.downloadHandler.text));
            }
        }

        private static UnityWebRequest CreateEndpointRequest(string apiEndpoint)
        {
            return UnityWebRequest.Get($"{BASE_URL}{apiEndpoint}");
        }
    }
}