module ConsumerTests

open NUnit.Framework
open FsUnit

open MarketData

module Utils =
    open Consumer.Portfolio

    let testPortfolio: Portfolio =
        Map.empty
            .Add(
                VOD,
                { shares = 10
                  price = 1
                  lastExecutedPrice = None }
            )
            .Add(
                BP,
                { shares = 20
                  price = 5
                  lastExecutedPrice = Some 5 }
            )

module Portfolio =
    open Utils

    open Consumer.Portfolio

    [<Test>]
    let PortfolioEntryTest () =
        { shares = 10
          price = 1
          lastExecutedPrice = None }
            .value
        |> should equal 10

        { shares = 25
          price = 2
          lastExecutedPrice = None }
            .value
        |> should equal 50

    [<Test>]
    let portfolioValueTest () =
        portfolioValue testPortfolio |> should equal 110
        portfolioValue Map.empty |> should equal 0

    [<TestFixture>]
    type GenerateRandomPortfolioTests() =
        [<Test>]
        member _.``Should generate portfolio with specified number of stocks``() =
            let nStocks = 5
            let portfolio = generateRandomPortfolio nStocks
            let actualStockCount = portfolio.Count
            actualStockCount |> should equal nStocks

        [<Test>]
        member _.``Should generate portfolio with valid entries``() =
            let nStocks = 3
            let portfolio = generateRandomPortfolio nStocks

            portfolio.Values
            |> Seq.iter (fun entry ->
                entry.shares |> should equal 100
                entry.price |> should equal 0
                entry.lastExecutedPrice |> should equal None)

module Handlers =
    open Utils

    open Consumer.Handlers

    [<TestFixture>]
    type HandleTradeTests() =
        [<Test>]
        member _.``Given a trade for an existing symbol, should return updated portfolio``() =
            testPortfolio[VOD].price |> should equal 1

            let trade =
                { symbol = VOD
                  price = 20.0
                  size = 10 }

            let updatedPortfolio = handleTrade trade testPortfolio |> Option.get

            let updatedEntry = updatedPortfolio[VOD]
            updatedEntry.price |> should equal 20
            updatedEntry.lastExecutedPrice |> Option.get |> should equal 20


        [<Test>]
        member _.``Given a trade for a non-existing symbol, should return None``() =
            let trade =
                { symbol = RIO
                  price = 18.0
                  size = 10 }

            let updatedPortfolio = handleTrade trade testPortfolio
            updatedPortfolio |> Option.isNone |> should equal true

    [<TestFixture>]
    type HandleQuoteTests() =
        [<Test>]
        member _.``Given a quote for an existing symbol, that doesn't have a lastExecutedPrice, should return updated portfolio``
            ()
            =
            testPortfolio[VOD].price |> should equal 1
            testPortfolio[VOD].lastExecutedPrice |> Option.isNone |> should equal true

            let quote = { symbol = VOD; bid = 5; ask = 10 }

            let updatedPortfolio = handleQuote quote testPortfolio |> Option.get
            let updatedEntry = updatedPortfolio[VOD]

            updatedEntry.price |> should equal quote.mid
            updatedEntry.lastExecutedPrice |> Option.isNone |> should equal true

        [<Test>]
        member _.``Given a quote for an existing symbol, that does have a lastExecutedPrice, should return updated portfolio``
            ()
            =
            testPortfolio[BP].price |> should equal 5
            testPortfolio[BP].lastExecutedPrice |> Option.get |> should equal 5

            let quote = { symbol = BP; bid = 5; ask = 10 }

            let updatedPortfolio = handleQuote quote testPortfolio |> Option.get

            // lastExecutedPrice shouldn't have changed value
            testPortfolio[BP].lastExecutedPrice
            |> Option.get
            |> should equal (Option.get updatedPortfolio[BP].lastExecutedPrice)

            let newPrice =
                Option.get testPortfolio[BP].lastExecutedPrice * 0.8 + quote.mid * 0.2

            updatedPortfolio[BP].price |> should equal newPrice

        [<Test>]
        member _.``Given a quote for a non-existing symbol, should return None``() =
            let quote = { symbol = RIO; bid = 5; ask = 10 }

            let updatedPortfolio = handleQuote quote testPortfolio

            updatedPortfolio |> Option.isNone |> should equal true
