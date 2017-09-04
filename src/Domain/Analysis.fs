namespace SentimentFS.AnalysisServer.Domain.Analysis
open System
open System.Collections.Generic
open SentimentFS.AnalysisServer.Domain.Sentiment

type Trend =
    | Increasing = 1
    | Stable = 0
    | Decreasing = -1

type AnalysisScore = { SentimentByQuantity: IDictionary<Sentiment, int>
                       KeyWords: struct (string * int) seq
                       Localizations: struct (float32 * float32) seq
                       Key: string
                       Trend: Trend
                       DateByQuantity: IDictionary<DateTime, int> }
    with static member Zero key = { SentimentByQuantity = Dictionary<Sentiment, int>()
                                    KeyWords = [||]
                                    Localizations = [||]
                                    Key = key
                                    Trend = Trend.Stable
                                    DateByQuantity = Dictionary<DateTime, int>() }

type Query =
    | GetAnalysisForKey of key:string
