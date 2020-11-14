/*
 Levithan Compiler - Semantic Analysis - 
  Semantic analyzer

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

//si cambiamos el IDdictionary, podriamos cambiar las categorias a los types que queremos?
// exp Node. nfjsd , Type.Primitive?

using System;
using System.Collections.Generic;

namespace Leviathan {

    class SemanticVisitor {

        //-----------------------------------------------------------
        static readonly IDictionary<TokenCategory, Type> typeMapper =
            new Dictionary<TokenCategory, Type>() {
                { TokenCategory.BOOL, Type.BOOL },
                { TokenCategory.INT, Type.INT }
            };

        // # TABLAS PARA LA PRIMERA PASADA #########################
        //-----------------------------------------------------------
        public SymbolTable gSymbolTable { // global variable table
            get;
            private set;
        }
        
        // set para variables globales
        HashSet<string> globalVariables = new HashSet<string>();

        //-----------------------------------------------------------
        public FunctionTable functionTable { // global function table
            get;
            private set;
        }
        // ######################################################
        //-----------------------------------------------------------
        public SemanticVisitor() {
            // Instantiate & initialize global tables
            functionTable = new FunctionTable();

            // Add predefined functions to global function table
            functionTable["printi"] = new FunctionRow(true, 1, null);
            functionTable["printc"] = new FunctionRow(true, 1, null);
            functionTable["prints"] = new FunctionRow(true, 1, null);
            functionTable["println"] = new FunctionRow(true, 0, null);
            functionTable["readi"] = new FunctionRow(true, 0, null);
            functionTable["reads"] = new FunctionRow(true, 0, null);
            functionTable["new"] = new FunctionRow(true, 1, null);
            functionTable["size"] = new FunctionRow(true, 1, null);
            functionTable["add"] = new FunctionRow(true, 2, null);
            functionTable["get"] = new FunctionRow(true, 2, null);
            functionTable["set"] = new FunctionRow(true, 3, null);


        }

        //-----------------------------------------------------------
        public void Visit(Program node) {
            Visit((dynamic) node[0]); // def-list
            //Visit((dynamic) node[1]);
        }


        public void Visit(DefList node) {
            VisitChildren(node); // def-list ::=  <def>* //def ::= <var-def>|<fun-def>
        }

        public void Visit(FunDef node) { //<fun-def> :: = <id> “(“ <id-list>? “)” “{“ <var-def-list><stmt-list> ”}”
            
            var funName = node.AnchorToken.Lexeme;
            if(functionTable.Contains(funName)){
                throw new SemanticError(
                    "Duplicated function: " + funName,
                    node.AnchorToken);
            } else {
                var numParams = Visit(node[0]);
                functionTable[funName] = new FunctionRow(false, numParams, null);
            }
            //Visit((dynamic) node[0]); // param-list (id-list)? DUDA
            //Visit((dynamic) node[1]); // var-def-list
            //Visit((dynamic) node[2]); // stmt-list
            VisitChildren(node);
        }

        public void Visit(VarDef node){
            Visit(node[0]);
        }

        public int Visit(ParamList node){
            var sonCtr = 0;
            foreach (var n in node){
                sonCtr++;
            }
            return sonCtr;
        }
        /*
        public void Visit(VarDefList node){
            VisitChildren(node);
        }

        public void Visit(StmtList node){
            VisitChildren(node);
        }

        public void Visit(VarDef node) { //<var-def> :: = “var” <id-list> “;”
            Visit((dynamic) node[0]); // IdList
        }
        */
        public void Visit(IdList node) { //<id-list> ::= <id> (“,” <id>)*
            VisitChildren(node); // Identifiers*
        }

        public void Visit(Identifier node){
            var varName = node.AnchorToken.Lexeme;

            if (globalVariables.Contains(varName)) {
                throw new SemanticError(
                    "Duplicated variable: " + varName,
                    node.AnchorToken);
            } else {
                globalVariables.Add(varName);
            }
        }
        
        void VisitChildren(Node node) {
            foreach (var n in node) {
                Visit((dynamic) n);
            }
        }
        /*
        //-----------------------------------------------------------
        public Type Visit(DeclarationList node) {
            VisitChildren(node);
            return Type.VOID;
        }

        //-----------------------------------------------------------
        public Type Visit(Declaration node) {

            var variableName = node[0].AnchorToken.Lexeme;

            if (Table.Contains(variableName)) {
                throw new SemanticError(
                    "Duplicated variable: " + variableName,
                    node[0].AnchorToken);

            } else {
                Table[variableName] =
                    typeMapper[node.AnchorToken.Category];
            }

            return Type.VOID;
        }

        //-----------------------------------------------------------
        public Type Visit(StatementList node) {
            VisitChildren(node);
            return Type.VOID;
        }

        //-----------------------------------------------------------
        public Type Visit(Assignment node) {

            var variableName = node.AnchorToken.Lexeme;

            if (Table.Contains(variableName)) {

                var expectedType = Table[variableName];

                if (expectedType != Visit((dynamic) node[0])) {
                    throw new SemanticError(
                        "Expecting type " + expectedType
                        + " in assignment statement",
                        node.AnchorToken);
                }

            } else {
                throw new SemanticError(
                    "Undeclared variable: " + variableName,
                    node.AnchorToken);
            }

            return Type.VOID;
        }

        //-----------------------------------------------------------
        public Type Visit(Print node) {
            node.ExpressionType = Visit((dynamic) node[0]);
            return Type.VOID;
        }

        //-----------------------------------------------------------
        public Type Visit(If node) {
            if (Visit((dynamic) node[0]) != Type.BOOL) {
                throw new SemanticError(
                    $"Expecting type {Type.BOOL} in conditional statement",
                    node.AnchorToken);
            }
            VisitChildren(node[1]);
            return Type.VOID;
        }

        //-----------------------------------------------------------
        public Type Visit(Identifier node) {

            var variableName = node.AnchorToken.Lexeme;

            if (Table.Contains(variableName)) {
                return Table[variableName];
            }

            throw new SemanticError(
                $"Undeclared variable: {variableName}",
                node.AnchorToken);
        }

        //-----------------------------------------------------------
        public Type Visit(IntLiteral node) {

            var intStr = node.AnchorToken.Lexeme;
            int value;

            if (!Int32.TryParse(intStr, out value)) {
                throw new SemanticError(
                    $"Integer literal too large: {intStr}",
                    node.AnchorToken);
            }

            return Type.INT;
        }

        //-----------------------------------------------------------
        public Type Visit(True node) {
            return Type.BOOL;
        }

        //-----------------------------------------------------------
        public Type Visit(False node) {
            return Type.BOOL;
        }

        //-----------------------------------------------------------
        public Type Visit(Neg node) {
            if (Visit((dynamic) node[0]) != Type.INT) {
                throw new SemanticError(
                    $"Operator - requires an operand of type {Type.INT}",
                    node.AnchorToken);
            }
            return Type.INT;
        }

        //-----------------------------------------------------------
        public Type Visit(And node) {
            VisitBinaryOperator('&', node, Type.BOOL);
            return Type.BOOL;
        }

        //-----------------------------------------------------------
        public Type Visit(Less node) {
            VisitBinaryOperator('<', node, Type.INT);
            return Type.BOOL;
        }

        //-----------------------------------------------------------
        public Type Visit(Plus node) {
            VisitBinaryOperator('+', node, Type.INT);
            return Type.INT;
        }

        //-----------------------------------------------------------
        public Type Visit(Mul node) {
            VisitBinaryOperator('*', node, Type.INT);
            return Type.INT;
        }

        //-----------------------------------------------------------
        void VisitChildren(Node node) {
            foreach (var n in node) {
                Visit((dynamic) n);
            }
        }

        //-----------------------------------------------------------
        void VisitBinaryOperator(char op, Node node, Type type) {
            if (Visit((dynamic) node[0]) != type ||
                Visit((dynamic) node[1]) != type) {
                throw new SemanticError(
                    $"Operator {op} requires two operands of type {type}",
                    node.AnchorToken);
            }
        }

        */
    }
}
