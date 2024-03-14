using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using UniDex.Patterns;
using UniDex.Pokemons.API;
using UniDex.Pokemons.API.Data;
using UnityEngine;

namespace UniDex.Pokemons
{
    public class PokemonManager : MonoSingleton<PokemonManager>
    {
        [SerializeField]
        private uint limit = 10000;
        [SerializeField, Range(1, 300)]
        private int maxSimultaneousAPICalls = 60;
        [SerializeField]
        private Texture defaultPokemonTexture;

        private Dictionary<int, PokemonObject> allPokemons = new Dictionary<int, PokemonObject>();

        /// <summary>
        /// Pokemons dictionary with Pokemon's ID as the key
        /// </summary>
        public ReadOnlyDictionary<int, PokemonObject> AllPokemons => new ReadOnlyDictionary<int, PokemonObject>(allPokemons);
        /// <summary>
        /// Whether the Pokemons fetching is finished, no matter if successful or not.
        /// </summary>
        public bool IsPokemonFetchCompleted { get; private set; }
        /// <summary>
        /// Whether the Pokemons are currently being received through the API
        /// </summary>
        public bool IsPokemonFetchingInProgress { get; private set; }

        /// <summary>
        /// Event called when one pack of simultaneous API calls is finished. Provides amount of currently loaded Pokemons, and total Pokemons to load.
        /// </summary>
        public event Action<int, int> OnPokemonFetchingProgressChanged;
        /// <summary>
        /// Event called when fetching call completes. It provides a boolean determining whether the fetch was successful or not.
        /// </summary>
        public event Action<bool> OnPokemonFetchCompleted;

        public async void FetchPokemons()
        {
            if (IsPokemonFetchingInProgress)
            {
                Debug.LogWarning("Pokemons are already being fetched");
                return;
            }

            IsPokemonFetchingInProgress = true;
            var pokemonListResult = await PokemonAPI.GetPokemonList(limit);

            if (pokemonListResult.IsError)
            {
                FinishFetchingWithError(pokemonListResult.Error);
                return;
            }

            NamedAPIResource[] pokemonList = pokemonListResult.Data.results;
            // Use throttling mechanism to limit API calls running at once to avoid SSL errors;
            int throttlesAmount = Mathf.CeilToInt(pokemonList.Length / (float) maxSimultaneousAPICalls);
            for (int throttle = 0; throttle < throttlesAmount; throttle++)
            {
                int apiCallsAmount = throttle == throttlesAmount - 1 ? pokemonList.Length - throttle * maxSimultaneousAPICalls : maxSimultaneousAPICalls;
                Task<PokemonObject>[] pokemonObjectsTasks = new Task<PokemonObject>[apiCallsAmount];
                try
                {
                    for (int i = 0; i < apiCallsAmount; i++)
                    {
                        int pokemonIndex = throttle * maxSimultaneousAPICalls + i;
                        pokemonObjectsTasks[i] = PokemonFactory.CreatePokemonFromAPI(pokemonList[pokemonIndex].name);
                    }

                    PokemonObject[] pokemons = await Task.WhenAll(pokemonObjectsTasks);
                    if (destroyCancellationToken.IsCancellationRequested) return;

                    foreach (PokemonObject pokemon in pokemons)
                    {
                        if (!pokemon.Texture)
                        {
                            pokemon.SetTexture(defaultPokemonTexture);
                        }

                        allPokemons.Add(pokemon.ID, pokemon);
                    }

                    OnPokemonFetchingProgressChanged?.Invoke(allPokemons.Count, pokemonList.Length);
                }
                catch (Exception exception)
                {
                    FinishFetchingWithError(exception.Message);
                    return;
                }
            }

            IsPokemonFetchCompleted = true;
            IsPokemonFetchingInProgress = false;

            OnPokemonFetchCompleted?.Invoke(true);
        }

        private void FinishFetchingWithError(string error)
        {
            Debug.LogError(error);

            IsPokemonFetchCompleted = true;
            IsPokemonFetchingInProgress = false;

            OnPokemonFetchCompleted?.Invoke(false);
        }
    }
}
