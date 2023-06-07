open Consumer
open Receiver

let rec receiver (quoteBuffer: QuoteBuffer) (tradeBuffer: TradeBuffer) =
    async {
        do addTrades tradeBuffer 5
        do! Async.Sleep(100)

        lock quoteBuffer (fun () -> addQuotes quoteBuffer 200)

        do addTrades tradeBuffer 20

        do! Async.Sleep(200)
        return! receiver quoteBuffer tradeBuffer
    }

let rec consumer (quoteBuffer: QuoteBuffer) (tradeBuffer: TradeBuffer) (portfolio: Portfolio.Portfolio) =
    async {
        if not tradeBuffer.IsEmpty then
            match tradeBuffer.TryDequeue() with
            | true, trade ->
                match Handlers.handleTrade trade portfolio with
                | Some newPortfolio ->
                    do Portfolio.printPortfolioValue newPortfolio
                    return! consumer quoteBuffer tradeBuffer newPortfolio
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

            do Portfolio.printPortfolioValue (newPortfolio)
            return! consumer quoteBuffer tradeBuffer newPortfolio

        do! Async.Sleep(100) // Avoid tight looping
        return! consumer quoteBuffer tradeBuffer portfolio
    }


[<EntryPoint>]
let main argv =
    let quoteBuffer = new QuoteBuffer()
    let tradeBuffer = new TradeBuffer()
    let portfolio = Portfolio.generateRandomPortfolio (10)
    do Portfolio.printPortfolioValue portfolio

    [ receiver quoteBuffer tradeBuffer; consumer quoteBuffer tradeBuffer portfolio ]
    |> Async.Parallel
    |> Async.RunSynchronously
    |> ignore

    0
