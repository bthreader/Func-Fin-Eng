module Receiver

open System.Collections.Concurrent
open System.Collections.Generic

open MarketData
open MarketData.Deterministic

type QuoteBuffer = Dictionary<FTSEStockSymbol, Quote>
type TradeBuffer = ConcurrentQueue<Trade>

let addQuote (quoteBuffer: QuoteBuffer) (generator: IQuoteGenerator) =
    let generatedQuote = generateQuote generator
    let symbol = generatedQuote.symbol

    match quoteBuffer.ContainsKey(symbol) with
    | true -> do quoteBuffer.Remove(symbol) |> ignore
    | _ -> ()

    do quoteBuffer.Add(symbol, generatedQuote)

let rec addTrade (tradeBuffer: TradeBuffer) (generator: ITradeGenerator) =
    let generatedTrade = generateTrade generator
    do tradeBuffer.Enqueue(generatedTrade)
