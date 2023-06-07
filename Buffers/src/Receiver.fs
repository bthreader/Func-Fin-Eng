module Receiver

open System.Collections.Concurrent
open System.Collections.Generic

open MarketData
open MarketData.MarketDataGenerator

type QuoteBuffer = Dictionary<FTSEStockSymbol, Quote>
type TradeBuffer = ConcurrentQueue<Trade>

let addQuote (quoteBuffer: QuoteBuffer) =
    let symbol = randomSymbol ()
    let generatedQuote = randomQuote (symbol)

    match quoteBuffer.ContainsKey(symbol) with
    | true -> do quoteBuffer.Remove(symbol) |> ignore
    | _ -> ()

    do quoteBuffer.Add(symbol, generatedQuote)

let rec addTrade (tradeBuffer: TradeBuffer) =
    let symbol = randomSymbol ()
    let generatedTrade = randomTrade (symbol)
    do tradeBuffer.Enqueue(generatedTrade)
