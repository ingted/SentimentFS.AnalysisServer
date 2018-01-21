namespace SentimentFS.AnalysisServer

open Common.Messages.Twitter
open Akka.Streams.Dsl
open Common.Dto.TwitterApi
open Akkling.Streams
open Akka
open Tweetinvi.Parameters
open System
open Tweetinvi.Models
open Tweetinvi
open SentimentFS.AnalysisServer.Common.Messages.Sentiment

module TwitterApi =

    let downloadTweetsFlow (maxConcurrentDownloads: int)(credentials: TwitterCredentials) =
        Flow.id
        |> Flow.asyncMapUnordered(maxConcurrentDownloads)(fun q ->
                                         async {
                                            let options = SearchTweetsParameters(q.key)
                                            options.SearchType <- Nullable<SearchResultType>(SearchResultType.Recent)
                                            options.Lang <- Nullable<LanguageFilter>(LanguageFilter.English)
                                            options.Filters <- TweetSearchFilters.None
                                            options.MaximumNumberOfResults <- q.quantity
                                            options.Since <- q.since
                                            return! SearchAsync.SearchTweets(options) |> Async.AwaitTask
                                         })
        |> Flow.collect(id)
        |> Flow.filter(fun tweet -> not tweet.IsRetweet)
        |> Flow.map(fun tweet ->
                        { IdStr = tweet.IdStr;
                          Text = tweet.Text;
                          Language = tweet.Language.ToString();
                          CreationDate = tweet.CreatedAt;
                          Longitude = match tweet.Coordinates with null -> 0.0 | coord -> coord.Longitude;
                          Latitude = match tweet.Coordinates with null -> 0.0 | coord -> coord.Latitude;
                          HashTags = (tweet.Hashtags |> Seq.map(fun x -> x.Text))
                          Sentiment = Emotion.Neutral })

module Actor =
    open Akkling
    open Akkling.Persistence
    open Akkling.Streams


    let tweetsActor (mailbox: Actor<TweetsMessage>) =
        let rec loop (state) =
            actor {
                let! msg = mailbox.Receive()
                return loop({tweets = []})
            }
        loop({tweets = []})



