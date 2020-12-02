# Leviathan compiler, version 0.5

This program is free software. You may redistribute it under the terms of the GNU General Public License version 3 or later. See `license.txt` for details.

Included in this release:

* Lexical analysis
* Syntactic analysis
* AST construction
* Semantic analysis
* WAT & WASM code generation

## Requirements

You need Node.js 12 or newer. Go to the [Node.js website](https://nodejs.org/en/download/). If you need to install it.

To check that you have the correct version of Node.js, at the terminal type:

    node -v

The output should show that you are using version 12 or higher:

    v14.15.0

## How to Build

To install the required Node.js modules and build the C# code, at the terminal type:

    make

## How to Run
To compile a leviathan source file, type:

    ./leviathanc.js <leviathan_source_file>

Where `<leviathan_source_file>` is the name of a Leviathan source file. You can
try with these files:

* `001_hello.leviathan`
* `002_binary.leviathan`
* `003_palindrome.leviathan`
* `004_factorial.leviathan`
* `005_arrays.leviathan`
* `006_next_day.leviathan`
* `007_literals.leviathan`
* `008_vars.leviathan`
* `009_operators.leviathan`
* `010_breaks.leviathan`

To execute the resulting WASM file, type:

    ./execute.js <wasm_file>

Where `<wasm_file>` is a WebAssembly binary file, for example:

* `001_hello.wasm`
* `002_binary.wasm`
* `003_palindrome.wasm`
* `004_factorial.wasm`
* `005_arrays.wasm`
* `006_next_day.wasm`
* `007_literals.wasm`
* `008_vars.wasm`
* `009_operators.wasm`
* `010_breaks.wasm`

## How to Clean

To delete all the files that get created automatically, at the terminal type:

    make clean