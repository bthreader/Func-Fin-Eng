module Receiver

open System.Collections.Concurrent
open System.Collections.Generic

open MarketData

type QuoteBuffer = Dictionary<FTSEStockSymbol, Quote>
type TradeBuffer = ConcurrentQueue<Trade>

let addQuote (quoteBuffer: QuoteBuffer) (generator: IQuoteGenerator) =
    let generatedQuote =
        { symbol = generator.symbol () ()
          bid = generator.bidPrice () ()
          ask = generator.askPrice () () }

    let symbol = generatedQuote.symbol

    match quoteBuffer.ContainsKey(symbol) with
    | true -> do quoteBuffer.Remove(symbol) |> ignore
    | _ -> ()

    do quoteBuffer.Add(symbol, generatedQuote)

let rec addTrade (tradeBuffer: TradeBuffer) (generator: ITradeGenerator) =
    let generatedTrade =
        { symbol = generator.symbol () ()
          size = generator.size () ()
          price = generator.price () () }

    do tradeBuffer.Enqueue(generatedTrade)
