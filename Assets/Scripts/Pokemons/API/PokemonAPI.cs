using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UniDex.Pokemons.API.Data;
using UnityEngine;
using UnityEngine.Networking;

namespace UniDex.Pokemons.API
{
    public static class PokemonAPI
    {
        private const string BASE_URL = "https://pokeapi.co/api/v2/";

        private static Dictionary<string, PokemonObject> pokemonCacheByName = new Dictionary<string, PokemonObject>();

        
        /// <returns>Result containing useful information about all Pokemons by combining multiple API endpoint results.</returns>
        public static async Task<PokemonAPIResult<PokemonObject[]>> GetAllPokemons(uint limit = 10000, uint offset = 0)
        {
            var speciesCollectionResult = await GetAPIData<PokemonCollection>($"pokemon?limit={limit}&offset={offset}");

            if (speciesCollectionResult.resultType == PokemonAPIResultType.Error)
            {
                return new PokemonAPIResult<PokemonObject[]>(speciesCollectionResult.error);
            }

            var speciesCollection = speciesCollectionResult.data;
            var pokemonSpeciesTasks = new Task<PokemonAPIResult<PokemonObject>>[speciesCollection.results.Length];
            for (int i = 0; i < pokemonSpeciesTasks.Length; i++)
            {
                string pokemonName = speciesCollection.results[i].name;
                pokemonSpeciesTasks[i] = GetPokemon(pokemonName);
            }

            await Task.WhenAll(pokemonSpeciesTasks);

            foreach (var task in pokemonSpeciesTasks)
            {
                if (task.Result.resultType == PokemonAPIResultType.Error)
                {
                    return new PokemonAPIResult<PokemonObject[]>(task.Result.error);
                }
            }

            PokemonObject[] pokemonSpeciesArray = pokemonSpeciesTasks.Select(task => task.Result.data).ToArray();
            return new PokemonAPIResult<PokemonObject[]>(pokemonSpeciesArray);
        }

        /// <returns>Result containing useful information about a Pokemon by combining multiple API endpoint results.</returns>
        public static async Task<PokemonAPIResult<PokemonObject>> GetPokemon(string pokemonName)
        {
            if (!pokemonCacheByName.TryGetValue(pokemonName, out PokemonObject pokemonData))
            {
                var pokemonResult = await GetPokemonAPI(pokemonName);
                if (pokemonResult.resultType == PokemonAPIResultType.Error)
                {
                    return new PokemonAPIResult<PokemonObject>($"{pokemonResult.error} Resource: pokemon-{pokemonName}");
                }

                var pokemonSpeciesResult = await GetPokemonSpeciesAPI(pokemonName);
                if (pokemonSpeciesResult.resultType == PokemonAPIResultType.Error)
                {
                    return new PokemonAPIResult<PokemonObject>($"{pokemonSpeciesResult.error} Resource: pokemon-species-{pokemonName}");
                }

                pokemonData = await PokemonObject.FromAPIData(pokemonResult.data, pokemonSpeciesResult.data);
                pokemonCacheByName.Add(pokemonName, pokemonData);
            }

            return new PokemonAPIResult<PokemonObject>(pokemonData);
        }

        /// <returns>Result containing Pokemon representation exactly as defined by JSON.</returns>
        public static async Task<PokemonAPIResult<Pokemon>> GetPokemonAPI(string pokemonName)
        {
            return await GetAPIData<Pokemon>($"pokemon/{pokemonName}");
        }

        /// <returns>Result containing Pokemon Species representation exactly as defined by JSON.</returns>
        public static async Task<PokemonAPIResult<PokemonSpecies>> GetPokemonSpeciesAPI(string pokemonName)
        {
            return await GetAPIData<PokemonSpecies>($"pokemon-species/{pokemonName}");
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

                string json = webRequest.downloadHandler.text;
                T data = JsonConvert.DeserializeObject<T>(json);
                return new PokemonAPIResult<T>(data);
            }
        }

        private static UnityWebRequest CreateEndpointRequest(string apiEndpoint)
        {
            return UnityWebRequest.Get($"{BASE_URL}{apiEndpoint}");
        }
    }
}