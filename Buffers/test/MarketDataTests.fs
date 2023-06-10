module MarketDataTests

open NUnit.Framework
open FsUnit

open MarketData

[<Test>]
let ``Quote .mid method should correctly average bid and ask`` () =
    { symbol = AAL; bid = 5; ask = 10 }.mid |> should equal 7.5
    { symbol = BP; bid = 10; ask = 20 }.mid |> should equal 15

[<Test>]
let ``RandomQuoteGenerator should generate valid quotes`` () =
    let quoteGenerator: IQuoteGenerator = Generate.RandomQuoteGenerator()

    [ 1..5 ]
    |> Seq.iter (fun _ ->

        let quote =
            { symbol = quoteGenerator.symbol () ()
              bid = quoteGenerator.bidPrice () ()
              ask = quoteGenerator.askPrice () () }

        quote.symbol |> should be ofExactType<FTSEStockSymbol>

        quote.bid |> should be (greaterThanOrEqualTo 2)
        quote.bid |> should be (lessThanOrEqualTo 5)

        quote.ask |> should be (greaterThanOrEqualTo 5)
        quote.ask |> should be (lessThanOrEqualTo 7))

[<Test>]
let ``RandomTradeGenerator should generate valid trades`` () =
    let tradeGenerator: ITradeGenerator = Generate.RandomTradeGenerator()

    [ 1..5 ]
    |> Seq.iter (fun _ ->

        let trade =
            { symbol = tradeGenerator.symbol () ()
              size = tradeGenerator.size () ()
              price = tradeGenerator.price () () }

        trade.symbol |> should be ofExactType<FTSEStockSymbol>

        trade.size |> should equal 5

        trade.price |> should be (greaterThanOrEqualTo 7)
        trade.price |> should be (lessThanOrEqualTo 8))
