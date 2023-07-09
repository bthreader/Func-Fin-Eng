# Triple Buffer

## Project Description

Similar to the "Buffers" project. This project aims to build a market data stream processor that handles real-time market data. However, instead of using a concurrent queue, it uses triple buffers, which avoids blocking behaviour in a single producer, single consumer scenario and has bounded memory usage.

## The Triple Buffer

```mermaid
graph LR
    subgraph t[Triple Buffer]

        subgraph b[backbuffers]
            B1[dirty backbuffer]
            B1 <-->|publish swap| B2
            B2[clean backbuffer]
        end

        B2 <-->|update swap| B3[outputBuffer]
    end

    M[Producer - Market A] -->|write price| B1

    B3 -->|read| P[Consumer - Pricer]
```

In the single producer and single consumer case, this approach has some useful characteristics:

1. The producer and consumer operate independently at their own rate and there is no blocking.
   - The producer can always write new data to the buffer.
   - The consumer can always read the latest (available) version of the data.
2. Bounded buffer size.
   - In the "Buffers" project, the (unbounded) queue could theoretically become very large due to processing rate discrepancies between the producer and consumer. Such behaviour could cause queue size to exceed the capacity of the processor's cache, causing performance issues.

Some other characteristics of a triple buffer (or reasons not to use one):

1. Data loss.
   - In the "Buffers" project, every executed trade was read and processed by the consumer.
   - In the triple buffer case, if the producer writes 2 values before the consumer reads, then the first (over-written) value will not be processed.
   - Not a concern for this project as we're only interested in getting the latest available price from the market.
2. Memory usage.
   - Three buffers use up more memory than one.
   - In this case the increase in performance due to the wait-free nature of a triple buffer justifies the memory overhead.

## Project Setup

```mermaid
graph TB
    A[Market A Producer] --GBP/USD rate--> BA[Triple Buffer]
    B[Market B Producer] --GBP/USD rate--> BB[Triple Buffer]
    BA --> P[Pricer]
    BB --> P
```

The project involves two buffers, one for each market. There is a separate producer thread for each market which writes at regular intervals to the respective buffer.

A single consumer thread then reads from both buffers to compute a combined rate in the form of a simple average:

```math
\text{GBPUSD}_{pricer} = \frac{\text{GBPUSD}_{market A} + \text{GBPUSD}_{market B}}{2}
```
