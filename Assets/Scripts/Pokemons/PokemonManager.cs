using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private uint pokemonLimit;

        private Dictionary<int, PokemonObject> allPokemons = new Dictionary<int, PokemonObject>();

        /// <summary>
        /// Pokemons dictionary with Pokemon's ID as the key
        /// </summary>
        public ReadOnlyDictionary<int, PokemonObject> AllPokemons => new ReadOnlyDictionary<int, PokemonObject>(allPokemons);
        /// <summary>
        /// Whether the Pokemons were successfully received through the API
        /// </summary>
        public bool IsPokemonFetchCompleted { get; private set; }

        protected override async void Awake()
        {
            base.Awake();
            
            var pokemonListResult = await PokemonAPI.GetPokemonList(pokemonLimit);

            if (pokemonListResult.IsError)
            {
                throw new System.Exception(pokemonListResult.Error);
            }

            NamedAPIResource[] pokemonList = pokemonListResult.Data.results;
            Task<PokemonObject>[] pokemonObjectsTasks = new Task<PokemonObject>[pokemonList.Length];
            for (int i = 0; i < pokemonList.Length; i++)
            {
                pokemonObjectsTasks[i] = PokemonFactory.CreatePokemonFromAPI(pokemonList[i].name);
            }

            PokemonObject[] pokemons = await Task.WhenAll(pokemonObjectsTasks);
            foreach (PokemonObject pokemon in pokemons)
            {
                allPokemons.Add(pokemon.ID, pokemon);
            }

            IsPokemonFetchCompleted = true;
        }
    }
}
