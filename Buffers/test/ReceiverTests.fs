module ReceiverTests

open NUnit.Framework
open FsUnit

open Receiver
open MarketData

type MockQuoteGenerator() =
    interface IQuoteGenerator with
        member _.symbol() = fun () -> AAL
        member _.askPrice() = fun () -> 10
        member _.bidPrice() = fun () -> 5

[<TestFixture>]
type AddQuoteTests() =
    [<Test>]
    member _.``Should add a new quote to an empty quote buffer``() =
        let quoteBuffer = new QuoteBuffer()

        addQuote quoteBuffer (MockQuoteGenerator())

        quoteBuffer.Count |> should equal 1
        quoteBuffer[AAL].bid |> should equal 5
        quoteBuffer[AAL].ask |> should equal 10

    [<Test>]
    member _.``Should replace an existing quote in the quote buffer``() =
        // Arrange
        let quoteBuffer = new QuoteBuffer()

        let existingQuote = { symbol = AAL; bid = 100; ask = 100 }
        quoteBuffer.Add(AAL, existingQuote)
        quoteBuffer.Count |> should equal 1
        quoteBuffer[AAL].bid |> should equal 100
        quoteBuffer[AAL].ask |> should equal 100

        // Act
        addQuote quoteBuffer (MockQuoteGenerator())

        // Assert
        quoteBuffer.Count |> should equal 1
        quoteBuffer[AAL].bid |> should equal 5
        quoteBuffer[AAL].ask |> should equal 10

type MockTradeGenerator() =
    interface ITradeGenerator with
        member _.symbol() = fun () -> AAL
        member _.size() = fun () -> 1
        member _.price() = fun () -> 1.0

[<Test>]
let addTradeTest () =
    // Arrange
    let tradeBuffer = new TradeBuffer()

    // Act
    addTrade tradeBuffer (MockTradeGenerator())

    // Assert
    tradeBuffer.Count |> should equal 1

    match tradeBuffer.TryPeek() with
    | true, trade ->
        trade.price |> should equal 1
        trade.size |> should equal 1
        trade.symbol |> should equal AAL
    | false, _ -> failwith ("broken TradeBuffer")
