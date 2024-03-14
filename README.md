![image](https://github.com/YanguDev/UniDex/assets/42221923/8ee757cd-87ee-4687-acb6-40c480c34b0e)

# UniDex

PokeDex created in Unity using [PokeAPI](https://pokeapi.co/) as data source being fetched in runtime.

**Unity Version:** 2022.3.14f1

## Features
- List through all available Pokemons with search functionality
![image](https://github.com/YanguDev/UniDex/assets/42221923/e38026d3-30ce-44a5-b925-a0ef8d0732d8)
- View basic details about selected Pokemon with quick navigation available
![image](https://github.com/YanguDev/UniDex/assets/42221923/6350e95d-c597-4b96-9c32-94bf7ab8e5b1)

# Problems Analysis
## API Fetching
**UnityWebRequests** were chosen for this project to perform the API fetching, and did better job than when I tried using C# HttpClients in both speed and ease of use. Despite being designed for Coroutines, it was also easy to make it work in async functions flawlessly.

When fetching Pokemons through PokeAPI, the returned data includes another API endpoints that need to be fetched, such as PokemonSpecies to get information such as Pokemon's description or color. The same goes for Pokemon sprites - unlike in HTML, we can't use URL as an image source directly in the Unity, and it needs to be downloaded through another UnityWebRequest.

To improve the fetching speed, multiple API endpoints can be fetched at once, but there another problem arose - when there are too many requests coming into the PokeAPI, it starts to fail the requests due to SSL. To mitigate this problem, throttling mechanism was implemented to limit maximum API calls at once and separate the fetching into multiple batches.

## Fetched Data Usage
The data fetched from the API is returned as a string in JSON format. In order to turn it into a class, some JSON converter needs to be used. Using SimpleJSON and manually getting desired data was one way to go, but seemed like a lot of work to datascrape all the needed information about the Pokemons. Instead I went with NewtonsoftJSON to convert the strings into structs prepared to reflect the needed data from the API. This however was an overkill, because while NewtonsoftJSON is powerful, it allocates a lot of memory for the Garbage Collector which resulted in severe frame drops while the data was being fetched. Switching to Unity's built-in JsonUtility was enough to get the job done, and frame drops are now gone.

## UI
I tried out UI Toolkit for this project. Unity team seems to be saying this is the future, so I guess it must be really good, right? Oh boy was I wrong. While I have to say it is promising and it's easy to create some blockouts of the UI, it's missing many basic features and a lot of useful CSS properties that make the designs beautiful. There is no GridView for the UI Toolkit for example. While it's not a problem to use any container with wrapping Flexbox to get this functionality, there is no possibility for native gap settings to space out the elements inside the container. There are also no media queries to change the layout based on the screen size, which I was hopeful to use to implement portrait UI for mobile devices. It's of course possible with using custom scripts, however this seems like a basic feature for a UI library that is being heavily inspired by HTML and CSS. Many features and issues are also not backported. I don't see the UI Toolkit as production ready for runtime UI (because it seems okay for Editor tools), but hopefully Unity will put way more work into this library in the future.

## Performance
Because there are ~1300 pokemons fetched from the API, the same amount of slots in the ScrollView have to be populated. Even if only 60 out of 1300 pokemons are visible on the screen, the rest is still rendered and takes the device's resources. To mitigate this problem I was able to write an Occlusion script for the ScrollView, which makes the elements not visible on the screen to not be rendered by UI Toolkit, simply by changing their "visible" property to "false".
