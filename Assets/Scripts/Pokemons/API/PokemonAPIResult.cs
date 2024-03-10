namespace UniDex.Pokemons.API
{
    public struct PokemonAPIResult<T>
    {
        public PokemonAPIResultType resultType;
        public string error;
        public T data;

        public PokemonAPIResult(T data)
        {
            this.data = data;
            error = null;
            resultType = PokemonAPIResultType.Success;
        }

        public PokemonAPIResult(string error)
        {
            this.error = error;
            data = default;
            resultType = PokemonAPIResultType.Error;
        }
    }
}