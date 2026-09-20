# Hacker News Best Stories API

An ASP.NET Core REST API that returns the highest-scoring stories from the Hacker News `beststories` feed.

## Run locally

Prerequisites: .NET 9 SDK

...

## Design

The API performs the following steps:

1. Loads the IDs from `beststories.json`.
2. Loads every referenced item, with at most 10 item requests.
3. Discards null, deleted, dead and non-story items, plus incomplete stories.
4. Sorts by score descending, then by time descending and finally by ID descending to avoid duplicates.
5. Returns the first `n` items.

I actively decided to fetch every item. &nbsp;The order of `beststories.json` is Hacker News's ranking, which is not guaranteed to be by score order. &nbsp;If I fetching only the first `n` IDs, I can't guarantee the highest `n` scores.

To handle high request volume:

- ...
- 
## Assumptions

- ...

## Tests

...

## Further enhancements

Given more time, I would add:

- ...

## HackerNews API

- [Official Hacker News API documentation](https://github.com/HackerNews/API)
- `GET https://hacker-news.firebaseio.com/v0/beststories.json`
- `GET https://hacker-news.firebaseio.com/v0/item/{id}.json`

## License

[MIT](LICENSE)
