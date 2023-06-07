module Receiver

open System.Collections.Concurrent
open System.Collections.Generic

open MarketData
open MarketData.MarketDataGenerator

type QuoteBuffer = Dictionary<FTSEStockSymbol, Quote>
type TradeBuffer = ConcurrentQueue<Trade>

let rec addQuotes (quoteBuffer: QuoteBuffer) (quotesToAdd: int) =
    let rec x (quoteBuffer: QuoteBuffer) (quotesToAdd: int) (count: int) =
        if count >= quotesToAdd then
            ()
        else
            let symbol = randomSymbol ()
            let generatedQuote = randomQuote (symbol)

            match quoteBuffer.ContainsKey(symbol) with
            | true -> do quoteBuffer.Remove(symbol) |> ignore
            | _ -> ()

            do quoteBuffer.Add(symbol, generatedQuote)

            x quoteBuffer quotesToAdd (count + 1)

    x quoteBuffer quotesToAdd 0

let rec addTrades (tradeBuffer: TradeBuffer) (tradesToAdd: int) =
    let rec x (tradeBuffer: TradeBuffer) (tradesToAdd: int) (count: int) =
        if count >= tradesToAdd then
            ()
        else
            let symbol = randomSymbol ()
            let generatedTrade = randomTrade (symbol)
            do tradeBuffer.Enqueue(generatedTrade)
            x tradeBuffer tradesToAdd (count + 1)

    x tradeBuffer tradesToAdd 0
