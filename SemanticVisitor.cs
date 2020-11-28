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

        public HashSet<string> getFunctionReference(string fName){
            return functionTable[fName].getReference();
        }

        public void setFunctionTable(string name, bool primitive, int arity){
            functionTable[name] = new FunctionRow(primitive, arity);
            //functionTable[name].reference = new HashSet<string>();
        }

        public void addLocalVariable(string name, string variable){
            functionTable[name].setReference(variable);
        }

        public int getFunctionArity(string name)
        {
            return functionTable[name].arity;
        }
        

        // Private constructor
        private Globales(){
            functionTable = new FunctionTable();

            functionTable["printi"] = new FunctionRow(true, 1);
            functionTable["printc"] = new FunctionRow(true, 1);
            functionTable["prints"] = new FunctionRow(true, 1);
            functionTable["println"] = new FunctionRow(true, 0);
            functionTable["readi"] = new FunctionRow(true, 0);
            functionTable["reads"] = new FunctionRow(true, 0);
            functionTable["new"] = new FunctionRow(true, 1);
            functionTable["size"] = new FunctionRow(true, 1);
            functionTable["add"] = new FunctionRow(true, 2);
            functionTable["get"] = new FunctionRow(true, 2);
            functionTable["set"] = new FunctionRow(true, 3);
            
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
        
        //-----------------------------------------------------------
                  

        //-----------------------------------------------------------
        public void Visit(Program node) {
            VisitChildren((dynamic) node); // son : DefList
        }


        public void Visit(DefList node) {
            VisitChildren(node); // sons : (FunDef | VarDef)*
        }

        public void Visit(FunDef node) { // visited sons :   
        
            var funName = node.AnchorToken.Lexeme;
            if(tables.getGlobalFunctions().Contains(funName)){
                throw new SemanticError(
                    "Duplicated function: " + funName,
                    node.AnchorToken);
            } else {
                var numParams = Visit((dynamic)node[0]);
                
                tables.setFunctionTable(funName, false, numParams);
                //Globales.functionTable[funName] = new FunctionRow(false, numParams, null);
            }
            
        }

        public void Visit(VarDef node){
            Visit((dynamic) node[0]); // son : IdDefList
        }

        public void Visit(IdDefList node){
            VisitChildren(node); // NewIdentifiers*
        }

        public void Visit(NewIdentifier node){ // no sons
            var varName = node.AnchorToken.Lexeme;

            if (tables.getGlobalVariables().Contains(varName) ) {
                throw new SemanticError(
                    "Duplicated variable: " + varName,
                    node.AnchorToken);
            } else {
                tables.setGlobalVariables(varName);
                //Globales.globalVariables.Add(varName);
            }
        }

        public int Visit(ParamList node){
            var sonCtr = 0;
            
            foreach (var n in node){
                sonCtr++;
            }
            return sonCtr;
        }

        public void Visit(EOF node){
            if(tables.getGlobalFunctions().Contains("main")){
                // all good 
            } else {
                throw new SemanticError(
                    "Reached end of file, no main function was declared.",
                    node.AnchorToken);
            }
        }

        void VisitChildren(Node node) {
            foreach (var n in node) {
                Visit((dynamic) n);
            }
        }
    }
    
    class SemanticVisitor2 {

        //bool inLoop = false; // considerar como un entero
        public int inLoop = 0;
        string funCallName;
        Node parentNode;
        // set para variables globales
        private Globales tables;
        //Tablas para la primera pasada
        
        public SemanticVisitor2() {
            // Instantiate & initialize global tables
            
            //Globales globales = Globales.getInstance();
            
            tables = Globales.getInstance();
        }

        public void Visit(Program node) {
            Visit((dynamic) node[0]); // son : DefList
            //Visit((dynamic) node[1]);
        }

        public void Visit(DefList node) {
            VisitChildren((dynamic) node); // sons : (FunDef | VarDef)*
        }

        public void Visit(VarDef node){
            // DON'T DO ANYTHING
        }

        public void Visit(FunDef node) { // sons : ParamList, var-def-list, stmt-list
            var funName = node.AnchorToken.Lexeme;
            
            //Visit((dynamic) node[0], funName); // ParamList
            //Visit((dynamic) node[1], funName); // var-def-list
            //Visit((dynamic) node[2], funName); // stmt-list             
            VisitChildren( node, funName);
        }
        
        public void Visit(ParamList node, string fName){ // sons : NewIdentifiers*
            VisitChildren( node, fName);
        }
        
        public void Visit(VarDefList node, string fName){ // sons : VarDef*
            VisitChildren( node, fName);
        }

        public void Visit(VarDef node, string fName){ // son : IdDefList
            Visit((dynamic) node[0], fName);
        }

        public void Visit(IdDefList node, string fName){ // sons : NewIdentifiers*
            VisitChildren( node, fName);
        }

        public void Visit(NewIdentifier node, string fName){ // no sons!
            var varName = node.AnchorToken.Lexeme;
            if(tables.getFunctionReference(fName).Contains(varName)){
                throw new SemanticError(
                    "Duplicated variable: " + varName + " in " + fName,
                    node.AnchorToken);
            }else {
                tables.addLocalVariable(fName, varName);
            }
        }
        
        // DUDA
        public void Visit(IdList node, string fName){ // sons : Identifiers*
            VisitChildren( node, fName);
        }

        public void Visit(Identifier node, string fName){ // I have no sons! =)
           var varName = node.AnchorToken.Lexeme;
            if(tables.getFunctionReference(fName).Contains(varName) || tables.getGlobalVariables().Contains(varName)){
                // This variable has already been declared and it's okay to use it :)
            } else {
                throw new SemanticError(
                    "This variable : " + varName + " in " + fName + " hasn't been declared",
                    node.AnchorToken);
            }
        }
        //
        public void Visit(StmtList node, string fName){ // sons: StmtAssign, StmtFunCall, StmtWhile, StmtDoWhile, StmtIncr, StmtDecr, StmtIf, StmtBreak,StmtReturn, StmtEmpty
            VisitChildren( node, fName);
        }

        public void Visit(StmtAssign node, string fName){ // sons: (+ - !) (* / %) (true false) (identifier, fun-call, array, lit) 

            parentNode = (dynamic)node;
            var varName = node.AnchorToken.Lexeme;
            if(tables.getFunctionReference(fName).Contains(varName) || tables.getGlobalVariables().Contains(varName)){ // exists?
                Visit((dynamic) node[0], fName); // stmt assign can only have 1 children
            }else { // else 
                throw new SemanticError(
                    "Variable: " + varName + " hasn't been declared",
                    node.AnchorToken);
            }
            
            //functionTable[fName].reference.Add(varName);
            // TODO: visit sons, it can be any Lit node
            //Visit((dynamic) node[0]); // expr
        }

        // LITS
        public void Visit(Int_Literal node, string fName){ //VALIDATE INT LITERAL
            var catchVal = node.AnchorToken.Lexeme;

            int result;
            if (!Int32.TryParse(catchVal, out result)) {
                throw new SemanticError(
                    "Variable: " + catchVal + " isn't in the aproved range of −2^31  and 2^31−1",
                    node.AnchorToken);
            }
        }

        public void Visit(Char_Literal node, string fName){ 
            var catchVal = node.AnchorToken.Lexeme;
        }

        public void Visit(String_Literal node, string fName){ 
            var catchVal = node.AnchorToken.Lexeme;
        }

        // TRUE && FALSE
        public void Visit(True node, string fName){
            // I HAVE NO KIDS! =)            
        }

        public void Visit(False node, string fName){
            // I HAVE NO KIDS! =)
        }

        public void Visit(UnEqual node, string fName){
            VisitChildren(node, fName);
        }


        public void Visit(Array node, string fName){ // son: expr-list
            funCallName = "array";
            
            Visit((dynamic) node[0], fName); 
        }
         // OpAdd
        public void Visit(Neg node, string fName){
            VisitChildren( node, fName);
        }

        public void Visit(Plus node, string fName){
            VisitChildren( node, fName);
        }

        public void Visit(Not node, string fName){
            VisitChildren( node, fName);
        }

        // OpMul
        public void Visit (Mul node, string fName){
            VisitChildren( node, fName);
        }
        public void Visit (Div node, string fName){
            VisitChildren( node, fName);
        }
        public void Visit (Mod node, string fName){
            VisitChildren( node, fName);
        }
        
        // OpRel
        public void Visit(Less node, string fName){
            VisitChildren( node , fName);
        }

        public void Visit(Less_Equal node, string fName){
            VisitChildren( node, fName);
        }

        public void Visit(More node, string fName){
            VisitChildren( node, fName);
        }

        public void Visit(More_Equal node, string fName){
            VisitChildren( node, fName);
        }

        public void Visit(Equal node, string fName){
            VisitChildren(node, fName);
        }

        // OpUnary
        public void Visit (StmtFunCall node, string fName){ //sons:  exprlist
            string funName = node.AnchorToken.Lexeme;
            funCallName = funName;
            parentNode = (dynamic)node;
            //Console.WriteLine(tables.getGlobalFunctions().Contains(funName));
            if(tables.getGlobalFunctions().Contains(funName)){
                // The function exists
                //Console.WriteLine("Hi");
                Visit((dynamic) node[0], fName); //exprlist
            } else {
                throw new SemanticError(
                    "Undeclared function: " + funName, node.AnchorToken);
            }
        }

        
        public void Visit (FunCall node, string fName){ //sons:  exprlist
            string funName = node.AnchorToken.Lexeme;
            funCallName = (dynamic)funName;
            if(tables.getGlobalFunctions().Contains(funName)){
                // The function exists
                //Console.WriteLine("Hi");
                Visit((dynamic) node[0], fName); //exprlist
            } else {
                throw new SemanticError(
                    "Undeclared function: " + funName, node.AnchorToken);
            }
        }
        public void Visit (StmtIncr node, string fName){
            var varName = node.AnchorToken.Lexeme;
            if(tables.getFunctionReference(fName).Contains(varName) || tables.getGlobalVariables().Contains(varName)){ // exists?
                // variable exists, no sons
            }else {  
                throw new SemanticError(
                    "Variable: " + varName + " hasn't been declared in " + fName,
                    node.AnchorToken);
            }
        }
        public void Visit (StmtDecr node, string fName){
            var varName = node.AnchorToken.Lexeme;
            if(tables.getFunctionReference(fName).Contains(varName) || tables.getGlobalVariables().Contains(varName)){ // exists?
                // variable exists, no sons
            }else {  
                throw new SemanticError(
                    "Variable: " + varName + " hasn't been declared in " + fName,
                    node.AnchorToken);
            }
        }
        public void Visit (StmtBreak node, string fName){ //sons: 
        //No tiene hijos
            //Console.WriteLine(inLoop);
            if(inLoop==0){
                throw new SemanticError(
                    "Break not in a while or do while: in " + fName,
                    node.AnchorToken);
            }
        }
           //No tiene hijos
        public void Visit (StmtReturn node, string fName){ //sons:  
           Visit((dynamic) node[0], fName);
        }
        public void Visit (StmtEmpty node, string fName){ //sons:  
           //No tiene hijos
        }

        public void Visit(StmtWhile node, string fName){
            inLoop++;
            Visit((dynamic) node[0], fName);
            Visit((dynamic) node[1], fName);
            inLoop--;
        }

        public void Visit(StmtDoWhile node , string fName){
            inLoop++;
            VisitChildren(node, fName);
            inLoop--;
        }

        public void Visit(StmtIf node, string fName){
            VisitChildren(node, fName);
            
        }

        public void Visit(ElseIfList node, string fName){
            VisitChildren(node, fName);
        }

        public void Visit(Elif node, string fName){
            VisitChildren(node, fName);
        }

        public void Visit(Else node, string fName){
            Visit((dynamic) node[0], fName);
        }

        public void Visit(ExprOr node, string fName){
            VisitChildren( node, fName);
        }

        public void Visit(ExprAnd node, string fName){
            VisitChildren( node, fName);
        }


         //stmt list 

        //

        public void Visit(ExprList node, string fName){ // sons: LITERALS* IDENTIFIER* FUNCALL* ARRAY? EMPTY
           // Received the fName of the function to call
           
           if(funCallName!="array"){
            int sonCtr = 0;
            foreach (var n in node){
                sonCtr++;
            }
            
                int acceptedParams = tables.getGlobalFunctions()[funCallName].getArity();
                
                if (acceptedParams != sonCtr){
                    throw new SemanticError(
                        "The function " + funCallName + " accepts " + acceptedParams + " args, but " + sonCtr + " were found.",
                        parentNode.AnchorToken);
            } else {
                VisitChildren(node, fName); 
            }
           } else {
            VisitChildren(node, fName);
           }

        }

        public void Visit(Condition node, string fName){
            Visit((dynamic) node[0], fName);
        }

        public void Visit(LoopCondition node, string fName){
            Visit((dynamic) node[0], fName);
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