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

module MarketDataGenerator =
    let private random = System.Random(System.Environment.TickCount)

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

    let randomSymbol () = symbols[random.Next(symbols.Length)]

    let private randomBidPrice () = 5 - random.Next(0, 3)

    let private randomAskPrice () = 5 + random.Next(0, 2)

    let private randomTradePrice () = 7.0 + random.NextDouble()

    let randomQuote (symbol: FTSEStockSymbol) =
        { symbol = symbol
          bid = randomBidPrice ()
          ask = randomAskPrice () }

    let randomTrade (symbol: FTSEStockSymbol) =
        { symbol = symbol
          size = 5
          price = randomTradePrice () }
