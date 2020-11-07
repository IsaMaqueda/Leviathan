/*
  Leviathan compiler - Semantic Analysis

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
                TokenCategory.SEMI_COLON,
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
                TokenCategory.NEG, 
                TokenCategory.NOT,
            };

        static readonly ISet<TokenCategory> firstOfLit = 
            new HashSet<TokenCategory>() {
                TokenCategory.TRUE, 
                TokenCategory.FALSE, 
                TokenCategory.INT_LITERAL,
                TokenCategory.CHAR_LITERAL,
                TokenCategory.STRING_LITERAL,
            };
        
        static readonly ISet<TokenCategory> firstOfExpr = 
            new HashSet<TokenCategory>() {
                TokenCategory.PLUS,
                TokenCategory.LESS,
                TokenCategory.NOT, 
                TokenCategory.IDENTIFIER,
                TokenCategory.BRACE_OPEN,
                TokenCategory.PARENTHESIS_OPEN,
                TokenCategory.TRUE,
                TokenCategory.FALSE, 
                TokenCategory.INT_LITERAL,
                TokenCategory.CHAR_LITERAL,
                TokenCategory.STRING_LITERAL,
            };
        
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

        public Node Program() // Prog ::= <def-list>
        { 
          var prog = new Program();
          prog.Add(DefList());
            //DefList();
          Expect(TokenCategory.EOF);
          return prog;
        }
        


        public Node DefList() // def-list ::=  <def>*
        { 
            var deflist = new DefList();
            //var def = new Def();
            while(firstOfDefinition.Contains(CurrentToken)){
                // add node hijo
                deflist.Add(Def());
                //Def();
            }
            return deflist;
        }
        
        public Node Def() //def ::= <var-def>|<fun-def>
        {
            
            switch(CurrentToken){
                case TokenCategory.VAR:
                    return VarDef();
                case TokenCategory.IDENTIFIER:
                    return FunDef();
                default:
                throw new SyntaxError(firstOfDefinition,
                                      tokenStream.Current);
            }
        }

        public Node VarDef()//<var-def> :: = “var” <var-list> “;”
        {
            var vardef = new VarDef(){
                AnchorToken = Expect(TokenCategory.VAR)
            };
            
            //vardef.Add(VarList()); <var-list>::=<id-list>
            //var varlist = new VarList();
            //varlist.Add(IdList());

            //vardef.Add(varlist);
            vardef.Add(IdList());
            ///    
            Expect(TokenCategory.SEMI_COLON);

            return vardef;
            /*Expect(TokenCategory.VAR);
            VarList();
            Expect(TokenCategory.SEMI_COLON);*/
        }

        /*
        public Node VarList() //<var-list>::=<id-list> no es nodo esta la podemos quitar
        {
            var varlist = new VarList();
            varlist.Add(IdList());
            
            return varlist;
            //IdList();
        }
        */

        public Node IdList() //<id-list> ::= <id> (“,” <id>)* ////
        {
            var firstIdToken = Expect(TokenCategory.IDENTIFIER);
            /*
            var idlist = new IdList(){
                AnchorToken = Expect(TokenCategory.IDENTIFIER)
            };
            */
            var idlist = new IdList(){
                new Identifier(){
                    AnchorToken = firstIdToken
                }
            };

            while (CurrentToken == TokenCategory.COMMA)
            {
                Expect(TokenCategory.COMMA);
                idlist.Add(new Identifier(){
                    AnchorToken = Expect(TokenCategory.IDENTIFIER)
                });
            }
            return idlist;

            /*
            Expect(TokenCategory.IDENTIFIER);
            while (CurrentToken == TokenCategory.COMMA)
            {
                Expect(TokenCategory.COMMA);
                Expect(TokenCategory.IDENTIFIER);
            }*/
        }
        
        public Node FunDef() //<fun def> :: = <id> “(“ <param-list> “)” “{“ <var-def-list><stmt-list> ”}”
        { //DUDA
            //Console.WriteLine("Entre a FunDef");
            var defToken = Expect(TokenCategory.IDENTIFIER);
            Expect(TokenCategory.PARENTHESIS_OPEN);

            //var paramlist = ParamList(); <param-list> ::= <id-list>?
            var paramlist = new ParamList();
            if(CurrentToken == TokenCategory.IDENTIFIER)
            {
                paramlist.Add(IdList());
            }
            ///

            Expect(TokenCategory.PARENTHESIS_CLOSE);
            Expect(TokenCategory.BRACE_OPEN);
            
            //var vardeflist = VarDefList(); <var-def-list> ::= <var-def>*
            var vardeflist = new VarDefList();
            while (CurrentToken == TokenCategory.VAR)
            {
                vardeflist.Add(VarDef());
            }
            ///
            var stmtlist = StmtList();
            Expect(TokenCategory.BRACE_CLOSE);

            var fundef = new FunDef(){
                paramlist,
                vardeflist,
                stmtlist
            };

            fundef.AnchorToken = defToken;

            return fundef;

            /*
            Expect(TokenCategory.IDENTIFIER);
            Expect(TokenCategory.PARENTHESIS_OPEN);
            ParamList();
            Expect(TokenCategory.PARENTHESIS_CLOSE);
            Expect(TokenCategory.BRACE_OPEN);
            VarDefList();
            StmtList();
            Expect(TokenCategory.BRACE_CLOSE);
            */

        }
        /*        
        public Node ParamList() //  <param-list> ::= <id-list>?  // esta la podemos quitar
        {
            var paramlist = new ParamList();
            if(CurrentToken == TokenCategory.IDENTIFIER)
            {
                paramlist.Add(IdList());
            }
            return paramlist;
        }
        */
        /*
        public Node VarDefList()// <var-def-list> ::= <var-def>*
        {
            var vardeflist = new VarDefList();
            while (CurrentToken == TokenCategory.VAR)
            {
                vardeflist.Add(VarDef());
            }
            return vardeflist;
        }
        */
        public Node StmtList()//<stmt-list> ::= <stmt>*
        {
            var stmtlist = new StmtList();
            while (firstOfStatement.Contains(CurrentToken)){
                stmtlist.Add(Stmt());
            }
            return stmtlist;
        }

        public Node Stmt() { //<stmt>::=‹stmt-assign›|‹stmt-incr›|‹stmt-decr›|‹stmt-fun-call›|‹stmt-if›|‹stmt-while›|‹stmt-do-while›|‹stmt-break›|‹stmt-return›|‹stmt-empty›
            
            switch (CurrentToken) {

            case TokenCategory.IDENTIFIER:
                return StmtIdentifier();
                
            case TokenCategory.IF:
                return StmtIf();

            case TokenCategory.WHILE:
                return StmtWhile();
            case TokenCategory.DO:
                return StmtDoWhile();

            case TokenCategory.BREAK:
                return StmtBreak();
        
            case TokenCategory.RETURN:
                return StmtReturn();
            
            case TokenCategory.SEMI_COLON:
                return StmtEmpty();
            default:
                throw new SyntaxError(firstOfStatement, tokenStream.Current);
            }
        }
        
        public Node StmtIdentifier()
        {
            var idtoken = Expect(TokenCategory.IDENTIFIER);
            /*
            var stmtidentifier = new StmtIdentifier(){
                AnchorToken = Expect(TokenCategory.IDENTIFIER)
            };
            */
            switch (CurrentToken) {
                case TokenCategory.ASSIGN:
                    return StmtAssign(idtoken);
                    //break;

                case TokenCategory.INCR:
                    return StmtIncr(idtoken);
                    //break;

                case TokenCategory.DECR:
                    return StmtDecr(idtoken);
                    //break;

                case TokenCategory.PARENTHESIS_OPEN:
                    return StmtFunCall(idtoken);
                    //break;
                default:
                    throw new SyntaxError(firstOfStatement, tokenStream.Current);         
            }

            //return stmtidentifier;
        }

        
        public Node StmtAssign(Token token) //<stmt-assign>::=<id>”=”<expr>”;”
        { 
            //Console.WriteLine("StmtAssign");
            var stmtassign = new StmtAssign(){
                AnchorToken = token
            };
            //var assignToken = Expect(TokenCategory.ASSIGN);
            Expect(TokenCategory.ASSIGN);
            var expe = Expr();
            //var result = new StmtAssign(){ expe } ;
            stmtassign.Add(expe);
            Expect(TokenCategory.SEMI_COLON);
            //result.AnchorToken = assignToken;
            return stmtassign;
        }
        
        
        public Node StmtIncr(Token token) // <stmt-assign>::=<id>”++” ”;”
        {
            var stmtincr = new StmtIncr(){
                AnchorToken = token
            };
            Expect(TokenCategory.INCR);
            Expect(TokenCategory.SEMI_COLON);
            return stmtincr;
        }


        public Node StmtDecr(Token token) //<stmt-assign>::=<id>”--” ”;”
        {
            var stmtdecr = new StmtDecr(){
                AnchorToken = token
            };
            Expect(TokenCategory.DECR);
            Expect(TokenCategory.SEMI_COLON);
            return stmtdecr;
        }

        public Node StmtFunCall(Token token) //<stm-fun-call>::=<fun-call>”;” No es nodo
        {
            var stmtfuncall = new StmtFunCall(){
                AnchorToken = token
            };
            stmtfuncall.Add(FunCall());
            Expect(TokenCategory.SEMI_COLON);

            return stmtfuncall;
        }
        public Node FunCall(){ //‹fun-call› :: = ‹id› “(“ ‹expr-list› “)”
            //Console.WriteLine("FunCall");
            Expect(TokenCategory.PARENTHESIS_OPEN);
            var exprlist = ExprList();
            Expect(TokenCategory.PARENTHESIS_CLOSE);
            return exprlist;
        }
        public Node ExprList(){ //<expr-list> ::= (<expr><expr-list-cont>)? duda: nodo vacio 
            var exprlist = new ExprList();
            if(firstOfExpr.Contains(CurrentToken))
            {
                //var expr1 = Expr();
                exprlist.Add(Expr());

                //var exprlistcont = ExprListCont(); <expr-list-cont> ::=( “,” <expr>)*
                //var exprlistcont = new ExprListCont();

                while(CurrentToken == TokenCategory.COMMA)
                {
                    Expect(TokenCategory.COMMA);
                    //var expr2 = Expr();
                    //exprlistcont.Add(expr2);
                    exprlist.Add(Expr());
                }

                //
                
                    //Console.WriteLine("Expresion list");
                    //exprlist.Add(expr1);
                    //exprlist.Add(exprlistcont);
                    //Expr();
                    //ExprListCont();
            }
            return exprlist;
        }
        /*
        public Node ExprListCont(){ //<expr-list-cont> ::=( “,” <expr>)*
            var exprlistcont = new ExprListCont();

            while(CurrentToken == TokenCategory.COMMA)
            {
                Expect(TokenCategory.COMMA);
                var expr = Expr();
                exprlistcont.Add(expr);
                //Console.WriteLine("ExprListCont");
                //Expect(TokenCategory.COMMA);
                //Expr();
            }
            return exprlistcont;
        }
        */
        public Node StmtIf() //<stmt>::=”if” ”(“<expr>”)” ”{“  <stmt-list>”}” <else-if-list><else>
        {
            var stmtif = new StmtIf(){
                AnchorToken = Expect(TokenCategory.IF)
            };
        
            
            //Expect(TokenCategory.IF);
            Expect(TokenCategory.PARENTHESIS_OPEN);
            //stmtif.Add(Expr());
            stmtif.Add(new Condition(){
                Expr()
            });
            Expect(TokenCategory.PARENTHESIS_CLOSE);
            Expect(TokenCategory.BRACE_OPEN);
            stmtif.Add(StmtList());
            Expect(TokenCategory.BRACE_CLOSE);

            // <else-if-list>::= (“elif” ”(“ <expr> “)” “{“<stmt-list>”}”)*
            var elseiflist = new ElseIfList();
            
            while(CurrentToken == TokenCategory.ELIF){
                var elif = new Elif(){
                    AnchorToken = Expect(TokenCategory.ELIF)
                };
                //Expect(TokenCategory.ELIF);
                Expect(TokenCategory.PARENTHESIS_OPEN);
                elif.Add(Expr());
                //Expr();
                Expect(TokenCategory.PARENTHESIS_CLOSE);
                Expect(TokenCategory.BRACE_OPEN);
                elif.Add(StmtList());
                Expect(TokenCategory.BRACE_CLOSE);

                elseiflist.Add(elif);
            }
            stmtif.Add(elseiflist);
            //
            
            //stmtif.Add(Else()); <else> ::=(  “else” “{“ <stmt-list> “}” )?
            if(CurrentToken == TokenCategory.ELSE) // DUDA
            {   
                var elseToken =  new Else(){
                    AnchorToken = Expect(TokenCategory.ELSE)
                };
                //Expect(TokenCategory.ELSE);
                Expect(TokenCategory.BRACE_OPEN);
                elseToken.Add(StmtList());
                Expect(TokenCategory.BRACE_CLOSE);
                stmtif.Add(elseToken);

            }
            return stmtif;
            
        }
        /*
        public Node ElseIfList() // <else-if-list>::= (“elif” ”(“ <expr> “)” “{“<stmt-list>”}”)*
        {
            
            while(CurrentToken == TokenCategory.ELIF){
                
                Expect(TokenCategory.ELIF);
                Expect(TokenCategory.PARENTHESIS_OPEN);
                Expr();
                Expect(TokenCategory.PARENTHESIS_CLOSE);
                Expect(TokenCategory.BRACE_OPEN);
                StmtList();
                Expect(TokenCategory.BRACE_CLOSE);
            }
            
        }
        */
        /*
        public Node Else() //<else> ::=(  “else” “{“ <stmt-list> “}” )?
        {
            if(CurrentToken == TokenCategory.ELSE)
            {   Expect(TokenCategory.ELSE);
                Expect(TokenCategory.BRACE_OPEN);
                StmtList();
                Expect(TokenCategory.BRACE_CLOSE);
            }
        }
        */
        public Node StmtWhile(){ // <stmt-while>::=”while” “(“ <expr> “)” “{“ <stmt-list> “}” 

            var stmtwhile = new StmtWhile(){
                AnchorToken = Expect(TokenCategory.WHILE)
            };
            
            Expect(TokenCategory.PARENTHESIS_OPEN);
            //stmtwhile.Add(Expr());
            stmtwhile.Add(new LoopCondition(){
                Expr()
            });
            Expect(TokenCategory.PARENTHESIS_CLOSE);
            Expect(TokenCategory.BRACE_OPEN);
            stmtwhile.Add(StmtList());
            Expect(TokenCategory.BRACE_CLOSE);

            return stmtwhile;
        }

        public Node StmtDoWhile(){ //<stmt-do-while>::=”do” “{“ <stmt-list>”}” “while” “(“<expr>”)” “;” DUDA 
            var stmtdo = new StmtDoWhile(){
                AnchorToken = Expect(TokenCategory.DO)
            };
            
            Expect(TokenCategory.BRACE_OPEN);
            stmtdo.Add(StmtList());
            Expect(TokenCategory.BRACE_CLOSE);
            Expect(TokenCategory.WHILE); //DUDA 
            Expect(TokenCategory.PARENTHESIS_OPEN);
            //stmtdo.Add(Expr());
            stmtdo.Add(new LoopCondition(){
                Expr()
            });
            Expect(TokenCategory.PARENTHESIS_CLOSE);
            Expect(TokenCategory.SEMI_COLON);

            return stmtdo;
        }

        public Node StmtBreak() //<stmt-break> ::= “”break” ”;” DUDA 
        {
            //Expect(TokenCategory.BREAK);
            var stmtbreak = new StmtBreak(){
                AnchorToken = Expect(TokenCategory.BREAK)
            };
            Expect(TokenCategory.SEMI_COLON);
            return stmtbreak;
        }
        
        public Node StmtReturn() //<stmt> ::= “return” <expr> “;”
        {
            var stmtreturn = new StmtReturn(){
                AnchorToken = Expect(TokenCategory.RETURN)
            };
            stmtreturn.Add(Expr());
            Expect(TokenCategory.SEMI_COLON);
            return stmtreturn;
            
            /*Expect(TokenCategory.RETURN);
            Expr();
            Expect(TokenCategory.SEMI_COLON);*/
        }
        
       public Node StmtEmpty() //<stmt-empty> ::= “;” No es un nodo DUDA
       {
           return new StmtEmpty(){
               AnchorToken = Expect(TokenCategory.SEMI_COLON)
           };
           //Expect(TokenCategory.SEMI_COLON);
       }
        public Node Expr() //<expr> ::= <expr-or> No es nodo DUDA, return empty node
        { 
            //var expr = new Expr();

            //expr.Add(ExprOr()); 
            //‹expr-or›::= ‹expr-and› ("||" ‹expr-and›)*
            //expr.Add(ExprAnd());
            var expr = ExprAnd();
            while(CurrentToken == TokenCategory.OR){
                var expr2 = new ExprOr(){
                    AnchorToken = Expect(TokenCategory.OR)
                };
                expr2.Add(expr);
                expr2.Add(ExprAnd());
                expr = expr2;
            }

            return expr; 
        }

        /*
        public Node Expr() //<expr> ::= <expr-or> No es nodo DUDA, return empty node
        { 
            var expr = new Expr();

            //expr.Add(ExprOr()); 
            //‹expr-or›::= ‹expr-and› ("||" ‹expr-and›)*
            expr.Add(ExprAnd());
            while(CurrentToken == TokenCategory.OR){
                Expect(TokenCategory.OR);
                //ExprAnd();
                expr.Add(ExprAnd());
            }

            return expr; 
        }
        */
        /* public Node ExprOr() //‹expr-or›::= ‹expr-and› ("||" ‹expr-and›)*
        {
            var expror = new ExprOr();
            expror.Add(ExprAnd());
            
            while(CurrentToken == TokenCategory.OR){
                Expect(TokenCategory.OR);
                //ExprAnd();
                expror.Add(ExprAnd());
            }
            return expror;
        }*/
        public Node ExprAnd() //‹expr-and› ::= ‹expr-comp› ("&&" ‹expr-comp›)*
        {
            /*var exprand = new ExprAnd();
            exprand.Add(ExprComp());*/
            var initExpr = ExprComp();
            while(CurrentToken == TokenCategory.AND){
                var expr2 = new ExprAnd(){
                    AnchorToken = Expect(TokenCategory.AND)
                };
                expr2.Add(initExpr);
                //exprand.Add(ExprComp());
                expr2.Add(ExprComp());
                initExpr = expr2;
            }
            return initExpr;
        }

        public Node ExprComp() //<expr-comp>::= <expr-rel>(<op-comp><expr-rel>)* DUDA
        {
            //var exprcomp = new ExprComp();
            var initExpr = ExprRel();
            //exprcomp.Add(ExprRel());
            while(firstOfOpComp.Contains(CurrentToken)){
                var expr2 = OpComp();
                expr2.Add(initExpr);
                expr2.Add(ExprRel());
                initExpr = expr2;
                //exprcomp.Add(OpComp());
                //exprcomp.Add(ExprRel());
            }
            return initExpr;
        }

        public Node OpComp(){
            switch(CurrentToken){
                case TokenCategory.EQUAL:
                    return new Equal(){
                        AnchorToken = Expect(TokenCategory.EQUAL)
                    };
                case TokenCategory.UNEQUAL:
                    return new UnEqual(){
                        AnchorToken = Expect(TokenCategory.UNEQUAL)
                    };
                default:
                    throw new SyntaxError(firstOfOpComp,tokenStream.Current);
            }
        }
        public Node ExprRel() //<expr-rel>::=<expr-add>(<op-rel><expr-add>)*
        {
            var initExpr = ExprAdd();
            //exprel.Add(ExprAdd());
            while(firstOfOpRel.Contains(CurrentToken)){
                var expr2 = OpRel();
                expr2.Add(initExpr);
                expr2.Add(ExprAdd());
                initExpr = expr2;
                //exprel.Add(OpRel());
                //exprel.Add(ExprAdd());
            }
            return initExpr;
        }

        public Node OpRel(){ //<opr-rel> ::= “<”| “<=” | “>” | “>=”
            switch(CurrentToken){
                
                case TokenCategory.LESS:
                    return new Less(){
                        AnchorToken = Expect(TokenCategory.LESS)
                    };

                case TokenCategory.LESS_EQUAL:
                    return new Less_Equal(){
                        AnchorToken = Expect(TokenCategory.LESS_EQUAL)
                    };
                    
                case TokenCategory.MORE:
                    return new More(){
                        AnchorToken = Expect(TokenCategory.MORE)
                    };
                
                case TokenCategory.MORE_EQUAL:
                    return new More_Equal(){
                        AnchorToken = Expect(TokenCategory.MORE_EQUAL)
                    };
                    
                default:
                    throw new SyntaxError(firstOfOpRel,tokenStream.Current);
            }
            
        }
        public Node ExprAdd() // <expr-add> ::= <expr-mul>(<op-add><expr-mul>) * 
        {
            //var expradd = new ExprAdd();
            //expradd.Add(ExprMul());
            var initExpr = ExprMul();
            while(firstOfOpAdd.Contains(CurrentToken)){
                var expr2 = OpAdd();
                expr2.Add(initExpr);
                expr2.Add(ExprMul());
                initExpr = expr2;
                //expradd.Add(OpAdd());
                //expradd.Add(ExprMul());
            }
            return initExpr;
            /*ExprMul();
            while(firstOfOpAdd.Contains(CurrentToken)){
                OpAdd();
                ExprMul();
            }*/
            
        }


        public Node OpAdd() // <op-add>::= “+” | “-”
        {
            switch(CurrentToken){
                case TokenCategory.PLUS:
                    return new Plus(){
                        AnchorToken = Expect(TokenCategory.PLUS)
                    };
                case TokenCategory.NEG:
                    return new Neg(){
                        AnchorToken = Expect(TokenCategory.NEG)
                    }; 
                default:
                    throw new SyntaxError(firstOfOpAdd,tokenStream.Current);
            }
        }

        public Node ExprMul() // <expr-mul> ::= <expr-unary>(<op-mul><expr-unary>)*
        {
            var initExpr = ExprUnary();
            while(firstOfOpMul.Contains(CurrentToken)){
                var expr2 = OpMul();
                expr2.Add(initExpr);
                expr2.Add(ExprUnary());
                initExpr = expr2;
                //var expr2 = OpMul();
                //exprmul.Add(OpMul());
                //exprmul.Add(ExprUnary());
            }
            return initExpr;
            /*
            var exprmul = new ExprMul();
            exprmul.Add(ExprUnary());
            
            while(firstOfOpMul.Contains(CurrentToken)){
                exprmul.Add(OpMul());
                exprmul.Add(ExprUnary());
            }
            return exprmul;
            */
        }

        public Node OpMul(){ //<op-mul>::= ”*” | “/” | “%”
             switch(CurrentToken){
                case TokenCategory.MUL:
                    return new Mul(){
                        AnchorToken = Expect(TokenCategory.MUL)
                    };
                case TokenCategory.DIV:
                    return new Div(){
                        AnchorToken = Expect(TokenCategory.DIV)
                    }; 
                case TokenCategory.MOD:
                    return new Mod(){
                        AnchorToken = Expect(TokenCategory.MOD)
                    }; 
                default: 
                    throw new SyntaxError(firstOfOpMul,
                                      tokenStream.Current);

            }
        }
        /*
        ‹expr-unary› ::= <OP-UNARY> <EXPR-UNARY>|<EXPR-PRIMARY>
        */

        public Node ExprUnary() // ‹expr-unary› ::=  ‹op-unary› * ‹expr-primary›
        {
            if(firstOfOpUnary.Contains(CurrentToken)){
                var opunary = OpUnary();
                var rootNode = ExprUnary();
                opunary.Add(rootNode);
                return opunary;
            }
            return ExprPrimary();
        }

        /*            
        public Node ExprUnary() // ‹expr-unary› ::=  ‹op-unary› * ‹expr-primary›
        {
            var exprunary = new ExprUnary();
            
            //var rootNode = ExprPrimary();
            while(firstOfOpUnary.Contains(CurrentToken)){
                //var opunary = OpUnary();
                Console.WriteLine("ExprUnary on OPUnary");
                //var node = OpUnary();
                //node.Add(rootNode);
                //rootNode = node;
                exprunary.Add(OpUnary());
            }
            //rootNode.Add(ExprPrimary());

            //var expr = ExprPrimary();
            exprunary.Add(ExprPrimary());
            //Console.WriteLine("ExprUnary");
            
            return exprunary;
        }*/

        /*
        public Node ExprUnary() // ‹expr-unary› ::=  ‹op-unary› * ‹expr-primary›
        {
            var exprunary = new ExprUnary();
            while(firstOfOpUnary.Contains(CurrentToken)){
                //Console.WriteLine("ExprUnary on OPUnary");
                exprunary.Add(OpUnary());    
            }
            //Console.WriteLine("ExprUnary");
            exprunary.Add(ExprPrimary());
            return exprunary;
        }
        */
        public Node OpUnary() // <op-unary> ::= “+” | “-” | “!”
        {
            switch(CurrentToken){
                case TokenCategory.PLUS:
                    return new Plus(){
                        AnchorToken = Expect(TokenCategory.PLUS)
                    };
                case TokenCategory.NEG:
                    return new Neg(){
                        AnchorToken = Expect(TokenCategory.NEG)
                    }; 
                case TokenCategory.NOT:
                    return new Not(){
                        AnchorToken = Expect(TokenCategory.NOT)
                    }; 
                default: 
                    throw new SyntaxError(firstOfOpUnary,
                                      tokenStream.Current);
            }
        }
      
        public Node ExprPrimary(){ //<expr-primary> ::= <id> | <fun-call> | <array> | <lit> | (“(“ <expr> “)”) DUDA
            
            //var exprprimary = new ExprPrimary();
            if(firstOfLit.Contains(CurrentToken)){
                //exprprimary.Add(Lit());
                return Lit();
            } else {
                switch(CurrentToken){

                    case TokenCategory.IDENTIFIER:
                        //exprprimary.Add(ExprPrimaryIdentifier());
                        return ExprPrimaryIdentifier();
                        //break;
                    case TokenCategory.BRACKET_OPEN:
                        //exprprimary.Add(Array());
                        return Array();
                        //break;
                    case TokenCategory.PARENTHESIS_OPEN:
                        Expect(TokenCategory.PARENTHESIS_OPEN);
                        //exprprimary.Add(Expr());
                        var expr = Expr();
                        Expect(TokenCategory.PARENTHESIS_CLOSE);
                        return expr;
                        //break;
                    default: 
                        throw new SyntaxError(TokenCategory.PARENTHESIS_OPEN,
                                      tokenStream.Current);

                }
            }
            //return exprprimary;
        }

        public Node ExprPrimaryIdentifier()//error because does not need 
        {
            /*
            var exprprimaryid = new ExprPrimaryIdentifier(){
                AnchorToken = Expect(TokenCategory.IDENTIFIER)
            };
            */
            var token = Expect(TokenCategory.IDENTIFIER);
            var identityNode = new Identifier(){
                AnchorToken = token
            };
            //Expect(TokenCategory.IDENTIFIER);
            if(CurrentToken==TokenCategory.PARENTHESIS_OPEN){
                //Console.WriteLine("Expr Primary Identifier");
                //exprprimaryid.Add(FunCall());
                var funcall = new FunCall(){
                    AnchorToken = token
                };
                funcall.Add(FunCall());
                return funcall;
            } else {
                return identityNode;
            }
            //return exprprimaryid;
        }

        public Node Array() //<array>::= “[“ <expr-list> “]”
        {
            var arr = new Array();
            Expect(TokenCategory.BRACKET_OPEN);
            arr.Add(ExprList());
            Expect(TokenCategory.BRACKET_CLOSE);
            return arr;
        }
        
        public Node Lit(){ //<lit>::= <lit-bool>|<lit-int>|<lit-char>|<lit-str
             switch(CurrentToken){
                case TokenCategory.TRUE :
                    return new True(){
                        AnchorToken = Expect(TokenCategory.TRUE)
                    };
        
                case TokenCategory.FALSE:
                    return new False(){
                        AnchorToken = Expect(TokenCategory.FALSE)
                    };
                    
                case TokenCategory.INT_LITERAL:
                    return new Int_Literal(){
                        AnchorToken = Expect(TokenCategory.INT_LITERAL)
                    };

                case TokenCategory.CHAR_LITERAL:
                    return new Char_Literal(){
                        AnchorToken = Expect(TokenCategory.CHAR_LITERAL)
                    };

                case TokenCategory.STRING_LITERAL:
                    return new String_Literal(){
                        AnchorToken = Expect(TokenCategory.STRING_LITERAL)
                    };
                    
                default: 
                    throw new SyntaxError(firstOfOpUnary,
                                      tokenStream.Current);
            }
        }
    
    }
}
