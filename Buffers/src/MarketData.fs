namespace MarketData

type FTSEStockSymbol =
    | VOD
    | RDSB
    | HSBA
    | BHP
    | GSK
    | AZN
    | BP
    | RIO
    | ULVR
    | AAL
    | LLOY
    | BARC
    | GLEN
    | REL
    | BT

type Quote =
    { symbol: FTSEStockSymbol
      bid: float
      ask: float }

    member this.mid = (this.bid + this.ask) / 2.0

type Trade =
    { symbol: FTSEStockSymbol
      price: float
      size: int }

type IQuoteGenerator =
    abstract member symbol: unit -> (unit -> FTSEStockSymbol)
    abstract member bidPrice: unit -> (unit -> int)
    abstract member askPrice: unit -> (unit -> int)

type ITradeGenerator =
    abstract member symbol: unit -> (unit -> FTSEStockSymbol)
    abstract member size: unit -> (unit -> int)
    abstract member price: unit -> (unit -> float)

module Deterministic =
    let generateQuote (generator: IQuoteGenerator) =
        { symbol = generator.symbol () ()
          bid = generator.bidPrice () ()
          ask = generator.askPrice () () }

    let generateTrade (generator: ITradeGenerator) =
        { symbol = generator.symbol () ()
          size = generator.size () ()
          price = generator.price () () }

module NonDeterministic =
    let private symbols =
        [ VOD
          RDSB
          HSBA
          BHP
          GSK
          AZN
          BP
          RIO
          ULVR
          AAL
          LLOY
          BARC
          GLEN
          REL
          BT ]

    let private random = System.Random()
    let randomSymbol () = symbols[random.Next(symbols.Length)]

    type RandomQuoteGenerator() =
        interface IQuoteGenerator with
            member _.symbol() = randomSymbol
            member _.bidPrice() = fun () -> 5 - random.Next(0, 3)
            member _.askPrice() = fun () -> 5 + random.Next(0, 2)

    type RandomTradeGenerator() =
        interface ITradeGenerator with
            member _.symbol() = randomSymbol
            member _.size() = fun () -> 5
            member _.price() = fun () -> 7.0 + random.NextDouble()
