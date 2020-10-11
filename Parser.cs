/*
  Leviathan compiler - This class performs the syntactic analysis,
  (a.k.a. parsing).

  Camila Rovirosa A01024192
  Eduardo Badillo A01020716
  Isabel Maqueda  A01652906

  This program is free software: you can redistribute it and/or modify
  it under the terms of the GNU General Public License as published by
  the Free Software Foundation, either version 3 of the License, or
  (at your option) any later version.JAJ

  This program is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  GNU General Public License for more details.

  You should have received a copy of the GNU General Public License
  along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;

namespace Leviathan {

    class Parser {

        static readonly ISet<TokenCategory> firstOfDefinition = 
            new HashSet<TokenCategory>() {
                TokenCategory.VAR, // var-def
                TokenCategory.IDENTIFIER, // fun-def
            };

        static readonly ISet<TokenCategory> firstOfStatement = 
            new HashSet<TokenCategory>() {
                TokenCategory.IDENTIFIER,
                TokenCategory.IF,
                TokenCategory.ELIF,
                TokenCategory.ELSE,
                TokenCategory.WHILE,
                TokenCategory.DO,
                TokenCategory.BREAK,
                TokenCategory.RETURN,
                TokenCategory.SEMICOLON
            };
        static readonly ISet<TokenCategory> firstOfOpComp = 
            new HashSet<TokenCategory>() {
                TokenCategory.EQUAL, 
                TokenCategory.UNEQUAL, 
            };
        static readonly ISet<TokenCategory> firstOfOpRel = 
            new HashSet<TokenCategory>() {
                TokenCategory.MORE, 
                TokenCategory.LESS,
                TokenCategory.LESS_EQUAL, 
                TokenCategory.MORE_EQUAL, 
            };
        
        static readonly ISet<TokenCategory> firstOfOpAdd = 
            new HashSet<TokenCategory>() {
                TokenCategory.PLUS, 
                TokenCategory.NEG, 
            };
        static readonly ISet<TokenCategory> firstOfOpMul = 
            new HashSet<TokenCategory>() {
                TokenCategory.MUL, 
                TokenCategory.DIV, 
                TokenCategory.MOD,
            };

        static readonly ISet<TokenCategory> firstOfOpUnary = 
            new HashSet<TokenCategory>() {
                TokenCategory.PLUS, 
                TokenCategory.LESS, 
                TokenCategory.NOT,
            };


        /*
        static readonly ISet<TokenCategory> firstOfExprPrimary = 
            new HashSet<TokenCategory>() {
                TokenCategory.IDENTIFIER, 
                TokenCategory.BRACKET_OPEN, 
                TokenCategory.TRUE,
                TokenCategory.FALSE,
                TokenCategory.CHAR_LITERAL,
                TokenCategory.INT_LITERAL,
                TokenCategory.STRING_LITERAL,
                TokenCategory.PARENTHESIS_OPEN
            };
        */
        IEnumerator<Token> tokenStream;

        public Parser(IEnumerator<Token> tokenStream) {
            this.tokenStream = tokenStream;
            this.tokenStream.MoveNext();
        }

        public TokenCategory CurrentToken {
            get { return tokenStream.Current.Category; }
        }

        public Token Expect(TokenCategory category) {
            if (CurrentToken == category) {
                Token current = tokenStream.Current;
                tokenStream.MoveNext();
                return current;
            } else {
                throw new SyntaxError(category, tokenStream.Current);
            }
        }

        public void Program() //Prog ::= <def-list>
        {
           DefList();
           Expect(TokenCategory.EOF);
        }

        public void DefList() // def-list ::=  <def>*
        { 
            while(firstOfDefinition.Contains(CurrentToken)){
                Def();
            }
        }
        
        public void Def() //def ::= <var-def>|<fun-def>
        {
            switch(CurrentToken){
                case TokenCategory.VAR:
                    VarDef();
                    break;
                case TokenCategory.IDENTIFIER:
                    FunDef();
                    break; 
                default:
                throw new SyntaxError(firstOfDefinition,
                                      tokenStream.Current);
            }
        }

        public void VarDef()//<var-def> :: = “var” <var-list> “;”
        {
            Expect(TokenCategory.VAR);
            VarList();
            Expect(TokenCategory.SEMI_COLON);
        }

        public void VarList() //<var-list>::=<id-list>
        {
            IdList();
        }

        public void IdList() //<id-list> ::= <id> <id-list-cont>
        {
            Expect(TokenCategory.IDENTIFIER);
            IdListCont();
        }
        
        public void IdListCont() //<id-list-cont>::= , ¨<id>(<id-list-cont>)*
        {
            Expect(TokenCategory.COMMA);
            Expect(TokenCategory.IDENTIFIER);
            while(CurrentToken==TokenCategory.COMMA){
                IdListCont();
            }
            
        }

        public void FunDef() //<fun def> :: = <id> “(“ <param-list> “)” “{“ <var-def-list><stmt-list> ”}”
        {
            Expect(TokenCategory.IDENTIFIER);
            Expect(TokenCategory.PARENTHESIS_OPEN);
            ParamList();
            Expect(TokenCategory.PARENTHESIS_CLOSE);
            Expect(TokenCategory.BRACE_OPEN);
            VarDefList();
            StmtList();
            Expect(TokenCategory.BRACE_CLOSE);

        }
        
        public void ParamList() //  <param-list> ::= <id-list>?  DUDA 1  
        {
            if(CurrentToken == TokenCategory.IDENTIFIER)
            {
                IdList();
            }
        }

        public void VarDefList()// <var-def-list> ::= <var-def>*
        {
            while (CurrentToken == TokenCategory.VAR)
            {
                VarDef();
            }
        }

        public void StmtList()//<stmt-list> ::= <stmt>*
        {
            while (firstOfStatement.Contains(CurrentToken)){
                Stmt();
            }
        }

        public void Stmt() { //<stmt>::=‹stmt-assign›|‹stmt-incr›|‹stmt-decr›|‹stmt-fun-call›|‹stmt-if›|‹stmt-while›|‹stmt-do-while›|‹stmt-break›|‹stmt-return›|‹stmt-empty›
            //Expect(TokenCategory.IDENTIFIER); // DUDA 2 
            switch (CurrentToken) {

            /*case TokenCategory.IDENTIFIER:
                    StmtIdentifier();
                    break; */
            
            case TokenCategory.ASSIGN:
                StmtAssign();
                break;

            case TokenCategory.INCR:
                StmtIncr();
                break;

            case TokenCategory.DECR:
                StmtDecr();
                break;

            /*case TokenCategory.:  //DUDA 2.1
                StmtFunCall(); 
                break;*/
            
            case TokenCategory.IF:
                StmtIf();
                break;

            case TokenCategory.WHILE:
                StmtWhile();
                break;
            case TokenCategory.DO:
                StmtDoWhile();
                break;
            
            case TokenCategory.BREAK:
                StmtBreak();
                break;

            case TokenCategory.RETURN:
                StmtReturn();
                break;
            
            case TokenCategory.SEMICOLON:
                StmtEmpty();
                break;
            default:
                throw new SyntaxError(firstOfStmt, tokenStream.Current);
            }
        }
        /* DUDA
        public void StmtIdentifier()
        {
            Expect(TokenCategory.IDENTIFIER);

            switch (CurrentToken) {
                case TokenCategory.ASSIGN:
                StmtAssign();
                break;

                case TokenCategory.INCR:
                StmtIncr();
                break;

                case TokenCategory.DECR:
                StmtDecr();
                break;

                case TokenCategory.PARENTHESIS_OPEN:
                stmt-fun-call();
                
            }
        }
        */
        public void StmtAssign() //<stmt-assign>::=<id>”=”<expr>”;”
        { 
            Expect(TokenCategory.IDENTIFIER); //  not if used StmtIDentifier 
            Expect(TokenCategory.ASSIGN);
            Expr();
            Expect(TokenCategory.SEMI_COLON);
        }
        public void StmtIncr() // <stmt-assign>::=<id>”++” ”;”
        {
            Expect(TokenCategory.IDENTIFIER); //  not if used StmtIDentifier 
            Expect(TokenCategory.INCR);
            Expect(TokenCategory.SEMI_COLON);
        }


        public void StmtDecr()
        {
            Expect(TokenCategory.IDENTIFIER); //
            Expect(TokenCategory.DECR);
            Expect(TokenCategory.SEMI_COLON);
        }

        public void StmtFunCall() //<stm-fun-call>::=<fun-call>”;”
        {
            FunCall();
            Expect(TokenCategory.SEMI_COLON);
        }
        public void FunCall(){ //‹fun-call› :: = ‹id› “(“ ‹expr-list› “)”
            Expect(TokenCategory.IDENTIFIER); // 
            Expect(TokenCategory.PARENTHESIS_OPEN);
            ExprList();
            Expect(TokenCategory.PARENTHESIS_CLOSE);
        }
        public void ExprList(){ //<expr-list> ::= (<expr><expr-list-cont>)?
             if(CurrentToken == TokenCategory.IDENTIFIER)
            {
                Expr();
                ExprListCont();
            }
        }
        public void ExprListCont(){ //<expr-list-cont> ::=( “,” <expr>)*
            
            while(CurrentToken == TokenCategory.COMMA)
            {
                //Expect(TokenCategory.COMMA);
                Expr();
            }
        }
        
        public void StmtIf() //<stmt>::=”if” ”(“<expr>”)” ”{“  <stmt-list>”}” <else-if-list><else>
        {
            Expect(TokenCategory.IF);
            Expect(TokenCategory.PARENTHESIS_OPEN);
            Expr();
            Expect(TokenCategory.PARENTHESIS_CLOSE);
            Expect(TokenCategory.BRACE_OPEN);
            StmtList();
            Expect(TokenCategory.BRACE_CLOSE);
            ElseIfList();
            Else();
            
        }
        public void ElseIfList() // <else-if-list>::= (“elif” ”(“ <expr> “)” “{“<stmt-list>”}”)*
        {
            while(CurrentToken == TokenCategory.ELIF){
                Expect(TokenCategory.ELIF);
                Expect(TokenCategory.PARENTHESIS_OPEN);
                Expr();
                Expect(TokenCategory.BRACE_OPEN);
                StmtList();
                Expect(TokenCategory.BRACE_OPEN);
            }
            
        }
        
        public void Else() //<else> ::=(  “else” “{“ <stmt-list> “}” )?
        {
            if(CurrentToken == TokenCategory.ELSE)
            {   Expect(TokenCategory.ELSE);
                Expect(TokenCategory.BRACE_OPEN);
                StmtList();
                Expect(TokenCategory.BRACE_OPEN);
            }
        }

        public void StmtWhile(){ // <stmt-while>::=”while” “(“ <expr> “)” “{“ <stmt-list> “}”
            Expect(TokenCategory.WHILE); 
            Expect(TokenCategory.PARENTHESIS_OPEN);
            Expr();
            Expect(TokenCategory.PARENTHESIS_CLOSE);
            Expect(TokenCategory.BRACE_OPEN);
            StmtList();
            Expect(TokenCategory.BRACE_CLOSE);
        }

        public void StmtDoWhile(){ //<stmt-do-while>::=”do” “{“ <stmt-list>”}” “while” “(“<expr>”)” “;”
            Expect(TokenCategory.DO);
            Expect(TokenCategory.BRACE_OPEN);
            StmtList();
            Expect(TokenCategory.BRACE_CLOSE);
            Expect(TokenCategory.WHILE);
            Expect(TokenCategory.PARENTHESIS_OPEN);
            Expr();
            Expect(TokenCategory.PARENTHESIS_CLOSE);
            Expect(TokenCategory.SEMI_COLON);
        }

        public void StmtBreak() //<stmt-break> ::= “”break” ”;”
        {
            Expect(TokenCategory.BREAK);
            Expect(TokenCategory.SE);
        }
        
        public void StmtReturn() //<stmt> ::= “return” <expr> “;”
        {
            Expect(TokenCategory.RETURN);
            Expr();
            Expect(TokenCategory.SEMI_COLON);
        }
        
       public void StmtEmpty() //<stmt-empty> ::= “;”
       {
           Expect(TokenCategory.SEMI_COLON);
       }
        public void Expr() //<expr> ::= <expr-or>
        { 
            ExprOr();
        }
        public void ExprOr() //‹expr-or›::= ‹expr-and› ("||" ‹expr-and›)*
        {
            ExprAnd();
            while(CurrentToken == TokenCategory.OR){
                Expect(TokenCategory.OR);
                ExprAnd();
            }
        }
        public void ExprAnd() //‹expr-and› ::= ‹expr-comp› ("&&" ‹expr-comp›)*
        {
            ExprComp();
            while(CurrentToken == TokenCategory.AND){
                Expect(TokenCategory.AND);
                ExprComp();
            }
        }

        public void ExprComp() //<expr-comp>::= <expr-rel>(<op-comp><expr-rel>)*
        {
            ExprRel();
            while(firstOfOpComp.Contains(CurrentToken)){
                OpComp();
                ExprRel();
            }
            
        }

        public void OpComp(){
            switch(CurrentToken){
                case TokenCategory.EQUAL:
                    Expect(TokenCategory.EQUAL);
                    break;

                case TokenCategory.UNEQUAL:
                    Expect(TokenCategory.UNEQUAL);
                    break;
                default:
                    throw new SyntaxError(firstOfOpComp,tokenStream.Current);
            }
        }
        public void ExprRel() //<expr-rel>::=<expr-add>(<op-rel><expr-add>)*
        {
            ExprAdd();
            while(firstOfOpRel.Contains(CurrentToken)){
                OpRel();
                ExprAdd();
            }
            
        }

        public void OpRel(){ //<opr-rel> ::= “<”| “<=” | “>” | “>=”
            switch(CurrentToken){
                
                case TokenCategory.LESS:
                    Expect(TokenCategory.LESS);
                    break;

                case TokenCategory.LESS_EQUAL:
                    Expect(TokenCategory.LESS_EQUAL);
                    break;
                    
                case TokenCategory.MORE:
                    Expect(TokenCategory.MORE);
                    break;
                
                case TokenCategory.MORE_EQUAL:
                    Expect(TokenCategory.MORE_EQUAL);
                    break;
                    
                default:
                    throw new SyntaxError(firstOfOpRel,tokenStream.Current);
            }
            
        }
        public void ExprAdd() // <expr-add> ::= <expr-mul>(<op-add><expr-mul>) * 
        {
            ExprMul();
            while(firstOfOpAdd.Contains(CurrentToken)){
                OpAdd();
                ExprMul();
            }
            
        }


        public void OpAdd() // <op-add>::= “+” | “-”
        {
            switch(CurrentToken){
                case TokenCategory.PLUS:
                    Expect(TokenCategory.PLUS);
                    break;
                case TokenCategory.LESS:
                    Expect(TokenCategory.LESS); 
                    break;                
                default:
                    throw new SyntaxError(firstOfOpAdd,tokenStream.Current);
            }
        }

        public void ExprMul() // <expr-mul> ::= <expr-unary>(<op-mul><expr-unary>)*
        {
            ExprUnary();
            while(firstOfOpMul.Contains(CurrentToken)){
                OpMul();
                ExprUnary();
            }
            
        }

        public void OpMul(){ //<op-mul>::= ”*” | “/” | “%”
             switch(CurrentToken){
                case TokenCategory.MUL:
                    Expect(TokenCategory.MUL);
                    break;
                case TokenCategory.DIV:
                    Expect(TokenCategory.DIV); 
                    break;
                case TokenCategory.MOD:
                    Expect(TokenCategory.MOD); 
                    break;
                default: 
                    throw new SyntaxError(firstOfOpMul,
                                      tokenStream.Current);

            }
        }
        public void ExprUnary() // ‹expr-unary› ::=  ‹op-unary› * ‹expr-primary›
        {
            while(firstOfOpUnary.Contains(CurrentToken)){
                OpUnary();    
            }
            ExprPrimary();
        }
        public void OpUnary()
        {
            switch(CurrentToken){
                case TokenCategory.PLUS:
                    Expect(TokenCategory.PLUS);
                    break;
                case TokenCategory.NEG:
                    Expect(TokenCategory.NEG); 
                    break;
                case TokenCategory.NOT:
                    Expect(TokenCategory.NOT); 
                    break;
                default: 
                    throw new SyntaxError(firstOfOpUnary,
                                      tokenStream.Current);
            }
        }
      
        public void ExprPrimary(){ //<expr-primary> ::= <id> | <fun-call> | <array> | <lit> | (“(“ <expr> “)”) DUDA
            switch(CurrentToken){
                case TokenCategory.IDENTIFIER:
                    Expect(TokenCategory.IDENTIFIER);
                    break;
                case TokenCategory:
                    break;
            }
        }

        public void Array() //<array>::= “[“ <expr-list> “]”
        {
            Expect(TokenCategory.BRACE_OPEN);
            ExprList();
            Expect(TokenCategory.BRACE_CLOSE);
            
        }
        
        public void Lit(){ //<lit>::= <lit-bool>|<lit-int>|<lit-char>|<lit-str
             switch(CurrentToken){
                case TokenCategory.TRUE :
                Expect(TokenCategory.TRUE);
                    break;
                case TokenCategory.FALSE:
                    Expect(TokenCategory.FALSE);
                    break;
                    
                case TokenCategory.INT_LITERAL:
                    Expect(TokenCategory.INT_LITERAL); 
                    break;
                case TokenCategory.CHAR_LITERAL:
                    Expect(TokenCategory.CHAR_LITERAL); 
                    break;
                case TokenCategory.STRING_LITERAL:
                    Expect(TokenCategory.STRING_LITERAL); 
                    break;
                default: 
                    throw new SyntaxError(firstOfOpUnary,
                                      tokenStream.Current);
            }
        }
    
    }
}
