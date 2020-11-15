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
        private static Globales instance;

        public static HashSet<string> globalVariables = new HashSet<string>();
        
        public HashSet<string> getGlobalVariables(){
            return globalVariables;
        }

        public void setGlobalVariables(string name){
            globalVariables.Add(name);
        }

        private static FunctionTable functionTable;

        public FunctionTable getGlobalFunctions(){
            return functionTable;
        }

        public void setFunctionTable(string name, bool primitive, int arity, HashSet<string> localSymbolTable){
            functionTable[name] = new FunctionRow(primitive, arity, localSymbolTable);
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

            if (instance == null){
                instance = new Globales();
                
            }
            
                return instance;
        }

    }

    class SemanticVisitor {

        // set para variables globales
        
        private Globales tables;
        //Tablas para la primera pasada
        
        public SemanticVisitor() {
            // Instantiate & initialize global tables
            
            //Globales globales  = Globales.getInstance();
            
            tables = Globales.getInstance();
        }

        public override string ToString() {
            var sb = new StringBuilder();
            sb.Append("Global Variables\n");
            sb.Append("====================\n");
            foreach (var entry in tables.getGlobalVariables()) {
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
            if(tables.getGlobalFunctions().Contains(funName)){
                throw new SemanticError(
                    "Duplicated function: " + funName,
                    node.AnchorToken);
            } else {
                var numParams = Visit((dynamic)node[0]);
                tables.setFunctionTable(funName, false, numParams, null);
                //Globales.functionTable[funName] = new FunctionRow(false, numParams, null);
                
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

        public void Visit(IdList node) { //<id-list> ::= <id> (“,” <id>)*
            VisitChildren(node); // Identifiers*
        }

        public void Visit(Identifier node){
            var varName = node.AnchorToken.Lexeme;

            if (tables.getGlobalVariables().Contains(varName)) {
                throw new SemanticError(
                    "Duplicated variable: " + varName,
                    node.AnchorToken);
            } else {
                tables.setGlobalVariables(varName);
                //Globales.globalVariables.Add(varName);
            }
        }
        
        void VisitChildren(Node node) {
            foreach (var n in node) {
                Visit((dynamic) n);
            }
        }
    }
    
    class SemanticVisitor2 {


        // set para variables globales
        private Globales tables;
        //Tablas para la primera pasada
        
        public SemanticVisitor2() {
            // Instantiate & initialize global tables
            
            //Globales globales = Globales.getInstance();
            
            tables = Globales.getInstance();
        }
        
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
            table.getGlobalFunctions()[funName].reference = new HashSet<string>();
            
            //Visit((dynamic) node[0], funName); // ParamList
            //Visit((dynamic) node[1], funName); // var-def-list
            //Visit((dynamic) node[2], funName); // stmt-list             
            VisitChildren((dynamic) node, funName);
        }
        
        public void Visit(ParamList node, string fName){ // sons : Identifiers*
            VisitChildren((dynamic) node, fName);
        }
        
        public void Visit(VarDeflist node, string fName){ // sons : VarDef*
            VisitChildren((dynamic) node, fName);
        }

        public void Visit(VarDef node, string fName){ // son : IdList
            Visit((dynamic) node[0], fName);
        }
        
        public void Visit(IdList node, string fName){ // sons : Identifiers*
            //HashSet<string> localVarTable = new HashSet<string>;
            VisitChildren((dynamic) node, fName);
        }

        //checar que nodo padre no sea Var Def, si si es y encuentra la referencia en getGlobal marcar error de Duplicate
        public void Visit(Identifier node, string fName){ // I have no sons! =)
            var varName = node.AnchorToken.Lexeme;
            if(tables.getGlobalFunctions()[fName].reference.Contains(varName)){
                throw new SemanticError(
                    "Duplicated variable: " + varName + "in " + fName,
                    node.AnchorToken);
            }else {
                tables.getGlobalFunctions()[fName].reference.Add(varName);
            }
        }

        public void Visit(StmtList node, string fName){ // sons: StmtAssign, StmtFunCall, StmtWhile, StmtDoWhile, StmtIncr, StmtDecr, StmtIf, StmtBreak,StmtReturn, StmtEmpty
            VisitChildren((dynamic) node, fName);
        }

        public void Visit(StmtAssign node, string fName){ // sons: (+ - !) (* / %) (true false) (identifier, fun-call, array, lit) 

            var varName = node.AnchorToken.Lexeme;
            if(tables.getGlobalFunctions()[fName].reference.Contains(varName)){
                // 
            }else {
                throw new SemanticError(
                    "Expected variable: " + varName + "in " + fName,
                    node.AnchorToken);
            }
            
            //functionTable[fName].reference.Add(varName);
            // TODO: visit sons, it can be any Lit node
            Visit((dynamic) node[0]); // expr
        }

        // LIT
        public void Visit(Int_Literal node, string fName){ 
            var catchVal = node.AnchorToken.Lexeme;
        }

        public void Visit(Char_Literal node, string fName){ 
            var catchVal = node.AnchorToken.Lexeme;
        }

        public void Visit(String_Literal node, string fName){ 
            var catchVal = node.AnchorToken.Lexeme;
        }

        // OpUnary
        public void Visit(Neg node, string fName){
            VisitChildren(node, fName);
        }

        public void Visit(Plus node, string fName){
            VisitChildren(node, fName);
        }

        // OpMul
        public void Visit (Mul node, string fname){
            VisitChildren(node, fName);
        }
        public void Visit (Div node, string fname){
            VisitChildren(node, fName);
        }
        public void Visit (Mod node, string fname){
            VisitChildren(node, fName);
        }
        
        // TRUE && FALSE
        public void Visit(True node, string fName){
            // I HAVE NO KIDS! =)            
        }

        public void Visit(False node, string fName){
            // I HAVE NO KIDS! =)
        }
        ///

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
        
        void VisitChildren(Node node, string fName){
            foreach (var n in node) {
                Visit((dynamic) n, fName);
            }
        }
    }
    
}