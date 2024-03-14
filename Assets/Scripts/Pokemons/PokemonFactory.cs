using System.Threading.Tasks;
using UniDex.Colors;
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
                throw new System.Exception($"Couldn't fetch Pokemon - {pokemonNameOrID}. Error: {pokemonResult.Error}");
            }

            Pokemon pokemon = pokemonResult.Data;
            var pokemonSpeciesTask = PokemonAPI.GetPokemonSpecies(pokemon);
            string spriteURL = pokemon.sprites.front_default;
            Task<Texture> textureTask = null;
            if (!string.IsNullOrEmpty(spriteURL))
            {
                textureTask = TextureDownloader.DownloadFromURL(spriteURL);
            }

            if (textureTask != null)
            {
                await Task.WhenAll(pokemonSpeciesTask, textureTask);
            }
            else
            {
                await pokemonSpeciesTask;
            }

            var pokemonSpeciesResult = pokemonSpeciesTask.Result;
            if (pokemonSpeciesResult.IsError)
            {
                throw new System.Exception($"Couldn't fetch Pokemon Species - {pokemonNameOrID}. Error: {pokemonSpeciesResult.Error}");
            }

            PokemonSpecies pokemonSpecies = pokemonSpeciesResult.Data;
            Texture pokemonTexture = textureTask?.Result;
            if (pokemonTexture)
            {
                pokemonTexture.filterMode = FilterMode.Point;
            }

            NamedColor namedColor = GetNamedColorFromAPIColor(pokemonSpecies);

            return new PokemonObject(pokemon, pokemonSpecies, pokemonTexture, namedColor);
        }

        public static async Task<PokemonObject> CreatePokemonFromAPI(uint pokemonID)
        {
            return await CreatePokemonFromAPI(pokemonID.ToString());
        }

        private static NamedColor GetNamedColorFromAPIColor(PokemonSpecies pokemonSpecies)
        {
            return pokemonSpecies.color.name switch
            {
                "black" => NamedColor.Black,
                "blue" => NamedColor.Blue,
                "brown" => NamedColor.Brown,
                "gray" => NamedColor.Gray,
                "green" => NamedColor.Green,
                "pink" => NamedColor.Pink,
                "purple" => NamedColor.Purple,
                "red" => NamedColor.Red,
                "white" => NamedColor.White,
                "yellow" => NamedColor.Yellow,
                _ => NamedColor.White
            };
        }
    }
}
