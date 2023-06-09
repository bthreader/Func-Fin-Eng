namespace Consumer

open MarketData

type PortfolioEntry =
    { shares: int
      price: float
      lastExecutedPrice: Option<float> }

    member this.value = float (this.shares) * this.price

type Portfolio = Map<FTSEStockSymbol, PortfolioEntry>

type PortfolioUpdateReason =
    | Quote
    | Trade
    | Init

    override this.ToString() =
        match this with
        | Quote -> "Q"
        | Trade -> "T"
        | Init -> "I"

module Portfolio =
    open System

    let value (portfolio: Portfolio) =
        portfolio.Values |> Seq.sumBy (fun entry -> entry.value)

    let printValue (portfolio: Portfolio) (updateReason: PortfolioUpdateReason) =
        let value = value portfolio

        Console.SetCursorPosition(0, Console.CursorTop)
        printf "Portfolio Value: %.2f (%s)" value (updateReason.ToString())

    let generateRandom nStocks =
        let rec newRandomSymbol (existingPortfolio: Portfolio) =
            Generate.randomSymbol ()
            |> fun symbol ->
                match existingPortfolio.ContainsKey(symbol) with
                | false -> symbol
                | true -> newRandomSymbol (existingPortfolio)

        let rec x (nStocks: int) (existingPortfolio: Portfolio) =
            if existingPortfolio.Count = nStocks then
                existingPortfolio
            else
                let randomSymbol = newRandomSymbol existingPortfolio

                let newPortfolio =
                    existingPortfolio.Add(
                        randomSymbol,
                        { shares = 100
                          price = 0
                          lastExecutedPrice = None }
                    )

                x nStocks newPortfolio

        x nStocks Map.empty

module Handlers =

    /// <summary> Adjusts portfolio for trade data. </summary>
    /// <returns>
    ///     * None if the symbol being traded doesn't exist in the portfolio.
    ///     * The updated portfolio if it does.
    /// </returns>
    let handleTrade (trade: Trade) (portfolio: Portfolio) : option<Portfolio> =
        match portfolio.TryGetValue(trade.symbol) with
        | true, value ->
            let newEntry =
                { price = trade.price
                  lastExecutedPrice = Some(trade.price)
                  shares = value.shares }

            Some(portfolio.Add(trade.symbol, newEntry))
        | false, _ -> None

    /// <summary> Adjusts portfolio for quote data. </summary>
    /// <returns>
    ///     * None if the symbol being quoted doesn't exist in the portfolio.
    ///     * The updated portfolio if it does.
    /// </returns>
    let handleQuote (quote: Quote) (portfolio: Portfolio) : option<Portfolio> =
        let handlePriceUpdate (quoteMid: float) (lastExecutedPrice: option<float>) =
            match lastExecutedPrice with
            | Some price -> price * 0.8 + quoteMid * 0.2
            | None -> quoteMid

        match portfolio.TryGetValue(quote.symbol) with
        | true, value ->
            let newEntry =
                { price = handlePriceUpdate quote.mid value.lastExecutedPrice
                  lastExecutedPrice = value.lastExecutedPrice
                  shares = value.shares }

            Some(portfolio.Add(quote.symbol, newEntry))
        | false, _ -> None
