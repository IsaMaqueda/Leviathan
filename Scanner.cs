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

  You should have received a copy of the GNU General Public License
  along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Leviathan {

    class Scanner {
        
        readonly string input;

        static readonly Regex regex = new Regex(
            @"
                (?<And>        [&][&]       )
              | (?<Assign>     [=]       )
              | (?<Comment>    [#].*     ) #este solo agarra comentario de una linea
              | (?<id>         [0-9a-zA-Z_]+     )
              | (?<lit-int> \d+       )
              | (?<lit-bool>   0|1|true|false)
              | (?<Less>       [<]       )
              | (?<Mul>        [*]       )
              | (?<Neg>        [-]       )
              | (?<Newline>    \n        )
              | (?<Carriage_Return>    \r)
              | (?<Tab>        \t)
              | (?<Backslash>  [\][\]    )
              | (?<Single_Quote>  [\'])
              | (?<Double_Quote>  [\"])
              | (?<Unicode_Char>  [∖uhhhhhh])
              | (?<lit-char> '.')
              | (?<lit-str>  "[^'"]+" )
              | (?<ParLeft>    [(]       )
              | (?<ParRight>   [)]       )
              | (?<Plus>       [+]       )
              | (?<WhiteSpace> \s        )     # Must go anywhere after Newline.
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
                {"true", TokenCategory.TRUE},
                {"else", TokenCategory.ELSE},
                {"var", TokenCategory.VAR},
                {"false", TokenCategory.FALSE},
                {"while", TokenCategory.WHILE}
            };


        public IEnumerable<Token> Start() {

            var row = 1;
            var columnStart = 0;
            
            Func<Match, TokenCategory, Token> newTok = (m, tc) =>
                new Token(m.Value, tc, row, m.Index - columnStart + 1);

            foreach (Match m in regex.Matches(input)) {

                if (m.Groups["Newline"].Success) {

                    // Found a new line.
                    row++;
                    columnStart = m.Index + m.Length;

                } else if (m.Groups["WhiteSpace"].Success
                    || m.Groups["Comment"].Success) {

                    // Skip white space and comments.

                } else if (m.Groups["Identifier"].Success) {

                    if (keywords.ContainsKey(m.Value)) {

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