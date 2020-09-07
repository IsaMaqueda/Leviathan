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
namespace Leviathan {
    enum TokenCategory {
        // plain identifier
        IDENTIFIER,
        // non keywords
        AND,
        ASSIGN, 
        INT_LITERAL,
        STRING_LITERAL,
        CHAR_LITERAL,
        LESS,
        MUL,
        NEG,
        PARENTHESIS_OPEN,
        PARENTHESIS_CLOSE,
        BRACE_OPEN,
        BRACE_CLOSE,
        PLUS,
        SEMI_COLON,
        // keywords 
        BREAK,
        DO,
        ELIF,
        ELSE,
        FALSE,
        IF,
        RETURN,
        TRUE,
        VAR,
        WHILE, 
        // OTHER
        ILLEGAL_CHAR, 
        EOF
    }
}