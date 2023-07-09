# Fin-Eng

[![Build and Test](https://github.com/bthreader/Fin-Eng/actions/workflows/main.yml/badge.svg)](https://github.com/bthreader/Fin-Eng/actions/workflows/main.yml)

A collection of experiments related to the financial markets for the purpose of learning more about buffers, concurrent processing and functional programming (via F#).

### Buffers (F#)

Using locks and blocking collections to process trades and quotes for the purpose of computing the real time value of a portfolio of stocks.

### Triple Buffer (Java)

Using a bounded, wait-free buffer implementation to process FX quotes arriving from two different markets in order to compute a single exchange rate at regular intervals.
