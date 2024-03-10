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

        public static async Task<PokemonAPIResult<PokemonSpecies[]>> GetAllPokemonSpecies(uint limit = 10000, uint offset = 0)
        {
            var speciesCollectionResult = await GetAPIData<PokemonSpeciesCollection>($"pokemon-species?limit={limit}&offset={offset}");

            if (speciesCollectionResult.resultType == PokemonAPIResultType.Error)
            {
                return new PokemonAPIResult<PokemonSpecies[]>(speciesCollectionResult.error);
            }

            var pokemonSpecies = new List<PokemonSpecies>();
            IEnumerable<string> pokemonNames = speciesCollectionResult.data.results.Select(result => result.name);
            foreach (string pokemonName in pokemonNames)
            {
                var speciesResult = await GetPokemonSpecies(pokemonName);

                if (speciesResult.resultType == PokemonAPIResultType.Error)
                {
                    return new PokemonAPIResult<PokemonSpecies[]>(speciesResult.error);
                }

                pokemonSpecies.Add(speciesResult.data);
            }

            return new PokemonAPIResult<PokemonSpecies[]>(pokemonSpecies.ToArray());
        }

        public static async Task<PokemonAPIResult<PokemonSpecies>> GetPokemonSpecies(uint pokemonID)
        {
            return await GetAPIData<PokemonSpecies>($"pokemon-species/{pokemonID}");
        }

        public static async Task<PokemonAPIResult<PokemonSpecies>> GetPokemonSpecies(string pokemonName)
        {
            return await GetAPIData<PokemonSpecies>($"pokemon-species/{pokemonName}");
        }

        private static async Task<PokemonAPIResult<T>> GetAPIData<T>(string endpoint)
        {
            using (UnityWebRequest webRequest = CreateEndpointRequest(endpoint))
            {
                var asyncOperation = webRequest.SendWebRequest();
                while (!asyncOperation.isDone)
                {
                    await Task.Yield();
                }

                if (!IsRequestSuccessful(webRequest))
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

        private static bool IsRequestSuccessful(UnityWebRequest webRequest)
        {
            return webRequest.result != UnityWebRequest.Result.ConnectionError
                && webRequest.result != UnityWebRequest.Result.ProtocolError
                && webRequest.result != UnityWebRequest.Result.DataProcessingError;
        }
    }
}