namespace UniDex.Pokemons.API
{
    public struct PokemonAPIResult<T>
    {
        public PokemonAPIResultType ResultType { get; private set; }
        public T Data { get; private set; }
        public string Error { get; private set; }

        public bool IsError => ResultType == PokemonAPIResultType.Error;

        public PokemonAPIResult(T data)
        {
            ResultType = PokemonAPIResultType.Success;
            Data = data;
            Error = null;
        }

        public PokemonAPIResult(string error)
        {
            ResultType = PokemonAPIResultType.Error;
            Data = default;
            Error = error;
        }
    }
}