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

        private static Dictionary<string, PokemonData> pokemonCacheByName = new Dictionary<string, PokemonData>();

        public static async Task<PokemonAPIResult<PokemonData[]>> GetAllPokemons(uint limit = 10000, uint offset = 0)
        {
            var speciesCollectionResult = await GetAPIData<PokemonSpeciesCollection>($"pokemon?limit={limit}&offset={offset}");

            if (speciesCollectionResult.resultType == PokemonAPIResultType.Error)
            {
                return new PokemonAPIResult<PokemonData[]>(speciesCollectionResult.error);
            }

            var speciesCollection = speciesCollectionResult.data;
            var pokemonSpeciesTasks = new Task<PokemonAPIResult<PokemonData>>[speciesCollection.results.Length];
            for (int i = 0; i < pokemonSpeciesTasks.Length; i++)
            {
                string pokemonName = speciesCollection.results[i].name;
                pokemonSpeciesTasks[i] = GetPokemonData(pokemonName);
            }

            await Task.WhenAll(pokemonSpeciesTasks);

            foreach (var task in pokemonSpeciesTasks)
            {
                if (task.Result.resultType == PokemonAPIResultType.Error)
                {
                    return new PokemonAPIResult<PokemonData[]>(task.Result.error);
                }
            }

            PokemonData[] pokemonSpeciesArray = pokemonSpeciesTasks.Select(task => task.Result.data).ToArray();
            return new PokemonAPIResult<PokemonData[]>(pokemonSpeciesArray);
        }

        public static async Task<PokemonAPIResult<PokemonData>> GetPokemonData(string pokemonName)
        {
            if (!pokemonCacheByName.TryGetValue(pokemonName, out PokemonData pokemonData))
            {
                var result = await GetAPIData<PokemonSpecies>($"pokemon/{pokemonName}");
                if (result.resultType == PokemonAPIResultType.Error)
                {
                    return new PokemonAPIResult<PokemonData>(result.error);
                }

                pokemonData = await PokemonData.FromPokemonSpecies(result.data);
                pokemonCacheByName.Add(pokemonName, pokemonData);
            }

            return new PokemonAPIResult<PokemonData>(pokemonData);
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