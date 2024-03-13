namespace UniDex.Pokemons.API.Data
{
    [System.Serializable]
    public struct NamedAPIResourceList
    {
        /// <summary>
        /// The total number of resources available from this API.
        /// </summary>
        public int count;
        /// <summary>
        /// The URL for the next page in the list.
        /// </summary>
        public string next;
        /// <summary>
        /// The URL for the previous page in the list.
        /// </summary>
        public string previous;
        /// <summary>
        /// A list of named API resources.
        /// </summary>
        public NamedAPIResource[] results;
    }
}
