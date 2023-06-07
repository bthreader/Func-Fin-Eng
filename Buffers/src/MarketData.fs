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
    let private random () =
        System.Random(System.Environment.TickCount)

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

    let randomSymbol () = symbols[random().Next(symbols.Length)]

    let randomQuote (symbol: FTSEStockSymbol) =
        let randomBidPrice = 5 - random().Next(0, 3)
        let randomAskPrice = 5 + random().Next(0, 2)

        { symbol = symbol
          bid = randomBidPrice
          ask = randomAskPrice }

    let randomTrade (symbol: FTSEStockSymbol) =
        let randomTradePrice = 7.0 + random().NextDouble()

        { symbol = symbol
          size = 5
          price = randomTradePrice }
