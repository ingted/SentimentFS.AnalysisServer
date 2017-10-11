namespace SentimentFS.AnalysisServer.WebApi

module Analysis =
    open Akka.Actor
    open Suave
    open Suave.Filters
    open Suave.Operators
    open Suave.Successful
    open SentimentFS.AnalysisServer.Core.Analysis
    open SentimentFS.AnalysisServer.Core.Sentiment
    open SentimentFS.AnalysisServer.Core.Actor
    open Cassandra
    open Tweetinvi
    open System.Net.Http
    open Newtonsoft.Json
    open SentimentFS.AnalysisServer.WebApi.Config

    let analysisController(system: ActorSystem) =
        let getAnalysisResultByKey(key):WebPart =
            fun (x : HttpContext) ->
                async {
                    let analysisActor = system.ActorSelection(Actors.analysisActor.Path)
                    let! result = analysisActor.Ask<AnalysisScore option>({ text = key }) |> Async.AwaitTask
                    return! (SuaveJson.toJson result) x
                }

        pathStarts "/api/analysis" >=> choose [
            GET >=> choose [ pathScan "/api/analysis/result/%s" getAnalysisResultByKey ]
        ]
