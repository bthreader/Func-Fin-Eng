namespace Consumer

open MarketData

module Portfolio =
    open System

    type PortfolioEntry =
        { shares: int
          price: float
          lastExecutedPrice: Option<float> }

        member this.value = float (this.shares) * this.price

    type Portfolio = Map<FTSEStockSymbol, PortfolioEntry>

    let portfolioValue (p: Portfolio) =
        p.Values |> Seq.sumBy (fun entry -> entry.value)

    type PortfolioUpdateReason =
        | Quote
        | Trade
        | Init

    let printPortfolioValue (portfolio: Portfolio) (updateReason: PortfolioUpdateReason) =
        let value = portfolioValue (portfolio)

        let reasonString =
            match updateReason with
            | Quote -> "Q"
            | Trade -> "T"
            | Init -> "I"

        Console.SetCursorPosition(0, Console.CursorTop)
        printf "Portfolio Value: %.2f (%s)" value reasonString

    let generateRandomPortfolio nStocks =
        let rec newRandomSymbolForPortfolio (existingPortfolio: Portfolio) =
            Generate.randomSymbol ()
            |> fun symbol ->
                match existingPortfolio.ContainsKey(symbol) with
                | false -> symbol
                | true -> newRandomSymbolForPortfolio (existingPortfolio)

        let rec x (nStocks: int) (existingPortfolio: Portfolio) =
            if existingPortfolio.Count = nStocks then
                existingPortfolio
            else
                let randomSymbol = newRandomSymbolForPortfolio existingPortfolio

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
    open Portfolio

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
