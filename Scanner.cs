/*
    Levithan Compiler - Lexical Analisys

    Camila Rovirosa A01024192
    Eduardo Badillo A01020716
    Isabel Maqueda  A01652906

  This program is free software: you can redistribute it and/or modify
  it under the terms of the GNU General Public License as published by
  the Free Software Foundation, either version 3 of the License, or
  (at your option) any later version.

  This program is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  GNU General Public License for more details.

*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Leviathan {

    class Scanner {
        
        readonly string input;

        // The most specific first, general later
        static readonly Regex regex = new Regex(
            @"
                (?<And>        &&                )
              | (?<Assign>     [=]               )
              | (?<Comment>    [#].*             )
              | (?<CharLiteral> '[^'\n]'         )
              | (?<IntLiteral> -?\d+             )
              | (?<Identifier> [0-9a-zA-Z_]+     )
              | (?<StringLiteral> ""[^""\n]*""  )
              | (?<Less>       [<]       )
              | (?<Mul>        [*]       )
              | (?<Neg>        [-]       )
              | (?<Plus>       [+]       )
              | (?<Newline>    \n        )
              | (?<ParLeft>    [(]       )
              | (?<ParRight>   [)]       )
              | (?<BraceLeft>   [{]      )
              | (?<BraceRight>   [}]     )
              | (?<WhiteSpace> \s        )
              | (?<Other>      .         )     # Must be last: match any other character.
            ",
            RegexOptions.IgnorePatternWhitespace
                | RegexOptions.Compiled
                | RegexOptions.Multiline
            );

            static readonly IDictionary<string, TokenCategory> keywords =
            new Dictionary<string, TokenCategory>() {
                {"break", TokenCategory.BREAK},
                {"if", TokenCategory.IF},
                {"do", TokenCategory.DO},
                {"return", TokenCategory.RETURN},
                {"elif", TokenCategory.ELIF},
                //{"boolLiteral", TokenCategory.BOOL_LITERAL},
                {"else", TokenCategory.ELSE},
                {"var", TokenCategory.VAR},
                //{"false", TokenCategory.LIT_BOOL},
                {"while", TokenCategory.WHILE}
            };

            static readonly IDictionary<string, TokenCategory> nonKeywords =
            new Dictionary<string, TokenCategory>() {
                {"And", TokenCategory.AND},
                {"Assign", TokenCategory.ASSIGN},
                //{"False", TokenCategory.FALSE},
                {"CharLiteral", TokenCategory.CHAR_LITERAL},
                {"IntLiteral", TokenCategory.INT_LITERAL},
                {"StringLiteral", TokenCategory.STRING_LITERAL},
                {"Less", TokenCategory.LESS},
                {"Mul", TokenCategory.MUL},
                {"Neg", TokenCategory.NEG},
                {"ParLeft", TokenCategory.PARENTHESIS_OPEN},
                {"ParRight", TokenCategory.PARENTHESIS_CLOSE},
                {"BraceLeft", TokenCategory.BRACE_OPEN},
                {"BraceRight", TokenCategory.BRACE_CLOSE},
                {"Plus", TokenCategory.PLUS},
                //{"True", TokenCategory.TRUE}
            };

         public Scanner(string input) {
            this.input = input;
        }

        public IEnumerable<Token> Start() {

            var row = 1;
            var columnStart = 0;
            
            Func<Match, TokenCategory, Token> newTok = (m, tc) =>
                new Token(m.Value, tc, row, m.Index - columnStart + 1);

            foreach (Match m in regex.Matches(input)) {
                Console.WriteLine(m.Value);
                if (m.Groups["Newline"].Success) {

                    // Found a new line.
                    row++;
                    columnStart = m.Index + m.Length;

                } else if (m.Groups["WhiteSpace"].Success
                    || m.Groups["Comment"].Success) {

                    // Skip white space and comments.

                } else if (m.Groups["Identifier"].Success) {

                    if (keywords.ContainsKey(m.Value)) {
                        Console.WriteLine(m.Value + " is a keyword");
                        // Matched string is a Buttercup keyword.
                        yield return newTok(m, keywords[m.Value]);

                    } else {

                        // Otherwise it's just a plain identifier.
                        yield return newTok(m, TokenCategory.IDENTIFIER);
                    }

                } else if (m.Groups["Other"].Success) {

                    // Found an illegal character.
                    yield return newTok(m, TokenCategory.ILLEGAL_CHAR);

                } else {
                    Console.WriteLine(m.Value + " is a nonKeyword");
                    // Match must be one of the non keywords.
                    foreach (var name in nonKeywords.Keys) {
                        if (m.Groups[name].Success) {
                            yield return newTok(m, nonKeywords[name]);
                            break;
                        }
                    }
                }
            }

            yield return new Token(null,
                                   TokenCategory.EOF,
                                   row,
                                   input.Length - columnStart + 1);
        }

    }
}