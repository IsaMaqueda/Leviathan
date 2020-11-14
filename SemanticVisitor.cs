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
using System.Text;
using System.Collections.Generic;

namespace Leviathan {

    // Singleton Design Pattern
    public class Globales{

        // Create instance of itself
        private static Globales instance = new Globales();

        public static HashSet<string> globalVariables = new HashSet<string>();
        
        public HashSet<string> getGlobalVariables(){
            return globalVariables;
        }


        private static FunctionTable functionTable{
            get;
            private set;
        }

        // Private constructor
        private Globales(){
            functionTable = new FunctionTable();


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

        // Return with a getter the only object available
        public static Globales getInstance(){
            return instance;
        }
        

        public static FunctionTable functionTable = new FunctionTable();

        public static Globales getTable(){
            return functionTable;
        }

    }

    class SemanticVisitor {

        // set para variables globales
        
        //Tablas para la primera pasada
        

        
        public SemanticVisitor() {
            // Instantiate & initialize global tables
            //Globales.functionTable = new FunctionTable();

            // Add predefined functions to global function table
            /*
            Globales.functionTable["printi"] = new FunctionRow(true, 1, null);
            Globales.functionTable["printc"] = new FunctionRow(true, 1, null);
            Globales.functionTable["prints"] = new FunctionRow(true, 1, null);
            Globales.functionTable["println"] = new FunctionRow(true, 0, null);
            Globales.functionTable["readi"] = new FunctionRow(true, 0, null);
            Globales.functionTable["reads"] = new FunctionRow(true, 0, null);
            Globales.functionTable["new"] = new FunctionRow(true, 1, null);
            Globales.functionTable["size"] = new FunctionRow(true, 1, null);
            Globales.functionTable["add"] = new FunctionRow(true, 2, null);
            Globales.functionTable["get"] = new FunctionRow(true, 2, null);
            Globales.functionTable["set"] = new FunctionRow(true, 3, null);
            */
        }
        
        

        public override string ToString() {
            var sb = new StringBuilder();
            sb.Append("Global Variables\n");
            sb.Append("====================\n");
            foreach (var entry in Globales.globalVariables) {
                sb.Append($"{entry}\n");
            }
            sb.Append("====================\n");
            return sb.ToString();
        }

        //-----------------------------------------------------------
                  

        //-----------------------------------------------------------
        public void Visit(Program node) {
            Visit((dynamic) node[0]); // def-list
        }


        public void Visit(DefList node) {
            VisitChildren(node); // def-list ::=  <def>* //def ::= <var-def>|<fun-def>
        }

        public void Visit(FunDef node) { //<fun-def> :: = <id> “(“ <id-list>? “)” “{“ <var-def-list><stmt-list> ”}”
            
            var funName = node.AnchorToken.Lexeme;
            if(Globales.functionTable.Contains(funName)){
                throw new SemanticError(
                    "Duplicated function: " + funName,
                    node.AnchorToken);
            } else {
                var numParams = Visit((dynamic)node[0]);
                Globales.functionTable[funName] = new FunctionRow(false, numParams, null);
                
            }
            
        }

        public void Visit(VarDef node){
            Visit((dynamic) node[0]);
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

            if (Globales.globalVariables.Contains(varName)) {
                throw new SemanticError(
                    "Duplicated variable: " + varName,
                    node.AnchorToken);
            } else {
                Globales.globalVariables.Add(varName);
            }
        }
        
        void VisitChildren(Node node) {
            foreach (var n in node) {
                Visit((dynamic) n);
            }
        }
    }
    /*
    class SemanticVisitor2 {
        
        public void Visit(Program node) {
            Visit((dynamic) node[0]); // def-list
            //Visit((dynamic) node[1]);
            
        }

        public void Visit(DefList node) {
            VisitChildren(node); // def-list ::=  <def>* //def ::= <var-def>|<fun-def>
        }

        public void Visit(FunDef node) { //<fun-def> :: = <id> “(“ <id-list>? “)” “{“ <var-def-list><stmt-list> ”}”
            
            //public HashSet<string> globalVariables = new HashSet<string>();
            var funName = node.AnchorToken.Lexeme;

            functionTable[funName].reference = new HashSet<string>();

            //Visit((dynamic) node[0]); // param-list (id-list)? DUDA
            Visit((dynamic) node[1], funName); // var-def-list
            Visit((dynamic) node[2], funName); // stmt-list             
        }

        public void Visit(VarDeflist node, string fName){
            Visit((dynamic) node[0], fName);
        }
        
        public void Visit(IdList node, string fName){
            foreach (var n in node) {
                Visit((dynamic) n, fName);
            }
            //VisitChildren(node);
        }

        public void Visit(Identifier node, string fName){
            functionTable[fName].reference.Add(node.AnchorToken.Lexeme);
        }

        public void Visit(StmtList node, string fName){
            foreach (var n in node) {
                Visit((dynamic) n, fName);
            }
        }

        public void Visit(StmtAssign node, string fName){ // <stmt-assign>::=<id>”=”<expr>”;”
            var varName = node.AnchorToken.Lexeme;
            functionTable[fName].reference.Add(varName);
            // TODO: visit sons, it can be any Lit node
            Visit((dynamic) node[0]); // expr
        }

        public void Visit(Int_Literal node){ 
            var catchVal = node.AnchorToken.Lexeme;
        }

        public void Visit(Char_Literal node){ 
            var catchVal = node.AnchorToken.Lexeme;
        }

        public void Visit(String_Literal node){ 
            var catchVal = node.AnchorToken.Lexeme;
        }

        public void Visit(Neg node){

        }

        public void Visit(ExprList node, string fName){
            foreach (var n in node) {
                Visit((dynamic) n, fName);
            }

        }


        void VisitChildren(Node node) {
            foreach (var n in node) {
                Visit((dynamic) n);
            }
        }
    }*/
    
}
