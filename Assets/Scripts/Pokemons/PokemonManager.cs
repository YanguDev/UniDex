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
        /// Whether the Pokemons were successfully received through the API
        /// </summary>
        public bool IsPokemonFetchCompleted { get; private set; }

        /// <summary>
        /// Event called when one pack of simultaneous API calls is finished. Provides amount of currently loaded Pokemons, and total Pokemons to load.
        /// </summary>
        public event Action<int, int> OnPokemonFetchingProgressChanged;
        /// <summary>
        /// Event called when all Pokemons are fetched successfully.
        /// </summary>
        public event Action OnPokemonFetchCompleted;

        protected override async void Awake()
        {
            base.Awake();
            
            var pokemonListResult = await PokemonAPI.GetPokemonList(limit);

            if (pokemonListResult.IsError)
            {
                throw new Exception(pokemonListResult.Error);
            }

            NamedAPIResource[] pokemonList = pokemonListResult.Data.results;
            // Use throttling mechanism to limit API calls running at once to avoid SSL errors;
            int throttlesAmount = Mathf.CeilToInt(pokemonList.Length / (float) maxSimultaneousAPICalls);
            for (int throttle = 0; throttle < throttlesAmount; throttle++)
            {
                int apiCallsAmount = throttle == throttlesAmount - 1 ? pokemonList.Length % maxSimultaneousAPICalls : maxSimultaneousAPICalls;
                Task<PokemonObject>[] pokemonObjectsTasks = new Task<PokemonObject>[apiCallsAmount];
                for (int i = 0; i < apiCallsAmount; i++)
                {
                    int pokemonIndex = throttle * maxSimultaneousAPICalls + i;
                    pokemonObjectsTasks[i] = PokemonFactory.CreatePokemonFromAPI(pokemonList[pokemonIndex].name);
                }

                PokemonObject[] pokemons = await Task.WhenAll(pokemonObjectsTasks);
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

            IsPokemonFetchCompleted = true;
            OnPokemonFetchCompleted?.Invoke();
        }
    }
}
