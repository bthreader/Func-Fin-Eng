open MarketData
open Consumer
open Receiver

let rec receiver (quoteBuffer: QuoteBuffer) (tradeBuffer: TradeBuffer) =
    async {
        let tradeReceiverTask =
            async {
                let rec loop () =
                    async {
                        do addTrade tradeBuffer (Generate.RandomTradeGenerator())
                        do! Async.Sleep(1)
                        do! loop ()
                    }

                do! loop ()
            }

        let quoteReceiverTask =
            async {
                let rec loop () =
                    async {
                        lock quoteBuffer (fun () -> do addQuote quoteBuffer (Generate.RandomQuoteGenerator()))
                        do! Async.Sleep(5)
                        do! loop ()
                    }

                do! loop ()
            }

        [ tradeReceiverTask; quoteReceiverTask ]
        |> Async.Parallel
        |> Async.RunSynchronously
        |> ignore
    }

let rec consumer (quoteBuffer: QuoteBuffer) (tradeBuffer: TradeBuffer) (portfolio: Portfolio) =
    async {
        if not tradeBuffer.IsEmpty then
            match tradeBuffer.TryDequeue() with
            | true, trade ->
                match Handlers.handleTrade trade portfolio with
                | Some newPortfolio ->
                    do Portfolio.printValue newPortfolio Trade
                    do! consumer quoteBuffer tradeBuffer newPortfolio
                | None -> ()
            | false, _ -> ()

        else if quoteBuffer.Count > 0 then
            let newPortfolio =
                lock quoteBuffer (fun () ->
                    let quotes = quoteBuffer |> Seq.map (fun kvp -> kvp.Value) |> Seq.toArray
                    do quoteBuffer.Clear()
                    quotes)
                |> Array.fold
                    (fun acc quote ->
                        match Handlers.handleQuote quote portfolio with
                        | Some newPortfolio -> newPortfolio
                        | None -> acc)
                    portfolio

            do Portfolio.printValue newPortfolio Quote
            do! consumer quoteBuffer tradeBuffer newPortfolio

        do! Async.Sleep(1) // Avoid tight looping
        do! consumer quoteBuffer tradeBuffer portfolio
    }

[<EntryPoint>]
let main argv =
    let quoteBuffer = new QuoteBuffer()
    let tradeBuffer = new TradeBuffer()
    let portfolio = Portfolio.generateRandom 10
    do Portfolio.printValue portfolio Init

    [ receiver quoteBuffer tradeBuffer; consumer quoteBuffer tradeBuffer portfolio ]
    |> Async.Parallel
    |> Async.RunSynchronously
    |> ignore

    0
