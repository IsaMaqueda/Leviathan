/*
  Leviathan compiler - Specific node subclasses for the AST (Abstract
  Syntax Tree).
  
  Camila Rovirosa A010241927
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

    // Leviathan nodes
    class Program: Node {}
    class Identifier: Node {}
    class DefList: Node {}
    class Def: Node {}
    class VarDef : Node {}
    class VarList : Node {}
    class IdList : Node {}
    class FunDef : Node {}
    class ParamList : Node {}
    class VarDefList : Node {}
    class StmtList: Node {}
    class Stmt: Node {}
    class StmtIdentifier: Node {}
    class StmtAssign: Node {}
    class StmtIncr: Node {}
    class StmtDecr: Node {}
    class StmtFunCall: Node {}
    class FunCall: Node {}
    class ExprList: Node {}
    class ExprListCont: Node {}
    class StmtIf: Node {}
    class Elif : Node {}
    class ElseIfList: Node {}
    class Else: Node {}
    class StmtWhile: Node {}
    class StmtDoWhile: Node {}
    class StmtBreak: Node {}
    class StmtReturn: Node {}
    class StmtEmpty: Node {}
    class Expr: Node {}
    class ExprOr: Node {}
    class ExprAnd: Node {}
    class ExprComp: Node {}
    class OpComp: Node {}
    class ExprRel: Node {}
    class OpRel: Node {}
    class ExprAdd: Node {}
    class OpAdd: Node {}
    class ExprMul: Node {}
    class OpMul: Node {}
    class ExprUnary: Node {}
    class OpUnary: Node {}
    class ExprPrimary: Node {}
    class ExprPrimaryIdentifier: Node {}
    class Array: Node {}
    class Lit: Node {}

   // Operators 
    class Equal: Node {}
    class UnEqual: Node {}
    class Less: Node {}
    class Less_Equal: Node {}
    class More: Node {}
    class More_Equal : Node {}
    class Plus: Node {}
    class Neg: Node {}
    class Mul: Node {}
    class Div: Node {}
    class Mod: Node {}
    class Not: Node {}

    //Lit
    class True: Node {}
    class False: Node {}
    class Int_Literal : Node {}
    class Char_Literal : Node {}
    class String_Literal : Node {}
    







}